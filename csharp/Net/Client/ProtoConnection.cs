using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.ProtocolBuffers;
using Major.Proto;
using System.Reflection;

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
            foreach (var o in msg.AllFields.Values) {
                if (o == null)
                    continue;
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
