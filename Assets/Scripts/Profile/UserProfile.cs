using MageBattle.Utility;
using System.Collections.Generic;

namespace MageBattle.Profile
{
    public static class UserProfile
    {
        private static string _pathProfile;
        private static Dictionary<System.Type, LocalProfileJData> _localProfileData = new Dictionary<System.Type, LocalProfileJData>();

        public static string pathProfile => _pathProfile;

        public static void Initialize()
        {
            InitPathProfile();
            InitLocalDatas();
        }

        private static void InitPathProfile()
        {
            _pathProfile = GetPathProfile();

            if (!System.IO.Directory.Exists(_pathProfile))
                System.IO.Directory.CreateDirectory(_pathProfile);
        }

        static string GetPathProfile()
        {
#if UNITY_EDITOR
            string foldername = @"/Mage_Battle_Editor";
#else  
            string foldername = @"/Mage_Battle";
#endif
            string folderPath = @"C:/ProgramData/Mage_Battle";
            string path = folderPath + foldername;

            return path;
        }

        private static void InitLocalDatas()
        {
            InitLocalData<UserData>(UserData.NAME);
        }

        private static void InitLocalData<T>(string name) where T : LocalProfileJData, new()
        {
            T profileData;
            string pathFile = pathProfile + "/" + name + ".json";
            if (System.IO.File.Exists(pathFile))
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(pathFile);
                profileData = JsonSerializationHelper.DeserializeObject<T>(reader.ReadToEnd());
                profileData.name = name;
                reader.Close();
            }
            else
            {
                profileData = new T();
                profileData.name = name;
                profileData.OnGenerate();
                profileData.Save();
            }

            _localProfileData[profileData.GetType()] = profileData;
        }

        public static T GetLocalData<T>() where T : LocalProfileJData
        {
            if (_localProfileData.ContainsKey(typeof(T)))
                return (T)_localProfileData[typeof(T)];
            return null;
        }

        public static void ResetAllLocalDatas()
        {
            foreach (var data in _localProfileData)
            {
                string pathFile = pathProfile + data.Value.name + ".json";
                if (System.IO.File.Exists(pathFile))
                    data.Value.ResetAndSave();
            }
        }
    }
}