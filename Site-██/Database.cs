using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___
{
    public static class Database
    {
        // I'm too lazy to use an actual proper db (mongo,mysql,etc) so directories w/text files it is.
        // Might make a better solution later, but not really required since this isnt gonna be massive

        public static object Read(ulong ID, string Key)
        {
            if (!Directory.Exists($"Database/Users/{ID}"))
                Directory.CreateDirectory($"Database/Users/{ID}");

            if(File.Exists($"Database/Users/{ID}/{Key}"))
                return File.ReadAllText($"Database/Users/{ID}/{Key}");
            return null;
        }
        public static void Write(ulong ID, string Key, object Value)
        {
            if (!Directory.Exists($"Database/Users/{ID}"))
                Directory.CreateDirectory($"Database/Users/{ID}");

            File.WriteAllText($"Database/Users/{ID}", Value.ToString());
        }

        public static void AddWarning(ulong ID, string warnId, IUser Caller, string Reason)
        {
            if (!Directory.Exists($"Database/Users/{ID}"))
                Directory.CreateDirectory($"Database/Users/{ID}");
            if (!Directory.Exists($"Database/Users/{ID}/Warnings"))
                Directory.CreateDirectory($"Database/Users/{ID}/Warnings");

            IWarning Warn = new IWarning();
            Warn.Caller = Caller.Id;
            Warn.Reason = Reason;
            Warn.Expires = DateTimeOffset.Now.AddDays(12).ToUnixTimeSeconds();
            Warn.ID = warnId;

            string Serialized = JsonConvert.SerializeObject(Warn);
            File.WriteAllText($"Database/Users/{ID}/Warnings/{warnId}", Serialized);
        }
        public static IWarning[] Warnings(ulong ID)
        {
            if (!Directory.Exists($"Database/Users/{ID}"))
                Directory.CreateDirectory($"Database/Users/{ID}");
            if (!Directory.Exists($"Database/Users/{ID}/Warnings"))
                Directory.CreateDirectory($"Database/Users/{ID}/Warnings");

            List<IWarning> Warnings = new List<IWarning>();
            foreach(var Warning in Directory.GetFiles($"Database/Users/{ID}/Warnings"))
                Warnings.Add(JsonConvert.DeserializeObject<IWarning>(File.ReadAllText(Warning)));

            return Warnings.ToArray();
        }
        public static bool DeleteWarning(ulong ID, string warnID)
        {
            try
            {
                File.Delete($"Database/Users/{ID}/Warnings/{warnID}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Extensions to user
        public static object Read(this IUser User, string Key) => Read(User.Id, Key);
        public static void Write(this IUser User, string Key, object Value) => Write(User.Id, Key, Value);
        public static void AddWarning(this IUser User, string warnId, IUser Caller, string Reason) => AddWarning(User.Id, warnId, Caller, Reason);
        public static IWarning[] Warnings(this IUser User) => Warnings(User.Id);
        public static bool DeleteWarning(this IUser User, string warnID) => DeleteWarning(User.Id, warnID);
    }
}
