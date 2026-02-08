using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BullBrukBruker
{
    public class GridCell
    {
        public readonly static float Size = 0.25f;
        private readonly Vector3Int index;
        private readonly Vector3 pos;
        private readonly Vector3 orinPos;
        private readonly bool canBreak;
        private int durability;
        private readonly ItemID itemInside;
        public Dictionary<ObjectID, List<ObjectCollision>> Objects { get; private set; }

        public GridCell(Vector3Int index, bool canBreak, int durability, ItemID itemInside)
        {
            this.index = index;
            this.canBreak = canBreak;
            this.durability = durability;
            this.itemInside = itemInside;
            pos = GridManager.Instance.Tilemap.GetCellCenterWorld(index);
            orinPos = pos - new Vector3(Size, Size, 0);

            Objects = new();
        }

        public Vector3Int CellIndex()
            => index;

        public Vector3 CellPos()
            => pos;

        public bool CanBreak()
            => canBreak;

        public Vector3 GetFractInCell(Vector3 pos)
        {
            Vector3 offset = pos - orinPos;
            float fullSize = Size * 2f;
            return new Vector3(
                offset.x / fullSize,
                offset.y / fullSize,
                offset.z / fullSize
            );
        }

        public bool CanCollide()
            => durability > 0;

        public void TakeHit()
        {
            if (!canBreak)
                return;

            durability--;
            MemeManager.Instance.ChangeToHappy();

            if (durability == 0)
            {
                GridManager.Instance.RemoveCellInGrid(index);

                if (!itemInside.Equals(ItemID.None))
                    ItemSpawner.Instance.SpawnItem(itemInside, CellPos());
            }
        }

        public void RegisterObject(ObjectID id, ObjectCollision obj)
        {
            if (!Objects.ContainsKey(id))
                Objects.Add(id, new List<ObjectCollision>());

            Objects[id].Add(obj);
        }

        public void UnRegisterObject(ObjectID id, ObjectCollision obj)
        {
            if (!Objects.ContainsKey(id)) return;

            Objects[id].Remove(obj);
        }
    }

    public class GridManager : SingletonMono<GridManager>
    {
        private int remainCells = 0;
        private Dictionary<Vector3Int, GridCell> cellGrid;

        [field: SerializeField] public List<LevelBoundController> LevelBounds { get; private set; }
        [field: SerializeField] public LevelBoundController DeathZone { get; private set; }
        [field: SerializeField] public Tilemap Tilemap { get; private set; }

        public void InitGridCells(List<SavedCell> savedCells)
        {
            cellGrid ??= new();
            cellGrid.Clear();

            Tilemap.ClearAllTiles();

            remainCells = 0;

            FillNonEmpyCells(savedCells);
            FillEmptyCells();
            CreateGridBounds();
        }

        private void FillNonEmpyCells(List<SavedCell> savedCells)
        {
            foreach (var savedCell in savedCells)
            {
                var pos = savedCell.Position;
                cellGrid.Add(pos, new GridCell(pos, savedCell.CanCollide,savedCell.NumCollide, savedCell.ItemInside));
                Tilemap.SetTile(pos, savedCell.Tile);

                if (savedCell.CanCollide)
                    remainCells++;
            }
        }

        private void FillEmptyCells()
        {
            var screenWidth = ScreenManager.Instance.ScreenWidth;
            var screenHeight = ScreenManager.Instance.TopScreenHeight;

            var totalXBlocks = Mathf.FloorToInt(screenWidth / (GridCell.Size * 2)) + 1;
            var totalYBlocks = Mathf.FloorToInt(screenHeight / (GridCell.Size * 2)) + 1;

            for (int u = -totalXBlocks; u <= totalXBlocks; u++)
            {
                for (int v = -totalYBlocks; v <= totalYBlocks; v++)
                {
                    var index = new Vector3Int(u, v, 0);
                    if (!cellGrid.ContainsKey(index))
                    {
                        cellGrid.Add(index, new GridCell(index, false, 0, ItemID.None));
                    }
                }
            }
        }

        private void CreateGridBounds()
        {
            var horizontalBounds = LevelBounds.Where(b => b.ID.Equals(ObjectID.HorizontalLevelBound))
                                    .Prepend(DeathZone).ToList();

            var verticalBounds = LevelBounds.Where(b => !b.ID.Equals(ObjectID.HorizontalLevelBound)).ToList();

            for (int i = 0; i < horizontalBounds.Count; i++)
            {
                float posY = (i % 2 == 0) ? -ScreenManager.Instance.DownScreenHeight : ScreenManager.Instance.TopScreenHeight;
                horizontalBounds[i].transform.position = new Vector3(0, posY, 0);
                horizontalBounds[i].gameObject.SetActive(true);
                horizontalBounds[i].InitLevelBound();
            }

            for (int i = 0; i < verticalBounds.Count; i++)
            {
                int sign = (i % 2 == 0) ? -1 : 1;
                verticalBounds[i].transform.position = new Vector3(sign * ScreenManager.Instance.ScreenWidth, 0, 0);
                verticalBounds[i].gameObject.SetActive(true);
                verticalBounds[i].InitLevelBound();
            }
        }

        public bool TryGetCell(Vector3Int celPos, out GridCell cell)
        {
            return cellGrid.TryGetValue(celPos, out cell);
        }

        public bool TryGetCell(Vector3 position, out GridCell cell)
        {
            var celPos = Tilemap.WorldToCell(position);

            return cellGrid.TryGetValue(celPos, out cell);
        }

        public void RemoveCellInGrid(Vector3Int index)
        {
            Tilemap.SetTile(index, null);
            remainCells -= 1;

            if (remainCells <= 0)
                Observer.PostEvent(EventID.OutOfCells, null);
        }
    }
}