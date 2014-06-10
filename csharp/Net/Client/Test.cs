using System;
using Major.Proto;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace Net.Client
{
    class Program
    {
        public static void Main (string[] args)
        {
            ProtoConnection c = new ProtoConnection (new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 9001));
            c.Connect (true);
            for (int i = 0; i < 100000; i++) {
                FileListing fl = c.Write<FileListing> (Meta.CreateBuilder ()
                .SetListRequest (ListRequest.CreateBuilder ()
                    .SetPath ("/")
                    .Build ())
                .Build ());
                Console.WriteLine (fl.GetFile (0));
            }
        }
    }
}

