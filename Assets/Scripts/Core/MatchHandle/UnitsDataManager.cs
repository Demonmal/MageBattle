using MageBattle.Core.Units;
using MageBattle.Profile;
using MageBattle.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core
{
    public class UnitsDataManager : SingletonObject<UnitsDataManager>
    {
        private UserData _mineUserData;

        private Dictionary<string, UnitData> _units = new Dictionary<string, UnitData>();

        public UserData mineUserData => _mineUserData;
        public IReadOnlyDictionary<string, UnitData> units => _units;

        public System.Action<UnitData> onAddedData;
        public System.Action<UserData> onUserJoined;
        public System.Action<UserData> onUserLeft;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            AddMineUser();
        }

        private void AddMineUser()
        {
            var data = UserProfile.GetLocalData<UserData>();
            _mineUserData = data;
            AddUserData(data, true);
        }

        public void AddUserData(UserData data, bool isMine = false, bool isBot = false)
        {
            string userId = data.userId;
            if(!_units.ContainsKey(userId))
            {
                UnitData unitData = new UnitData()
                {
                    userId = data.userId,
                    spellsId = data.spellsId,
                    isMine = isMine,
                    isBot = isBot
                };
                _units.Add(userId, unitData);
                onAddedData?.Invoke(unitData);
            }
        }

        public UnitData GetDataFromUserID(string userId)
        {
            if (_units.ContainsKey(userId))
            {
                return _units[userId];
            }
            return null;
        }
    }
}