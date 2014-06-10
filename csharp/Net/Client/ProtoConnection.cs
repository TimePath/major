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
    public class CallbackAttribute : Attribute
    {
    }

    public class ProtoConnection
    {
        private Socket connection;
        private IPEndPoint remote;
        private NetworkStream netStream;

        protected ProtoConnection ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Net.Client.AsyncClient"/> class.
        /// </summary>
        /// <param name="ipEndpoint">Ip address: new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000)</param>
        public ProtoConnection (IPEndPoint ipEndpoint)
        {
            this.remote = ipEndpoint;
        }

        public void Read ()
        {
            Callback (this, Meta.ParseDelimitedFrom (netStream));
        }

        private bool IsApplicable (MethodInfo method, object o)
        {
            if (method.GetCustomAttributes (typeof(CallbackAttribute), false).Length == 0)
                return false;
            ParameterInfo[] c = method.GetParameters ();
            if (c.Length != 1)
                return false;
            return c [0].ParameterType.IsAssignableFrom (o.GetType ());
        }

        protected void Callback (object callbacks, Meta msg)
        {
            foreach (var f in msg.AllFields.Values) {
                var o = f as IMessageLite;
                if (o == null)
                    continue;
                Type type = o.GetType ();
                LinkedList<IMessageLite> list;
                lock (buffer) {
                    if (!buffer.ContainsKey (o.GetType ())) {
                        list = buffer [type] = new LinkedList<IMessageLite> ();
                    } else {
                        list = buffer [type];
                    }
                }
                list.AddLast (o);
                lock (locks) {
                    if (locks.ContainsKey (type)) {
                        locks [type].Set ();
                    }
                }

                MethodInfo m = null;
                foreach (var methodInfo in callbacks.GetType ().GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)) {
                    if (IsApplicable (methodInfo, o)) {
                        m = methodInfo;
                        break;
                    }
                }
                if (m == null) {
                    throw new SystemException ("No callback for '" + o.GetType () + "'.");
                }
                try {
                    m.Invoke (this, new object[]{ o });
                } catch (Exception e) {
                    throw new SystemException ("Callback failed for '" + o.GetType () + "'.", e);
                }
            }
        }

        Dictionary<Type, ManualResetEvent> locks = new Dictionary<Type, ManualResetEvent> ();
        Dictionary<Type, LinkedList<IMessageLite>> buffer = new Dictionary<Type, LinkedList<IMessageLite>> ();

        /// <summary>
        /// Write the specified msg and wait for a response of type T.
        /// This method will be the first to know before any callbacks are fired.
        /// </summary>
        /// <param name="msg">Message.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T Write<T> (IMessageLite msg) where T : IMessageLite
        {
            Type type = typeof(T);
            ManualResetEvent l;
            lock (locks) {
                if (!locks.ContainsKey (type)) { // Never expected one of these messages before, initialize first
                    l = locks [type] = new ManualResetEvent (false);
                } else { // Re-use existing
                    l = locks [type];
                    l.Reset ();
                }
            }
            Write (msg);
            l.WaitOne ();
            lock (buffer) {
                IMessageLite response = buffer [type].First.Value;
                buffer [type].RemoveFirst ();
                return (T)response;
            }
        }

        public void Write (IMessageLite msg)
        {
            msg.WriteDelimitedTo (netStream);
        }

        public void Connect (bool block)
        {
            ManualResetEvent initialized = new ManualResetEvent (false);
            connection = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            connection.BeginConnect (remote, ar => {
                connection.EndConnect (ar);
                Console.WriteLine ("Connected to {0}", remote.ToString ());
                initialized.Set ();

                netStream = new NetworkStream (connection, false);
                new Thread (() => {
                    while (true) {
                        // Await data
                        if (!connection.Poll (-1, SelectMode.SelectRead)) {
                            continue;
                        }
                        Read ();
                    }
                }).Start ();

            }, null);
            if (block) {
                initialized.WaitOne (); // Block until connected
            }
        }
    }
}
