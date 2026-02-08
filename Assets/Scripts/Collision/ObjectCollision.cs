using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BullBrukBruker
{
    public abstract class ObjectCollision : MonoBehaviour
    {
        protected List<GridCell> currentCells;
        protected List<GridCell> cellsNeedRemoved;
        protected Dictionary<ObjectID, Func<ObjectCollision, bool>> collisionHandles;

        // Dependencies
        public ObjectCollisionConfigRecord Config { get; protected set; }

        protected virtual void Awake()
        {
            SetObjectCollisionConfigRecord();
            InitializeCollisionHandles();

            currentCells = new List<GridCell>();
            cellsNeedRemoved = new List<GridCell>();
        }

        protected virtual void OnDisable()
        {
            CoroutineUtils.StartSafeCourotine(GridManager.Instance, UnregisterAllCells());
        }

        protected abstract void SetObjectCollisionConfigRecord();

        protected abstract void InitializeCollisionHandles();

        public virtual void CalculateCellGroup()
        {
            CollectionUtils.MoveAllTo(currentCells, cellsNeedRemoved);

            GridManager gridManager = GridManager.Instance;

            if (!gridManager.TryGetCell(transform.parent.position, out GridCell orinCell))
                return;

            ProcessCell(orinCell);
            
            var totalXBlocks = Mathf.FloorToInt(Mathf.Abs(Config.Width - GridCell.Size) / (GridCell.Size * 2));
            var totalYBlocks = Mathf.FloorToInt(Mathf.Abs(Config.Height - GridCell.Size) / (GridCell.Size * 2));

            var originIndex = orinCell.CellIndex();
            for (int u = -totalXBlocks; u <= totalXBlocks; u++)
            {
                for (int v = -totalYBlocks; v <= totalYBlocks; v++)
                {
                    var newIndex = originIndex + new Vector3Int(u, v, 0);
                    if (gridManager.TryGetCell(newIndex, out GridCell cell))
                        ProcessCell(cell);
                }
            }

            UnregisterRemovedCells();
        }

        protected virtual void ProcessCell(GridCell cell)
        {
            if (cellsNeedRemoved.Contains(cell))
                cellsNeedRemoved.Remove(cell);

            if (currentCells.Contains(cell)) return;

            RegisterNewCell(cell);
            currentCells.Add(cell);          
        }

        protected virtual void RegisterNewCell(GridCell newCell)
        {
            newCell.RegisterObject(Config.ID, this);
        }

        protected virtual void UnregisterRemovedCells()
            => StartCoroutine(C_UnregisterRemovedCells());

        protected virtual IEnumerator C_UnregisterRemovedCells()
        {
            yield return new WaitForEndOfFrame();

            foreach (var cell in cellsNeedRemoved)
                cell.UnRegisterObject(Config.ID, this);
        }

        protected virtual IEnumerator UnregisterAllCells()
        {
            yield return new WaitForEndOfFrame();

            foreach (var cell in currentCells)
                cell.UnRegisterObject(Config.ID, this);
        }

        public virtual void CalculateCollision()
        {
            foreach (var cell in currentCells)
                if (TryCalculateCollisionPerCell(cell)) return;

            return;
        }

        protected virtual bool TryCalculateCollisionPerCell(GridCell cell)
        {
            foreach (var objectID in cell.Objects.Keys)
                if (collisionHandles.TryGetValue(objectID, out var tryHandle))
                    if(TryCalculateCollisionPerObjectID(objectID, cell, tryHandle)) return true;

            return false;
        }

        protected virtual bool TryCalculateCollisionPerObjectID(ObjectID objectID, GridCell cell, Func<ObjectCollision, bool> tryHandle)
        {
            foreach (var collisionableObject in cell.Objects[objectID])
                if (tryHandle(collisionableObject))
                    return true;

            return false;
        }
    }
}