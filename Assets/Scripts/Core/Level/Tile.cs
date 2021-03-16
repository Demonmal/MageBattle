using MageBattle.Core.Units;
using UnityEngine;

namespace MageBattle.Core.Level
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Material _spawnZoneMaterial;

        private TileType _type;
        private TileType _defaultTileType = TileType.Empty;

        public TileType type => _type;
        public int id { get; private set; }
        public int x { get; private set; }
        public int z { get; private set; }

        public void Initialize (int x, int z)
        {
            this.x = x;
            this.z = z;
            MarkAsEmpty();
        }

        public void SetId(int id)
        {
            this.id = id;
        }

        public bool IsBlocked()
        {
            return _type == TileType.With_Obstacle || _type == TileType.With_Player;
        }

        public void SetType(TileType type)
        {
            switch (type)
            {
                case TileType.With_Obstacle:
                case TileType.With_Player:
                    _type = type;
                    break;
            }
        }

        public void MarkAsEmpty()
        {
            _type = _defaultTileType;
        }

        public void MarkAsSpawnZone()
        {
            _defaultTileType = TileType.SpawnPoint;
            if(_type == TileType.Empty)
            {
                _type = TileType.SpawnPoint;
            }
            gameObject.GetComponent<MeshRenderer>().material = _spawnZoneMaterial;
        }

        public bool TryGetObstacleOnTile(out Obstacle obstacle)
        {
            obstacle = null;
            if(type == TileType.With_Obstacle)
            {
                LevelBuilder.instance.obstaclesManager.TryGetObstacleOnPlace(x, z, out obstacle);
            }
            return type == TileType.With_Obstacle;
        }

        public bool TryGetUnitOnTile(out Unit unit)
        {
            unit = null;
            if (type == TileType.With_Player)
            {
                var units = UnitsManager.instance.aliveUnits;
                foreach (var aliveUnit in units)
                {
                    if (aliveUnit.currentTile.id == id)
                    {
                        unit = aliveUnit;
                        break;
                    }
                }
            }
            return type == TileType.With_Player;
        }

        public override string ToString()
        {
            return $"Tile{x}:{z}";
        }
    }
}