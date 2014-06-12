using System;
using Major.Proto;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace Net.Client
{
    class Program
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger ();

        public static void Main (string[] args)
        {
            ProtoConnection c = new ProtoConnection (new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 9001));
            c.Connect (true);
            for (int i = 0; i < 100000; i++) {
                ListResponse fl = c.Write<ListResponse> (Meta.CreateBuilder ()
                    .SetTag (i)
                    .SetListRequest (ListRequest.CreateBuilder ()
                        .SetPath ("/")
                        .Build ())
                    .Build ());
                logger.Log (i % 1000 == 0 ? NLog.LogLevel.Info : NLog.LogLevel.Debug, fl.GetFile (0));
            }
        }
    }
}

