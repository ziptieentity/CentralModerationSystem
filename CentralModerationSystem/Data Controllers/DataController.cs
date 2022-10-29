using CentralModerationSystem.Data_Classes;
using Newtonsoft.Json;

namespace CentralModerationSystem.Data_Controllers
{
    public static class DataController
    {
        public static MutesDatabase MuteDatabase;
        public static string MutesDirectory = $"{Environment.ProcessPath}/../Data/Mutes.json";

        public async static Task<MutesDatabaseItem?> IsMuted(ulong Steam64ID)
        {
            await RefreshDatabases();
            foreach(MutesDatabaseItem item in MuteDatabase.Items.FindAll(x => x.Steam64ID == Steam64ID && x.Active == true))
            {
                if (MuteDatabase == null)
                    continue;
                if (item.MuteEndTimeEpoch < GetEpochTime().TotalSeconds)
                    continue;
                return item;
            }
            return null;
        }     
        public async static Task<List<MutesDatabaseItem>> GetMuteHistory(ulong Steam64ID)
        {
            await RefreshDatabases();
            return MuteDatabase.Items.FindAll(x => x.Steam64ID == Steam64ID);
        }
        public async static Task<MutesDatabaseItem> AddMute(ulong Steam64ID, long EndTimeEpoch, ulong InitiatorID, string Reason = "No Reason")
        {
            await RefreshDatabases();
            MutesDatabaseItem item = new MutesDatabaseItem
            {
                Steam64ID = Steam64ID,
                MuteEndTimeEpoch = EndTimeEpoch,
                MuteStartTimeEpoch = (long)GetEpochTime().TotalSeconds,
                Reason = Reason,
                MuteInitiator = InitiatorID,
                Active = true
            };
            MuteDatabase.Items.Add(item);
            string mutesDatabaseJSON = JsonConvert.SerializeObject(MuteDatabase, Formatting.Indented);
            await File.WriteAllTextAsync(MutesDirectory, mutesDatabaseJSON);
            return item;
        }
        public async static Task<bool> RemoveMute(ulong Steam64ID)
        {
            await RefreshDatabases();
            MutesDatabaseItem item = MuteDatabase.Items.FirstOrDefault(x => x.Steam64ID == Steam64ID && x.Active == true);
            if (item == null)
                return false;
            item.Active = false;
            string mutesDatabaseJSON = JsonConvert.SerializeObject(MuteDatabase, Formatting.Indented);
            await File.WriteAllTextAsync(MutesDirectory, mutesDatabaseJSON);
            return true;
        }
        public async static Task RefreshDatabases()
        {
            string JSONString = await File.ReadAllTextAsync(MutesDirectory);
            MutesDatabase mutesDatabase = JsonConvert.DeserializeObject<MutesDatabase>(JSONString);
            foreach(MutesDatabaseItem item in mutesDatabase.Items)
            {
                if (item.MuteEndTimeEpoch < GetEpochTime().TotalSeconds)
                    item.Active = false;
            }
            string mutesDatabaseJSON = JsonConvert.SerializeObject(MuteDatabase, Formatting.Indented);
            await File.WriteAllTextAsync(MutesDirectory, mutesDatabaseJSON);
            MuteDatabase = mutesDatabase;
            return;
        }
        public static TimeSpan GetEpochTime()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return t;
        }
    }
}
