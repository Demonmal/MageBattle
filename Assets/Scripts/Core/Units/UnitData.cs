using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Units
{
    public class UnitData : MonoBehaviour
    {
        public string userId;
        public List<int> spellsId;
        public bool isBot;
        public bool isMine;
    }
}