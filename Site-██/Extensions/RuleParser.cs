using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Extensions
{
    public static class RuleParser
    {
        public static string ParseRules(this string Input)
        {
            Input = Input.Replace("{A1}", "Respect all users of the server, regardless of how much you like them or not. Treat others the way you want to be treated.");
            Input = Input.Replace("{A2}", "Do not send a lot of small messages or images right after each other, do not disrupt the chat by spamming.");
            Input = Input.Replace("{A3}", "Do not send any content that can be deemed NSFW, this includes cropped images of said material.");
            Input = Input.Replace("{A4}", "You are not allowed to advertise anywhere on this server unless a channel is meant for it. This also includes advertising in server members' direct messages.");
            Input = Input.Replace("{A5}", "If a staff member asks you to stop something, then stop, or more severe action may be taken against you.");
            Input = Input.Replace("{A6}", "Insults, bigotry of any kind (ex: homophobia, transphobia, racism, sexism), acting like a jerk will result in severe punishment.");
            Input = Input.Replace("{A7}", "Do not bring topics like politics and religion into the server.");
            Input = Input.Replace("{A7-A}", "Symbols like swastikas and communist ones (hammer & sickle) are not allowed, and will get you punished, this includes memes that support them");
            Input = Input.Replace("{A8}", "The use of alternative accounts is allowed as long as it's not used to evade a punishment (ex: mute, ban) or it may result in a more severe punishment.");
            Input = Input.Replace("{A9}", "Impersonation of any staff or server member is not allowed.");
            Input = Input.Replace("{A10}", "This is because we are not able to moderate any other languages than English, however, common words as Konichiwa and Déjà vu & similar, are allowed.");

            Input = Input.Replace("{B1}", "please do not use any inappropriate usernames/avatars.");
            Input = Input.Replace("{B2}", "Don't ping several users without their prior permission/request");
            Input = Input.Replace("{B3}", "You must follow the Discord Terms of Service while in this server.");
            Input = Input.Replace("{B4}", "The server owner reserves the permissions to ban any users for any given reason, even if its not against the rules, and is not required to provide a reason on why the ban was issued.");
            Input = Input.Replace("{B5}", "Joking about insensitive topics such as bombings, school shootings, political propaganda and other topics will result in a warn and a mute. This discord is meant to be an escape from the outside world so we don’t need politics, drama and insensitive jokes here..");
            Input = Input.Replace("{B6}", "Please do not ping high ranking staff or Rai for trivial reasons, this will result in a warn and a mute. If it is important please make a ticket or follow chain of command. Chain of command basically means go through lower staff before going to higher staff members.");
            Input = Input.Replace("{B7}", "The usage of bot commands in any other channel than <#709012714160390235> can result in a warning..");


            Input = Input.Replace("{C1}", "Use common sense when posting messages, if you have a feeling that it shouldn't be posted even though it isn't stated so in the rules, then you probably shouldn't");
            Input = Input.Replace("{C2}", "We understand that mental and physical health is important, but please do not talk about sensitive subjects like suicide, self-harm, and similar. Otherwise you may be muted. Seek professional help if needed.");
            Input = Input.Replace("{C3}", "if you have the permission to upload emojis, do not upload stickers, they are under the same permission as emojis, but without staff permission, uploading stickers will get you punished. If you have the permission for emojis, do not delete any of them, unless allowed by staff");
            Input = Input.Replace("{C3-A}", "Emojis that are low quality (white background, low resolution(unless that is a part of the joke in it)) will be removed.");
            Input = Input.Replace("{C4}", "This is a guild for everyone, not just you, if you tell someone to leave or harass them when they join, you will be warned & muted.");

            return Input;
        }
    }
}
