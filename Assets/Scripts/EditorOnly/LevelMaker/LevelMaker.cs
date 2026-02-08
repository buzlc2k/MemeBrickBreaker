using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BullBrukBruker
{
#if UNITY_EDITOR
    [Serializable]
    public class ItemTileKey
    {
        public float Value; // 0 to 1
        public Tile Tile;
    }

    public class LevelMaker : MonoBehaviour
    {
        [SerializeField] private int currentLevel;
        [SerializeField] private Tilemap blockMaptile;
        [SerializeField] private Tilemap itemMapTile;

        private ItemID GetItem(Vector3Int position)
        {
            if (itemMapTile.HasTile(position))
            {
                var id = ((Tile)itemMapTile.GetTile(position)).sprite.name;
                return (ItemID)Enum.Parse(typeof(ItemID), id);
            }

            return ItemID.None;
        }

        private bool GetCanCollide(string ID)
        {
            return ConfigsManager.Instance.BlockConfig.Records
                .FirstOrDefault(data => data.ID.ToString().Equals(ID)).CanCollide;
        }

        private int GetNumCollide(string ID)
        {
            return ConfigsManager.Instance.BlockConfig.Records
                .FirstOrDefault(data => data.ID.ToString().Equals(ID)).NumCollide;
        }

        private SavedCell CreateSavedTile(Tile tile, Vector3Int position)
        {
            var savedCell = new SavedCell
            {
                Tile = tile,
                Position = position,
                ItemInside = GetItem(position),
                CanCollide = GetCanCollide(tile.sprite.name),
                NumCollide = GetNumCollide(tile.sprite.name)
            };

            return savedCell;
        }

        private List<SavedCell> GetAllTiles()
        {
            var savedTiles = new List<SavedCell>();

            foreach (var pos in blockMaptile.cellBounds.allPositionsWithin)
            {
                if (!blockMaptile.HasTile(pos)) continue;

                var tile = blockMaptile.GetTile<Tile>(pos);
                if (tile == null)
                {
                    Debug.LogError($"Tile at position {pos} is null!");
                    continue;
                }

                savedTiles.Add(CreateSavedTile(tile, pos));
            }

            return savedTiles;
        }

        public void SaveLevel()
        {
            var levelConfig = ConfigsManager.Instance.LevelConfig;
            var existingRecord = levelConfig.GetRecord(currentLevel);

            if (existingRecord != null)
            {
                Debug.Log($"Overriding existing level {currentLevel}");
                levelConfig.RemoveRecord(existingRecord);
            }

            var newRecord = new LevelConfigRecord
            {
                Index = currentLevel,
                Cells = GetAllTiles()
            };

            levelConfig.AddRecord(newRecord);

            EditorUtility.SetDirty(levelConfig);
            AssetDatabase.SaveAssets();

            Debug.Log($"Level {currentLevel} saved successfully!");
        }

        private void LoadSavedCell(SavedCell cell)
        {
            blockMaptile.SetTile(cell.Position, cell.Tile);

            if (cell.ItemInside.Equals(ItemID.None)) return;

            var itemTile = Resources.Load<Tile>($"Art/TilePallet/Item/{cell.ItemInside}");
            itemMapTile.SetTile(cell.Position, itemTile);
        }

        public void LoadLevel()
        {
            var record = ConfigsManager.Instance.LevelConfig.GetRecord(currentLevel); ;

            if (record == null)
            {
                Debug.LogWarning($"Level {currentLevel} does not exist!");
                return;
            }

            ClearLevel();

            foreach (var cell in record.Cells)
                LoadSavedCell(cell);

            Debug.Log($"Level {currentLevel} loaded successfully!");
        }

        public void ClearLevel()
        {
            blockMaptile.ClearAllTiles();

            itemMapTile.ClearAllTiles();
        }

        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField] private int minStepsPerItemSpawn;
        [SerializeField] private int maxStepsPerItemSpawn;
        [SerializeField] private List<ItemTileKey> items;

        public void GenerateRandomItems()
        {
            var startIndex = blockMaptile.WorldToCell(startPoint.position);
            var endIndex = blockMaptile.WorldToCell(endPoint.position);

            var pathNodes = Pathfinding.FindRandomizedPath(startIndex, endIndex);
            items.Sort((a, b) => a.Value.CompareTo(b.Value));

            int currentPerItemSpawn = UnityEngine.Random.Range(minStepsPerItemSpawn, maxStepsPerItemSpawn);
            int currentStep = 0;

            foreach (var node in pathNodes)
            {
                if (currentStep > currentPerItemSpawn
                && blockMaptile.GetTile(node.Index) != null
                &&  GetCanCollide(blockMaptile.GetTile<Tile>(node.Index).sprite.name))
                {
                    currentStep = 0;
                    currentPerItemSpawn = UnityEngine.Random.Range(minStepsPerItemSpawn, maxStepsPerItemSpawn);

                    float itemVal = UnityEngine.Random.value;
                    foreach (var item in items)
                        if (itemVal <= item.Value)
                        {
                            itemMapTile.SetTile(node.Index, item.Tile);
                            break;
                        }    
                }

                currentStep++;
            }
        }
    }
#endif
}