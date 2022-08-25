using Site___.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Tweetinvi.Models;
using System.IO;
using Tweetinvi.Parameters;
using System.Net;
using Discord;

namespace Site___.Modules.Events
{
    internal class TweetInQuotes
    {
        public TweetInQuotes()
        {
            Globals.Client.MessageReceived += onMessageReceived;
        }

        private async Task onMessageReceived(SocketMessage msg)
        {
            if (msg.Channel.Id != 797827770301677588) return;

            if(msg.Attachments.Count > 0)
            {
                var Http = new WebClient();
                List<IMedia> Media = new List<IMedia>();
                foreach (var att in msg.Attachments)
                {
                    string ext = Path.GetExtension(att.Url);
                    if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                        Media.Add(await Globals.Twitter.Upload.UploadTweetImageAsync(Http.DownloadData(att.Url)));
                    else if (ext == ".mp4" || ext == ".gif")
                        Media.Add(await Globals.Twitter.Upload.UploadTweetVideoAsync(Http.DownloadData(att.Url)));
                }
                string fixedContent = msg.Content;
                if (fixedContent.Length > 280)
                    fixedContent = fixedContent.Substring(0, 280);

                var Result = await Globals.Twitter.Tweets.PublishTweetAsync(new PublishTweetParameters(fixedContent)
                {
                    Medias = Media
                });

                Emoji twitter = Emoji.Parse("🐦");
                await msg.AddReactionAsync(twitter);
            }
        }
    }
}
