using MageBattle.Core.Enums;
using MageBattle.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Data
{
    public static class DamageValuesContainer
    {
        private static Dictionary<DamageSource, float> _damageBySource;
        private const string _dataPath = "Data/damage_info";

        public static IReadOnlyDictionary<DamageSource, float> damageBySource => _damageBySource;

        public static void Load()
        {
            TextAsset textAsset = Resources.Load<TextAsset>(_dataPath);
            _damageBySource = JsonSerializationHelper.DeserializeObject<Dictionary<DamageSource, float>>(textAsset.text);
        }
    }
}