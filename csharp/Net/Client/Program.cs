using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.ProtocolBuffers.Examples.AddressBook;

namespace Net.Client
{
	public class AsyncClient
	{
		public static void Main (string[] args)
		{
			AsyncClient c = new AsyncClient (new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 9001));
			c.Connect ();
			(new Thread (() => {
				Person contact = Person.CreateBuilder ()
					.SetId (1)
					.SetName ("Foo")
					.SetEmail ("foo@bar.com")
					.AddPhone (Person.Types.PhoneNumber.CreateBuilder ()
						.SetNumber ("1234-5678")
						.Build ())
					.Build ();

				AddressBook book = AddressBook.CreateBuilder ()
					.AddPerson (contact)
					.Build ();

				book.WriteDelimitedTo (c.netStream);
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
						Person p = Person.ParseDelimitedFrom (netStream);
						Console.WriteLine (p);
					}
				});
				netThread.Start ();
			}, null);
			// Block until connected
			initialized.WaitOne ();
		}
	}
}