using Microsoft.AspNetCore.SignalR.Client;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ClientsLibrary
{
    public static class ClientUtility
    {
        static HubConnection _connection;
        static string _user;
        static string _room;
        static string _message;

        public static async Task<bool> HandleCommandAsync(string command)
        {
            var parts = command?.Split(" ");
            var commandPart = !string.IsNullOrWhiteSpace(command) ? parts[0] : string.Empty;
            if (commandPart.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            bool isGet = commandPart.Equals("get", StringComparison.OrdinalIgnoreCase);
            // accept values with at least 3 charectars
            string value = parts?.Length > 1 && (parts[1].Length > 2 || isGet) ? parts[1] : string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (command.StartsWith("connect", StringComparison.OrdinalIgnoreCase) && Uri.TryCreate(value, UriKind.Absolute, out Uri url) && url.IsWellFormedOriginalString())
                {
                    _connection = new HubConnectionBuilder()
                        .WithUrl(url) // "https://localhost:44382/chat"
                        .WithAutomaticReconnect()
                        .Build();

                    await ConnectWithRetryAsync(_connection);
                }
                else if (command.StartsWith("user", StringComparison.OrdinalIgnoreCase))
                {
                    _user = value;
                }
                else if (command.StartsWith("room", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(_user))
                {
                    _room = value;
                    await _connection.InvokeAsync("JoinRoom", new { user = _user, room = _room });
                }
                else if (command.StartsWith("get", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(_room) && int.TryParse(value, out int limit))
                {
                    await _connection.InvokeAsync("GetLastMessages", _room, limit);
                }
                else if (command.StartsWith("post", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(_room))
                {
                    _message = value;
                    await _connection.InvokeAsync("SendMessage", _message);
                }
                else if (command.StartsWith("post", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(_room))
                {
                    _message = value;
                    await _connection.InvokeAsync("SendMessage", _message);
                }
                else
                {
                    Console.Write($"Invalid command. Please try again.{Environment.NewLine}");
                }
            }
            else
            {
                Console.Write($"Invalid command. Please try again.{Environment.NewLine}");
            }

            return false;
        }
        private static async Task<bool> ConnectWithRetryAsync(HubConnection connection)
        {
            // Keep trying to until we can start or the token is canceled.
            while (true)
            {
                try
                {
                    await connection.StartAsync();
                    Debug.Assert(connection.State == HubConnectionState.Connected);

                    connection.On<string, string>("ReceiveMessage", (user, message) =>
                    {
                        Console.Write($"{user}: {message}{Environment.NewLine}");
                    });
                    return true;
                }
                catch
                {
                    // Failed to connect, trying again in 5000 ms.
                    Debug.Assert(connection.State == HubConnectionState.Disconnected);
                    await Task.Delay(5000);
                }
            }
        }
    }
}
