namespace Clients.RealtimeClient
{
    using ClientsLibrary;

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

                bool shouldBreak = await ClientUtility.HandleCommandAsync(command, true);
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