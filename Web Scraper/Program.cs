using System;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Web_Scraper
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        public static LaunchScraper _launchManager;
        public static ClosureManager _closureManager;

        public async Task RunBotAsync() {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            _launchManager = new LaunchScraper();
            _closureManager = new ClosureManager();


            string token = "<include token here>";

            _client.Log += _client_Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync() 
        {
            _client.MessageReceived += HandleCommandAsync;
            
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg) 
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            //Prevent bot from talking to itself
            if (message.Author.IsBot) return;

            Console.WriteLine("Channel name: " + context.Channel.Name);

            if ((!context.Channel.Name.Equals("andrew-runs-commands-on-skyblock")) && (!context.Channel.Name.Equals("bot-commands"))) 
            {
                Console.WriteLine("Wrong channel! Channel name: " + context.Channel.Name);
                return;
            }

            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            }

        }
    }
}


