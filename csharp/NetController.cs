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
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger ();

        public Connection (IPEndPoint ipEndpoint) : base (ipEndpoint)
        {
        }

        [Callback]
        void Info (InfoResponse i)
        {
            logger.Debug ("Got {0}", i);
        }
    }

    public class NetController
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger ();
        ProtoConnection conn;

        public void Connect (string user, string password)
        {
            conn = new Connection (new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 9001));
            conn.Connect (true);
            logger.Info ("Connected", user, password);
            new Thread (() => {
                VFSConsumer c = new FUSEConsumer ();
                AppDomain.CurrentDomain.ProcessExit += (s, e) => {
                    logger.Debug ("Process exiting");
                    c.Stop ();
                };
                c.Start (new NetProvider (conn), opts);
                logger.Debug ("Finished");
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

