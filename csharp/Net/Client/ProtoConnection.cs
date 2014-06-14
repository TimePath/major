using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Reflection;
using Major.Proto;
using Google.ProtocolBuffers;

namespace Net.Client
{
    /// <summary>
    /// Decorated methods are considered in the callback process.
    /// </summary>
    public class CallbackAttribute : Attribute
    {
    }

    public class ProtoConnection
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger ();
        private Socket connection;
        private IPEndPoint remote;
        private NetworkStream netStream;

        internal ProtoConnection ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Net.Client.ProtoConnection"/> class.
        /// </summary>
        /// <param name="ipEndpoint">Ip address: new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000)</param>
        public ProtoConnection (IPEndPoint ipEndpoint)
        {
            this.remote = ipEndpoint;
        }

        /// <summary>
        /// Read data from the connected stream.
        /// </summary>
        internal void Read ()
        {
            Callback (this, Meta.ParseDelimitedFrom (netStream));
        }

        internal bool IsApplicable (MethodInfo method, object o)
        {
            if (method.GetCustomAttributes (typeof(CallbackAttribute), false).Length == 0)
                return false;
            ParameterInfo[] c = method.GetParameters ();
            if (c.Length != 1)
                return false;
            return c [0].ParameterType.IsAssignableFrom (o.GetType ());
        }

        private readonly Dictionary<Type, FutureMessage> locks = new Dictionary<Type, FutureMessage> ();

        private FutureMessage GetFuture (Type type)
        {
            lock (locks) {
                if (!locks.ContainsKey (type)) {
                    return locks [type] = new FutureMessage ();
                }
                return locks [type];
            }
        }

        private class FutureMessage
        {
            private LinkedList<IMessageLite> list = new LinkedList<IMessageLite> ();
            private EventWaitHandle notify = new AutoResetEvent (false);
            private object readLock = new {};

            public void Push (IMessageLite o)
            {
                lock (list) {
                    list.AddLast (o);
                    Notify (); // Wake up
                }
            }

            public void Notify ()
            {
                if (list.Count == 0)
                    return;
                notify.Set ();
            }

            public IMessageLite Pop ()
            {
                while (true) {
                    lock (readLock) { // One reading thread at a time
                        notify.WaitOne ();
                        lock (list) { // Stop writing to the list
                            if (list.First == null) { // Must have data safeguard
                                continue;
                            }
                            IMessageLite response = list.First.Value;
                            list.RemoveFirst ();
                            return response;
                        }
                    }
                }
            }
        }

        internal void Callback (object callbacks, Meta msg)
        {
            foreach (var f in msg.AllFields.Values) {
                var o = f as IMessageLite;
                if (o == null)
                    continue;

                GetFuture (o.GetType ()).Push (o);

                MethodInfo m = null;
                foreach (var methodInfo in callbacks.GetType ().GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)) {
                    if (IsApplicable (methodInfo, o)) {
                        m = methodInfo;
                        break;
                    }
                }
                if (m == null) {
                    logger.Debug ("No callback for '{0}'.", o.GetType ());
                    continue;
                }
                try {
                    m.Invoke (callbacks, new object[]{ o });
                } catch (Exception e) {
                    logger.Warn ("Callback failed for '{0}': {1}.", o.GetType (), e);
                }
            }
        }

        private int counter;

        public Meta.Builder CreateBuilder ()
        {
            return Meta.CreateBuilder ().SetTag (Interlocked.Increment (ref counter));
        }

        /// <summary>
        /// Write the specified msg and wait for a response of type T.
        /// This method will be the first to know before any callbacks are fired.
        /// </summary>
        /// <param name="msg">Message.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T Write<T> (Meta msg) where T : IMessageLite
        {
            Write (msg);
            Type type = typeof(T);
            return (T)GetFuture (type).Pop ();
        }

        /// <summary>
        /// Write the specified msg. Does not wait for a response.
        /// </summary>
        /// <param name="msg">Message.</param>
        public void Write (IMessageLite msg)
        {
            if (netStream != null) // Allow for testing without a server
                msg.WriteDelimitedTo (netStream);
        }

        /// <summary>
        /// Connect the client and begin reading on a new thread.
        /// </summary>
        /// <param name="blocking">If set to <c>true</c>, blocks until connected.</param>
        public void Connect (bool blocking)
        {
            ManualResetEvent initialized = new ManualResetEvent (false);
            connection = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            connection.BeginConnect (remote, ar => {
                connection.EndConnect (ar);
                logger.Info ("Connected to {0}", remote.ToString ());
                initialized.Set ();

                netStream = new NetworkStream (connection, false);
                new Thread (() => {
                    Thread.CurrentThread.Name = "Networking";
                    int timeout = 5;
                    while (true) {
                        // Await data
                        if (!connection.Poll (timeout * 1000000, SelectMode.SelectRead)) {
                            logger.Warn ("{0} seconds of inactivity", timeout);
                            foreach (var l in locks.Values) {
                                l.Notify ();
                            }
                            continue;
                        }
                        Read ();
                    }
                }).Start ();

            }, null);
            if (blocking) {
                initialized.WaitOne (); // Block until connected
            }
        }
    }
}
