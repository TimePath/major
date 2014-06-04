using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Major.Proto;

namespace Net.Client
{
    public class AsyncClient
    {
        public static void Main (string[] args)
        {
            AsyncClient c = new AsyncClient (new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 9001));
            c.Connect ();
            (new Thread (() => {
                FileListing f = FileListing.CreateBuilder ()
                                .AddFile (Major.Proto.File.CreateBuilder ()
                        .SetName ("text.txt")
                        .SetBody ("text")
                        .SetType (Major.Proto.File.Types.FileType.FILE)
                        .Build ())
                    .Build ();
                c.Write(f);
            })).Start ();
        }

        private Socket connection;
        private IPEndPoint remote;
        private NetworkStream netStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="Net.Client.AsyncClient"/> class.
        /// </summary>
        /// <param name="ipEndpoint">Ip address: new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000)</param>
        public AsyncClient (IPEndPoint ipEndpoint)
        {
            this.remote = ipEndpoint;
        }

        public void Write(FileListing msg)
        {
            var buf = new MemoryStream();
            msg.WriteTo(buf);

            netStream.WriteShort(buf.Length); 
            buf.WriteTo(netStream);
            netStream.Flush();
        }

        public FileListing Read()
        {
            int length = (netStream.ReadByte() << 8) | netStream.ReadByte();
            byte[] buf = new byte[length];
            int total = 0;
            while (total < length)
            {
                total += netStream.Read(buf, total, length - total);
            }
            return FileListing.ParseFrom(new MemoryStream(buf));
        }

        public void Connect ()
        {
            ManualResetEvent initialized = new ManualResetEvent (false);
            connection = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            connection.BeginConnect (remote, ar => {
                connection.EndConnect (ar);
                Console.WriteLine ("Connected to {0}", remote.ToString ());
                initialized.Set ();

                netStream = new NetworkStream (connection, false);

                Thread netThread = new Thread (() => {
                    while (true) {
                        // Await data
                        if (!connection.Poll (-1, SelectMode.SelectRead)) {
                            continue;
                        }
                        FileListing f = Read();
                        Console.WriteLine ("Got {0}", f.GetFile(0));
                    }
                });
                netThread.Start ();
            }, null);
            // Block until connected
            initialized.WaitOne ();
        }
    }
}
