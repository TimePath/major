using System;
using Major.Proto;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace Net.Client
{
    class CallbackMonitor
    {
        [Callback] void test (ListResponse d)
        {
            var i = 0;
        }

        [Callback] void test (FileChunk d)
        {
            var i = 0;
        }
    }

    class Program
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger ();

        public static void Main (string[] args)
        {
            Threading ();
            //Benchmark ();
        }

        static void Threading ()
        {
            var c = new ProtoConnection ();
            var callbacks = new CallbackMonitor ();
            Random random = new Random();
            new Thread (() => {
                while (true) {
                    c.Callback (callbacks, c.CreateBuilder ()
                        .SetFiles (ListResponse.CreateBuilder ().Build ())
                        .Build ());
                    Thread.Sleep(random.Next(0, 10));
                }
            }).Start ();
            new Thread (() => {
                while (true) {
                    c.Write<ListResponse> (null);
                    Thread.Sleep(random.Next(0, 10));
                }
            }).Start ();
            new Thread (() => {
                while (true) {
                    c.Callback (callbacks, c.CreateBuilder ()
                        .SetChunk(FileChunk.CreateBuilder().Build())
                        .Build ());
                    Thread.Sleep(random.Next(0, 10));
                }
            }).Start ();
            new Thread (() => {
                while (true) {
                    c.Write<FileChunk> (null);
                    Thread.Sleep(random.Next(0, 10));
                }
            }).Start ();
        }

        static void Benchmark ()
        {
            ProtoConnection c = new ProtoConnection (new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 9001));
            c.Connect (true);
            for (int i = 0; i < 100000; i++) {
                ListResponse fl = c.Write<ListResponse> (c.CreateBuilder ()
                    .SetListRequest (ListRequest.CreateBuilder ()
                        .SetPath ("/")
                        .Build ())
                    .Build ());
                logger.Log (i % 1000 == 0 ? NLog.LogLevel.Info : NLog.LogLevel.Debug, fl.GetFile (0));
            }
        }
    }
}

