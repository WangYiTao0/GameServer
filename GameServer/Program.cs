using System.Globalization;
using AsServer;

namespace GameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServePeer server = new ServePeer();
            //指定应用层
            server.SetApplication(new NetMsgCenter());
            server.Start(6666, 10);

            Console.ReadKey();     
        }
    }
}