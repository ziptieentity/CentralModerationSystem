using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CentralModerationSystem.Data_Classes;
using CentralModerationSystem.Data_Controllers;
using Newtonsoft.Json;

namespace CentralModerationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet, Route("/api/moderation/mutes/ismuted")]
        public async Task<string> IsMuted(ulong Steam64ID)
        {
            MutesDatabaseItem item = await DataController.IsMuted(Steam64ID);
            if(item == null)
                return JsonConvert.SerializeObject(new Error() { Message = "The player is not muted." });
            return JsonConvert.SerializeObject(item);
        }
        [HttpGet, Route("/api/moderation/mutes/getmutehistory")]
        public async Task<string> GetMuteHistory(ulong Steam64ID)
        {          
            return JsonConvert.SerializeObject(await DataController.GetMuteHistory(Steam64ID));
        }
        [HttpPost, Route("/api/moderation/mutes/addmute")]
        public async Task<string> AddMute(ulong Steam64ID, ulong StaffID, int EndTimeEpoch, string Reason = "No Reason")
        {
            await DataController.RefreshDatabases();
            if (DataController.MuteDatabase.Items.FirstOrDefault(x => x.Steam64ID == Steam64ID && x.MuteEndTimeEpoch > DataController.GetEpochTime().TotalSeconds && x.Active == true) != null)
                return JsonConvert.SerializeObject(new Error() { Message = "Player is already muted." });
            return JsonConvert.SerializeObject(await DataController.AddMute(Steam64ID, EndTimeEpoch, StaffID, Reason));
        }
        [HttpPost, Route("/api/moderation/mutes/removemute")]
        public async Task<string> AddMute(ulong Steam64ID)
        {
            bool Success = await DataController.RemoveMute(Steam64ID);
            if (Success)
                return JsonConvert.SerializeObject(new Error() { Message = "Success" });
            return JsonConvert.SerializeObject(new Error() { Message = "The specified player is not muted." });
        }
    }
}
