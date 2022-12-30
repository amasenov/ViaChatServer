using Chat.API.Models;
using Chat.Application.Models;
using Chat.Application.Models.Enumerations.Includes;
using Chat.Application.Services;

using Microsoft.AspNetCore.SignalR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;

namespace Chat.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;

        private readonly PostService _postService;
        private readonly UserService _userService;
        private readonly RoomService _roomService;

        public ChatHub(PostService postService,
                       UserService userService,
                       RoomService roomService,
                       IDictionary<string, UserConnection> connections)
        {
            _postService = postService;
            _userService = userService;
            _roomService = roomService;

            _botUser = "Via Chat Bot";
            _connections = connections;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.OthersInGroup(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has left");
                SendUsersConnected(userConnection.Room);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            var room = await _roomService.GetRoomByNameAsync(userConnection.Room, RoomIncludes.All, false);
            if(room.IsNull())
            {
                await Clients.Caller.SendAsync("ReceiveError",  $"{userConnection.Room} does not exist, do you want to add it?");
            }
            else
            {
                var user = await _userService.GetUserByNameAsync(userConnection.User, UserIncludes.None, false);
                // if user was not found, create new user
                user ??= await _userService.CreateUserAsync(new() { Name = userConnection.User });

                await Groups.AddToGroupAsync(Context.ConnectionId, room.Name);

                _connections[Context.ConnectionId] = userConnection;

                await Clients.OthersInGroup(room.Name).SendAsync("ReceiveMessage", _botUser, $"{user.Name} has joined {room.Name}");

                if(room.Users.HasValues())
                {
                    // show only the last 100 messages
                    var posts = room.Users.SelectMany(x => x.Posts.OrderByDescending(p => p.CreateDate)).Take(100).ToList();
                    foreach (var post in posts)
                    {
                        await Clients.Caller.SendAsync("ReceiveMessage", post.User, post.Message);
                    }
                }

                await SendUsersConnected(room.Name);
            }
        }

        public async Task SendMessage(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                CreatePost model = new()
                {
                    Msg = message,
                    User = userConnection.User,
                    Room = userConnection.Room
                };
                var result = await _postService.CreatePostAsync(model);
                await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", userConnection.User, result.Message);
            }
        }

        public async Task GetLastMessages(string name, int limit)
        {
            var room = await _roomService.GetRoomByNameAsync(name, RoomIncludes.All, false);

            // show only the requested number of messages
            var posts = room.Users.SelectMany(x => x.Posts.OrderByDescending(p => p.CreateDate)).Take(limit).ToList();
            foreach (var post in posts)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", post.User, post.Message);
            }
        }

        public async Task AddRoom(UserConnection userConnection)
        {
            CreateRoom model = new()
            {
                Name = userConnection.Room
            };
            _ = await _roomService.CreateRoomAsync(model);
            // clear error
            await Clients.Caller.SendAsync("ReceiveError");
        }

        public Task SendUsersConnected(string room)
        {
            var users = _connections.Values
                .Where(c => c.Room == room)
                .Select(c => c.User);

            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }
    }
}
