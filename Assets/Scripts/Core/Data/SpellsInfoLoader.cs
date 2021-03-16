using MageBattle.Core.Units.Spells;
using MageBattle.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Data
{
    public static class SpellsInfoLoader
    {
        private static Dictionary<int, SpellInfo> _spellsInfo;
        private const string _dataPath = "Data/spells_info";

        public static IReadOnlyDictionary<int, SpellInfo> spellsInfo => _spellsInfo;

        public static void Load()
        {
            TextAsset textAsset = Resources.Load<TextAsset>(_dataPath);
            _spellsInfo = JsonSerializationHelper.DeserializeObject<Dictionary<int, SpellInfo>>(textAsset.text);
        }
    }
}