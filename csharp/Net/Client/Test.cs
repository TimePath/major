using System;
using Major.Proto;
using System.Net;
using System.Threading;

namespace Net.Client
{
    class Program
    {
        public static void Main (string[] args)
        {
            ProtoConnection c = new ProtoConnection (new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 9001));
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

