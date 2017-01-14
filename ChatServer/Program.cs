using System;
using System.Net;

namespace ChatServer
{
    internal class Program
    {
        static void Main(String[] args)
        {
            IPAddress hostAddress = IPAddress.Loopback;

            if (args.Length > 0)
            {
                if (!IPAddress.TryParse(args[0], out hostAddress))
                {
                    hostAddress = IPAddress.Loopback;
                }
            }
            
            Int32 hostPort = 7777;

            if (args.Length > 1)
            {
                if (!Int32.TryParse(args[1], out hostPort))
                {
                    hostPort = 7777;
                }
            }

            Database db = new Database(
                IPAddress.Parse("37.59.104.44"), 
                "nevrax_b00233705", "uws*290393*ws",
                "nevrax_university");
            db.Open();

            ChatApplication chatApp = new ChatApplication(hostAddress, hostPort);
            chatApp.Open(db);
            chatApp.Run();
            Console.WriteLine("Chat Service is running at {0}", chatApp.Address);

            hostPort += 11;

            MemberApplication memberApp = new MemberApplication(hostAddress, hostPort);
            memberApp.Open(db);
            memberApp.Run();
            Console.WriteLine("Member Service is running at {0}", memberApp.Address);

            Console.WriteLine("Press any key to exit ...");
            Console.ReadLine();

            memberApp.Close();
            chatApp.Close();
            db.Close();
        }
    }
}
