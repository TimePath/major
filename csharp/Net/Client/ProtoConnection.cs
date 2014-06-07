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

namespace Net.Client
{
    public class ProtoConnection
    {
        private Socket connection;
        private IPEndPoint remote;
        private NetworkStream netStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="Net.Client.AsyncClient"/> class.
        /// </summary>
        /// <param name="ipEndpoint">Ip address: new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000)</param>
        public ProtoConnection (IPEndPoint ipEndpoint)
        {
            this.remote = ipEndpoint;
        }

        public Meta Read ()
        {
            return Meta.ParseDelimitedFrom (netStream);
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
                        Meta msg = Read ();
                        Console.WriteLine ("Got {0}", msg);
                    }
                }).Start ();

            }, null);
            if (block) {
                initialized.WaitOne (); // Block until connected
            }
        }
    }
}
