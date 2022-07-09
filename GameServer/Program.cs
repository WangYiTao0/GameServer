using AsServer;

namespace GameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            ServePeer server = new ServePeer();
            server.Start(6666, 10);

            Console.ReadKey();     
        }
    }
}