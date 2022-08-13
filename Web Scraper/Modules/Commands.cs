using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace Web_Scraper.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("next_launch")]
        public async Task next_launch()
        {
            Console.WriteLine(Context.Message.Content);

            LaunchScraper scraper = new LaunchScraper();
            await ReplyAsync(embed: scraper.PrintSchedule().Build());

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
                    .WithColor(Discord.Color.Green)
                    .WithCurrentTimestamp();
            await ReplyAsync(embed: embedBuilder.Build());
        }

        [Command("!next_closure")]
        public async Task next_closure()
        {
            await ReplyAsync("Coming soon!");
        }

            [Command("help")]
        public async Task help()
        {

            string helpMessage = ""
                + "**!next_launch** prints the next 10 upcoming launches \n"
                + "**!help** prints this message \n"
                + "**!sanity** checks whether the bot is alive\n"
                + "**!next_closure** checks whether a boca chica static fire is scheduled or active\n";


            var embedBuilder = new Discord.EmbedBuilder()
                    .WithTitle("SpaceX Bot Commands")
                    .WithDescription(helpMessage)
                    .WithColor(Discord.Color.Green)
                    .WithCurrentTimestamp();

            
            await ReplyAsync(embed: embedBuilder.Build());
        }
    }
}
