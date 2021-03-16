using System;
using UnityEngine;

namespace MageBattle.Core.Level
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] private bool _destroyable;
        private Tile _tile;
        private string _id;
        private LevelBuilder _levelBuilder;
        public event Action onDestroy;

        private const float _offsetY = 0.25f;

        public bool destroyable => _destroyable;
        public Tile tile => _tile;
        public string id => _id;

        public void Initialize(string id)
        {
            _id = id;
            _levelBuilder = LevelBuilder.instance;
        }

        public void Spawn(Tile tile)
        {
            _tile = tile;
            _tile.SetType(TileType.With_Obstacle);
            transform.position = new Vector3(tile.transform.position.x, _offsetY, tile.transform.position.z);
        }

        public void Destroy()
        {
            if (!_destroyable)
                return;
            _levelBuilder.obstaclesManager.OnDestroyedObstacle(this);
            _tile.MarkAsEmpty();
            _tile = null;
            _levelBuilder.obstaclesPool.Push(this);
        }
    }
}