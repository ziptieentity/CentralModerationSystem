using CentralModerationSystem.Data_Classes;

namespace CentralModerationSystem.Data_Controllers
{
    [Serializable]
    public class MutesDatabaseItem : DatabaseItem
    {       
        public string Reason;
        public long MuteEndTimeEpoch;
        public long MuteStartTimeEpoch;
        public ulong MuteInitiator;
        public bool Active;
    }
}
