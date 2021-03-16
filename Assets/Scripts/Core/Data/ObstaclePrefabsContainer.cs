using MageBattle.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace MageBattle.Core.Data
{
    [CreateAssetMenu(menuName = "Data/Level/ObstaclePrefabsContainer")]
    public class ObstaclePrefabsContainer : ScriptableObject
    {
        public List<ObstaclePrefabInfo> obstaclePrefabsInfo;

        public GameObject GetObstaclePrefabById(string id)
        {
            foreach (var info in obstaclePrefabsInfo)
            {
                if (info.obstacleId == id)
                    return info.prefab;
            }
            DebugUtility.LogError($"Obstacle prefab with id <b>{id}</b> wasn't found");
            return null;
        }
    }

    [Serializable]
    public class ObstaclePrefabInfo
    {
        public string obstacleId;
        public GameObject prefab;
    }
}