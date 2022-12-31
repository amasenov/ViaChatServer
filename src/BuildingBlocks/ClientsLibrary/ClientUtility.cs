using Microsoft.AspNetCore.SignalR.Client;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ClientsLibrary
{
    public static class ClientUtility
    {
        static HubConnection _connection = null;
        static string _user = null;
        static string _room = null;
        static string _message = null;

        public static async Task<bool> HandleCommandAsync(string command, bool useLive = false)
        {
            var parts = command?.Split(" ");
            var commandPart = !string.IsNullOrWhiteSpace(command) ? parts[0].Trim() : string.Empty;
            if(!Enum.TryParse(commandPart, true, out CommandType commandType))
            {
                Console.Write($"Invalid command. Please try again.{Environment.NewLine}");
            }
            if (commandType == CommandType.Exit)
            {
                return true;
            }

            bool isGet = commandType == CommandType.Get;
            // accept values with at least 3 charectars
            string value = parts?.Length > 1 && (parts[1].Length > 2 || isGet) ? parts[1] : string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                if ((commandType == CommandType.Connect) && Uri.TryCreate(value, UriKind.Absolute, out Uri url) && url.IsWellFormedOriginalString())
                {
                    _connection = new HubConnectionBuilder()
                        .WithUrl(url) // "https://localhost:44382/chat"
                        .WithAutomaticReconnect()
                        .Build();

                    await ConnectWithRetryAsync(_connection);
                }
                else if (commandType == CommandType.User)
                {
                    _user = value;
                }
                else if ((commandType == CommandType.Room) && !string.IsNullOrWhiteSpace(_user))
                {
                    _room = value;
                    await _connection.InvokeAsync("JoinRoom", new { user = _user, room = _room });
                }
                else if ((commandType == CommandType.Leave) && !string.IsNullOrWhiteSpace(_room))
                {
                    _room = null;
                    await _connection.InvokeAsync("LeaveRoom");
                }
                else if ((commandType == CommandType.Get) && !string.IsNullOrWhiteSpace(_room) && int.TryParse(value, out int limit))
                {
                    await _connection.InvokeAsync("GetLastMessages", _room, limit);
                }
                else if ((commandType == CommandType.Post) && !string.IsNullOrWhiteSpace(_room))
                {
                    _message = value;
                    await _connection.InvokeAsync("SendMessage", _message);
                }
                else if (useLive && (commandType == CommandType.Live))
                {
                    _room = value;
                    await _connection.InvokeAsync("JoinRoom", new { user = _user, room = _room });

                    _connection.On<string, string>("ReceiveMessage", (user, message) =>
                    {
                        Console.Write($"{user}: {message}{Environment.NewLine}");
                    });
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

                    connection.On<string, string>("ReceiveLastMessages", (user, message) =>
                    {
                        Console.Write($"{user}: {message}{Environment.NewLine}");
                    });
                    //connection.On<string, string>("ReceiveMessage", (user, message) =>
                    //{
                    //    Console.Write($"{user}: {message}{Environment.NewLine}");
                    //});
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
