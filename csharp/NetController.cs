using System;
using System.Net;
using System.Threading;
using FS;
using FS.Consumer.FUSE;
using Major.Proto;
using Net.Client;

namespace GUI
{
    class Connection : ProtoConnection
    {
        public Connection (IPEndPoint ipEndpoint) : base (ipEndpoint)
        {
        }

        [Callback]
        void List (FileListing l)
        {
            Console.WriteLine ("Got {0}", l);
        }
    }

    public class NetController
    {
        ProtoConnection conn;

        public void Connect (string user, string password)
        {
            conn = new Connection (new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 9001));
            conn.Connect (true);
            Console.WriteLine ("Connected", user, password);
            new Thread (() => {
                VFSConsumer c = new FUSEConsumer ();
                AppDomain.CurrentDomain.ProcessExit += (s, e) => {
                    Console.WriteLine ("Process exiting");
                    c.Stop ();
                };
                c.Start (new NetProvider (conn), opts);
                Console.WriteLine ("Finished");
            }).Start ();
        }

        MountOptions opts;

        public NetController (MountOptions opts)
        {
            if (opts == null)
                throw new ArgumentNullException ("opts");
            this.opts = opts;
        }
    }
}

