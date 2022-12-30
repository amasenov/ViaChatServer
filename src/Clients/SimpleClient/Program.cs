namespace Clients.SimpleClient
{
    using ClientsLibrary;

    using Microsoft.AspNetCore.SignalR.Client;

    using System;
    using System.Threading.Tasks;


    class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.Write("Enter a command: ");
                var command = Console.ReadLine();

                bool shouldBreak = await ClientUtility.HandleCommandAsync(command, false);
                if (shouldBreak)
                {
                    Console.Write($"{Environment.NewLine}Press any key to exit...");
                    break;
                }
            }

            Console.ReadKey(true);
        }
    }
}