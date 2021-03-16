using MageBattle.Core.Data;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Level
{
    public class ObstaclesPool : MonoBehaviour
    {
        [SerializeField] private ObstaclePrefabsContainer _obstaclePrefabsContainer;
        private Dictionary<string, Stack<Obstacle>> _obstacles = new Dictionary<string, Stack<Obstacle>>();
        private  Vector3 _idlePosition = new Vector3(1000, 1000, 1000);
        private int _defaultCount = 5;
        private float _localScale;

        public void Initialize(float localScale)
        {
            _localScale = localScale;
            CreateObstacles();
        }

        private void CreateObstacles()
        {
            foreach (var info in _obstaclePrefabsContainer.obstaclePrefabsInfo)
            {
                _obstacles.Add(info.obstacleId, new Stack<Obstacle>());
                CreateObstacleObjectById(info.obstacleId, _defaultCount);
            }
        }

        private void CreateObstacleObjectById(string id, int count)
        {
            var prefab = _obstaclePrefabsContainer.GetObstaclePrefabById(id);
            for (int i = 0; i < count; i++)
            {
                var obstacle = Instantiate(prefab, LevelBuilder.instance.obstaclesPoolHolderObj).GetComponent<Obstacle>();
                obstacle.Initialize(id);
                obstacle.transform.localScale = new Vector3(_localScale, obstacle.transform.localScale.y, _localScale);
                Push(obstacle);
            }
        }

        public void Push(Obstacle obstacle)
        {
            obstacle.transform.position = _idlePosition;
            _obstacles[obstacle.id].Push(obstacle);
        }

        public Obstacle PopById(string id)
        {
            Obstacle obstacle = null;
            if (_obstacles.ContainsKey(id))
            {
                if (_obstacles[id].Count == 0)
                {
                    CreateObstacleObjectById(id, 1);
                }
                obstacle = _obstacles[id].Pop();
            }
            return obstacle;
        }

        public bool IsObstacleDestroyableById(string id)
        {
            var prefab = _obstaclePrefabsContainer.GetObstaclePrefabById(id);
            return prefab.GetComponent<Obstacle>().destroyable;
        }

        public void Clear()
        {
            _obstacles.Clear();
        }
    }
}