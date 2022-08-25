using Site___.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Net;
using Tweetinvi.Models;
using System.IO;
using Tweetinvi.Parameters;
using Discord;
using Serilog;

namespace Site___.Modules.Events
{
    internal class randomTweet
    {
        public randomTweet()
        {
            Globals.Client.MessageReceived += onMessageReceived;
        }

        Random rng = new Random();
        private async Task onMessageReceived(SocketMessage msg)
        {
            if(rng.Next(1,3000) == 5)
            {
                try
                {
                    var Http = new WebClient();
                    List<IMedia> Media = new List<IMedia>();
                    foreach (var att in msg.Attachments)
                    {
                        string ext = Path.GetExtension(att.Url);
                        if (ext == ".png" || ext==".jpg"||ext==".jpeg")
                            Media.Add(await Globals.Twitter.Upload.UploadTweetImageAsync(Http.DownloadData(att.Url)));
                        else if (ext ==".mp4"||ext==".gif")
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
                catch(Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }
    }
}
