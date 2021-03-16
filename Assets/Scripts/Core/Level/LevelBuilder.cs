using MageBattle.Core.Data;
using MageBattle.Core.MatchHandle;
using MageBattle.Core.Spawn;
using MageBattle.Core.Units;
using MageBattle.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Level
{
    [RequireComponent(typeof(ObstaclesManager), typeof(ObstaclesPool))]
    public class LevelBuilder : SingletonObject<LevelBuilder>
    {
        [SerializeField] private int _sizeX;
        [SerializeField] private int _sizeZ;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private float _scaleMultiplier;
        [SerializeField] private int _obstaclesNumber;
        [SerializeField] private Gem _gemPrefab;
        private GemPositionGiver _gemPositionGiver;
        private Tile[,] _tiles;
        private Gem _gem;
        private PathHelper _pathHelper;

        private const string _gemHolderObjName = "Gems";
        private const string _tilesHolderObjName = "Tiles";
        private const string _obstaclesHolderObjName = "Obstacles";
        private const string _obstaclesPoolObjName = "ObstaclesPool";

        public Transform obstaclesHolderObj { get; private set; }
        public Transform obstaclesPoolHolderObj { get; private set; }
        public ObstaclesManager obstaclesManager { get; private set; }
        public ObstaclesPool obstaclesPool { get; private set; }
        public PathHelper pathHelper => _pathHelper;
        public int sizeX => _sizeX;
        public int sizeZ => _sizeZ;
        public Gem gem => _gem;

        private void Awake()
        {
            _instance = this;
        }

        public void GenerateLevel()
        {
            obstaclesManager = GetComponent<ObstaclesManager>();
            obstaclesManager.Initialize(this);

            obstaclesPool = GetComponent<ObstaclesPool>();

            _tiles = new Tile[_sizeX, _sizeZ];

            FindAndDeleteHoldersIfItExists();

            var tilesHolderObj = new GameObject(_tilesHolderObjName);
            tilesHolderObj.transform.SetParent(transform);

            var gemsHolderObj = new GameObject(_gemHolderObjName);
            gemsHolderObj.transform.SetParent(transform);

            obstaclesHolderObj = new GameObject(_obstaclesHolderObjName).transform;
            obstaclesHolderObj.SetParent(transform);

            obstaclesPoolHolderObj = new GameObject(_obstaclesPoolObjName).transform;
            obstaclesPoolHolderObj.SetParent(transform);

            obstaclesPool.Initialize(_scaleMultiplier);
            CreateGround(tilesHolderObj.transform);
            CreateObstacles();
            UnitSpawner.instance.AssignSpawnPoints();
            CreateGem(gemsHolderObj.transform);
            _pathHelper = new PathHelper(this);
        }

        private void FindAndDeleteHoldersIfItExists()
        {
#if UNITY_EDITOR
            obstaclesPool.Clear();
            if (transform.Find(_gemHolderObjName))
            {
                DestroyImmediate(transform.Find(_gemHolderObjName).gameObject);
            }
            if (transform.Find(_tilesHolderObjName))
            {
                DestroyImmediate(transform.Find(_tilesHolderObjName).gameObject);
            }
            if (transform.Find(_obstaclesHolderObjName))
            {
                DestroyImmediate(transform.Find(_obstaclesHolderObjName).gameObject);
            }
            if (transform.Find(_obstaclesPoolObjName))
            {
                DestroyImmediate(transform.Find(_obstaclesPoolObjName).gameObject);
            }
#endif
        }

        private void CreateGem(Transform holder)
        {
            _gem = Instantiate(_gemPrefab, holder);
            _gem.onGemGathered += OnGemGathered;
            _gemPositionGiver = new GemPositionGiver(this);
            RelocateGem();
        }

        private void OnGemGathered(Unit unit)
        {
            RelocateGem();
        }

        private void RelocateGem()
        {
            Tile tileToSetGem = _gemPositionGiver.GetNewGemTile();
            _gem.Relocate(tileToSetGem);
        }

        private void CreateGround(Transform holder)
        {
            int id = 1; 
            for (int x = 0; x < _sizeX; x++)
            {
                for (int z = 0; z < _sizeZ; z++)
                {
                    Vector3 position = new Vector3(-_sizeX / 2 + x, 0, -_sizeZ / 2 + z);
                    Tile tile = Instantiate(_tilePrefab, holder.transform).GetComponent<Tile>();
                    tile.Initialize(x, z);
                    tile.name = $"Tile_{x}_{z}";
                    tile.transform.position = position;
                    tile.transform.localScale = Vector3.one * _scaleMultiplier;
                    _tiles[x, z] = tile;
                    _tiles[x, z].SetId(id++);
                }
            }
        }

        public List<Tile> GetTilesWithoutObstacles()
        {
            var result = new List<Tile>();
            foreach (var tile in _tiles)
            {
                if(!tile.IsBlocked())
                {
                    result.Add(tile);
                }
            }
            return result;
        }
        
        public bool CanMoveFromTile(Tile tile)
        {
            var tilesNearby = GetTilesFromFourSights(tile);
            foreach (var nearbyTile in tilesNearby)
            {
                if (!nearbyTile.IsBlocked())
                    return true;
            }
            return false;
        }

        public bool CanMoveFromTile(Tile tile, out int movableTilesCount)
        {
            movableTilesCount = 0;
            bool result = false;
            var tilesNearby = GetTilesFromFourSights(tile);
            foreach (var nearbyTile in tilesNearby)
            {
                if (!nearbyTile.IsBlocked())
                {
                    result = true;
                    movableTilesCount++;
                }
            }
            return result;
        }

        public List<Tile> GetTilesFromFourSights(Tile tile)
        {
            List<Tile> result = new List<Tile>();
            if (tile.x < _sizeX - 1)
            {
                result.Add(_tiles[tile.x + 1, tile.z]);
            }
            if (tile.z < _sizeZ - 1)
            {
                result.Add(_tiles[tile.x, tile.z + 1]);
            }
            if (tile.x > 0)
            {
                result.Add(_tiles[tile.x - 1, tile.z]);
            }
            if (tile.z > 0)
            {
                result.Add(_tiles[tile.x, tile.z - 1]);
            }
            return result;
        }

        public List<Tile> GetAllTilesNearby(Tile tile)
        {
            List<Tile> result = new List<Tile>();
            result.AddRange(GetTilesFromFourSights(tile));
            if (tile.x < _sizeX - 1 && tile.z < _sizeZ - 1)
            {
                result.Add(_tiles[tile.x + 1, tile.z + 1]);
            }
            if (tile.x < _sizeX - 1 && tile.z > 0)
            {
                result.Add(_tiles[tile.x + 1, tile.z - 1]);
            }
            if (tile.x > 0 && tile.z < _sizeZ - 1)
            {
                result.Add(_tiles[tile.x - 1, tile.z + 1]);
            }
            if (tile.x > 0 && tile.z > 0)
            {
                result.Add(_tiles[tile.x - 1, tile.z - 1]);
            }
            return result;
        }

        public List<Tile> GetAllTilesInRanges((int, int) xRange, (int,int) zRange)
        {
            var result = new List<Tile>();
            for (int x = xRange.Item1; x <= xRange.Item2; x++)
            {
                for (int z = zRange.Item1; z <= zRange.Item2; z++)
                {
                    result.Add(_tiles[x,z]);
                }
            }
            return result;
        }

        public List<Tile> GetAllReachableTiles(Tile currentTile, int maxDistanceToMove)
        {
            var result = new List<Tile>();
            var xRange = (Mathf.Clamp(currentTile.x - maxDistanceToMove, 0, _sizeX - 1), Mathf.Clamp(currentTile.x + maxDistanceToMove, 0, _sizeX - 1));
            var zRange = (Mathf.Clamp(currentTile.z - maxDistanceToMove, 0, _sizeZ - 1), Mathf.Clamp(currentTile.z + maxDistanceToMove, 0, _sizeZ - 1));
            var tilesToCheck = GetAllTilesInRanges(xRange, zRange);
            foreach (var tile in tilesToCheck)
            {
                if (tile.x == currentTile.x && tile.z == currentTile.z)
                    continue;
                if (_pathHelper.GetDistanceBetweenTiles(currentTile, tile) <= maxDistanceToMove)
                {
                    result.Add(tile);
                }
            }
            return result;
        }

        public Tile GetTileOnPosition(int x, int z)
        {
            return _tiles[x, z];
        }

        public Tile[,] GetAllTiles()
        {
            return _tiles;
        }

        private void CreateObstacles()
        {
            int undestroyableCount = _obstaclesNumber / 2;
            int destroyableCount = _obstaclesNumber - undestroyableCount;
            obstaclesManager.CreateDefaultLevelObstacles(destroyableCount, undestroyableCount);
        }

        private void OnDestroy()
        {
            _gem.onGemGathered -= OnGemGathered;
        }
    }
}