// See https://aka.ms/new-console-template for more information
using NetworkLab6;

class Program
{
   
    static ServerTcp serverTcp;
    static Thread listenThread; // поток для прослушивания
    private static void Main(string[] args)
    {
       
        serverTcp = new ServerTcp();
        try
        {
            listenThread = new Thread(new ThreadStart(serverTcp.Initiate));
            listenThread.Start(); //старт потока
        }
        catch (Exception ex)
        {
            serverTcp.Shutdown();
            Console.WriteLine(ex.Message);
        }
    }
}