using Newtonsoft.Json;
using System.Collections.Generic;

namespace MageBattle.Profile
{
    public class UserData : LocalProfileJData
    {
        [JsonIgnore]
        public const string NAME = "user_local_data";

        public string userId;
        public List<int> spellsId;

        public override void OnGenerate()
        {
            userId = "000000";
            spellsId = new List<int>() {1, 2, 3, 4};
        }
    }
}