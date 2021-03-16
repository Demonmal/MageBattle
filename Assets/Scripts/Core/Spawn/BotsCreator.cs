using MageBattle.Core.MatchHandle;
using MageBattle.Core.Units;
using MageBattle.Core.Units.Bots;
using MageBattle.Profile;
using MageBattle.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Spawn
{
    public class BotsCreator : SingletonObject<BotsCreator>
    {
        private Dictionary<string, BotData> _dataById = new Dictionary<string, BotData>();
        private const string _dataPath = "Data/BotUserDatas/bots_data";
        private List<BotData> _botsData;

        public IReadOnlyDictionary<string, BotData> dataById => _dataById;

        private void Start()
        {
            GameHandler.instance.timeComponent.onMatchStarted += OnMatchStarted;
        }

        private void OnMatchStarted()
        {
            GameHandler.instance.timeComponent.onMatchStarted -= OnMatchStarted;
            CreateRestBots();
        }

        public void CreateRestBots()
        {
            Debug.Log($"CreateRestBots");
            int unitsCount = UnitsManager.instance.unitsById.Count;
            int botsToCreate = GameHandler.instance.maxPlayersCount - unitsCount;
            if(botsToCreate > 0)
            {
                LoadData();
                var shuffledDatas = ShuffleArray.Shuffle(_botsData);
                for (int i = 0; i < botsToCreate; i++)
                {
                    BotData data = shuffledDatas[i];
                    var userData = new UserData()
                    {
                        name = $"bot_{i}",
                        spellsId = data.spellsId,
                        userId = $"bot_{i}_id"
                    };
                    Debug.Log($"Add bot data {userData.userId}");
                    _dataById.Add(userData.userId, data);
                    CreateBot(userData);
                }
            }
        }

        private void CreateBot(UserData data)
        {
            UnitsDataManager.instance.AddUserData(data, false, true);
        }

        private void LoadData()
        {
            string jData = Resources.Load<TextAsset>(_dataPath).text;
            _botsData = JsonSerializationHelper.DeserializeObject<List<BotData>>(jData);
        }
    }
}