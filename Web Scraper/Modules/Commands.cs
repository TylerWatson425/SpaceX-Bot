using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;

namespace Web_Scraper.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("next_launch")]
        [Alias("nl", "nl star_link", "nl star_ship")]
        public async Task next_launch()
        {
            Console.WriteLine(Context.Message.Content);
            LaunchScraper scraper = new LaunchScraper();

            string filterOption = "";

            if ((Context.Message.Content != "!next_launch") && (Context.Message.Content != "!nl")) 
            {
                Console.WriteLine("hi!");
                filterOption = Context.Message.Content.Split(' ')[1];
                await ReplyAsync(embed: scraper.PrintSchedule(filterOption).Build());

            } else
            {
                await ReplyAsync(embed: scraper.PrintSchedule(filterOption).Build());
            }
        }

        [Command("sanity")]
        public async Task sanity()
        {
            await ReplyAsync("Hello! I'm alive. :)");
        }

        [Command("test")]
        public async Task test()
        {
            var embedBuilder = new Discord.EmbedBuilder()
                    .WithTitle("title")
                    .WithDescription("description")
                    .WithColor(Discord.Color.DarkOrange)
                    .WithCurrentTimestamp();

            await ReplyAsync(embed: embedBuilder.Build());
        }

        [Command("next_closure")]
        [Alias("nc")]
        public async Task next_closure()
        {
            ClosureManager closureManager = new ClosureManager();

            Thread.Sleep(2000);

            await ReplyAsync(embed: closureManager.GenerateDiscordReport().Build());
        }

        [Command("help")]
        public async Task help()
        {

            string helpMessage = ""
                + "**!next_launch** prints the next 10 upcoming launches \n"
                + "**!nl <star_link> or <star_ship>** the secondary parameters allows you to filter by star link or star ship launches only \n"
                + "**!help** prints this message \n"
                + "**!sanity** checks whether the bot is alive\n"
                + "**!next_closure** checks whether a boca chica static fire is scheduled or active\n";


            var embedBuilder = new Discord.EmbedBuilder()
                    .WithTitle("SpaceX Bot Commands")
                    .WithDescription(helpMessage)
                    .WithColor(Discord.Color.DarkBlue)
                    .WithCurrentTimestamp();

            
            await ReplyAsync(embed: embedBuilder.Build());
        }
    }
}
