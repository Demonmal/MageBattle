using MageBattle.Utility;
using Newtonsoft.Json;
using System.IO;

namespace MageBattle.Profile
{
    public abstract class LocalProfileJData : ProfileDataBase
    {
        [JsonIgnore]
        public string name;

        public virtual async void Save()
        {
            string jsonStr = await JsonSerializationHelper.SerializeObjectAsync(this, Formatting.Indented);
            string path = UserProfile.pathProfile + "/" + name + ".json";
            using (StreamWriter streamWritter = new StreamWriter(path))
            {
                streamWritter.WriteLine(jsonStr);
            }
        }

        public virtual void ResetAndSave()
        {
            OnGenerate();
            Save();
        }
    }
}