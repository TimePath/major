using System;
using Major.Proto;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace Net.Client
{
    class ConnectionTest : ProtoConnection
    {
        public ConnectionTest (IPEndPoint ipEndpoint) : base (ipEndpoint)
        {
        }

        [Callback]
        void List (FileListing l)
        {
            Console.WriteLine("Got {0}", l);
            Write(Meta.CreateBuilder().SetFiles(l).Build());
        }
    }

    class Program
    {
        public static void Main (string[] args)
        {
            ProtoConnection c = new ConnectionTest (new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 9001));
            c.Connect (true);
            c.Write (Meta.CreateBuilder ()
                .SetFiles (FileListing.CreateBuilder ()
                    .AddFile (Major.Proto.FileListing.Types.File.CreateBuilder ()
                        .SetName ("text.txt")
                        .SetBody ("text")
                        .SetType (Major.Proto.FileListing.Types.File.Types.FileType.FILE)
                        .Build ())
                    .Build ())
                .Build ()
            );
        }
    }
}

