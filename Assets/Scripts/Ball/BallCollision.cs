using System;
using UnityEngine;

namespace BullBrukBruker
{
    public class BallCollision : ObjectCollision
    {
        protected BallCollisionConfigRecord ballConfig;
        protected ObjectDespawning despawning;
        protected BallMovingForward moving;

        protected override void Awake()
        {
            base.Awake();

            SetObjectDespawning();
            SetBallContinuousMoving();
        }

        protected override void SetObjectCollisionConfigRecord()
        {
            Config = ConfigsManager.Instance.ObjectCollisionConfig.GetRecordByKeySearch(GetComponentInParent<BallController>().ID);
            ballConfig = Config as BallCollisionConfigRecord;
        }

        protected virtual void SetObjectDespawning() => despawning = GetComponentInParent<BallController>().Despawning;

        protected virtual void SetBallContinuousMoving() => moving = GetComponentInParent<BallController>().Moving;

        protected override void InitializeCollisionHandles()
        {
            collisionHandles = new()
            {
                [ObjectID.Paddle] = objCollision => TryCollideWithPaddle(objCollision),

                [ObjectID.HorizontalLevelBound] = objCollision => TryCollideBounds(objCollision),

                [ObjectID.VerticalLevelBound] = objCollision => TryCollideBounds(objCollision),

                [ObjectID.DeathBound] = objCollision => TryCollideDeathBound(objCollision),
            };
        }

        public override void CalculateCellGroup()
        {
            CollectionUtils.MoveAllTo(currentCells, cellsNeedRemoved);

            GridManager gridManager = GridManager.Instance;

            var ro = transform.parent.position;
            var rd = transform.parent.up;
            rd.x = Mathf.Abs(rd.x) < Mathf.Epsilon ? 0.01f : rd.x;
            rd.y = Mathf.Abs(rd.y) < Mathf.Epsilon ? 0.01f : rd.y;

            if (!gridManager.TryGetCell(ro, out GridCell orinCell))
                return;

            ProcessCell(orinCell);
            Vector3Int currentIndex = orinCell.CellIndex(); 

            Vector2Int sign = new((int)Mathf.Sign(rd.x), (int)Mathf.Sign(rd.y));            
            Vector2 cellStep = new(GridCell.Size * 2 * Mathf.Sqrt(1 + (rd.y / rd.x) * (rd.y / rd.x)),
                                    GridCell.Size * 2 * Mathf.Sqrt(1 + (rd.x / rd.y) * (rd.x / rd.y)));
            var roFract = orinCell.GetFractInCell(ro);
            Vector2 nextBoundaryDistance = Vector2.zero;
            float currentRayLength = 0;

            if (sign.x < 0)
                nextBoundaryDistance.x = roFract.x * cellStep.x;
            else
                nextBoundaryDistance.x = (1 - roFract.x) * cellStep.x;

            if (sign.y < 0)
                nextBoundaryDistance.y = roFract.y * cellStep.y;
            else
                nextBoundaryDistance.y = (1 - roFract.y) * cellStep.y;

            var rayLength = Mathf.Max(moving.Config.Speed * Time.deltaTime, Config.RayLength);
            while (currentRayLength <= rayLength)
            {
                if (nextBoundaryDistance.x < nextBoundaryDistance.y)
                {
                    currentIndex.x += sign.x;
                    nextBoundaryDistance.x += cellStep.x;
                    currentRayLength = nextBoundaryDistance.x;
                }
                else
                {
                    currentIndex.y += sign.y;
                    nextBoundaryDistance.y += cellStep.y;
                    currentRayLength = nextBoundaryDistance.y;
                }

                if (gridManager.TryGetCell(currentIndex, out GridCell cell))
                    ProcessCell(cell);
            }

            UnregisterRemovedCells();
        }

        protected override bool TryCalculateCollisionPerCell(GridCell cell)
        {
            if (cell.CanCollide())
            {
                if (currentCells[0] == cell)
                    transform.parent.position -= GridCell.Size * transform.parent.up;

                var (contactPoint, distance, normal) = PhysicsUtils.GetRayIntersectedInformation(
                                                                    transform.parent,
                                                                    cell.CellPos(),
                                                                    GridCell.Size,
                                                                    GridCell.Size);
                
                if (distance <= Config.RayLength)
                {
                    transform.parent.up = Vector3.Reflect(transform.parent.up, normal);
                    cell.TakeHit();
                }
                else if (distance <= moving.CurrentSpeed * Time.deltaTime)
                    moving.ChangeSpeedOnTunneling(contactPoint);
                
                return true;
            }

            return base.TryCalculateCollisionPerCell(cell);
        }

        protected override bool TryCalculateCollisionPerObjectID(ObjectID objectID, GridCell cell, Func<ObjectCollision, bool> tryHandle)
        {
            foreach (var collisionableObject in cell.Objects[objectID])
            {
                if (tryHandle(collisionableObject))
                    return true;
                else if (TryTunnelingWithObjects(collisionableObject.transform.parent.position, collisionableObject.Config.Width, collisionableObject.Config.Height))
                    return true;
            }

            return false;
        }

        protected virtual bool TryTunnelingWithObjects(Vector3 staticObjectPos, float staticObjectWidth, float staticObjectHeight)
        {
            if (!PhysicsUtils.TryGetRayIntersectedInformation(transform.parent, Config.RayLength, staticObjectPos, staticObjectWidth, staticObjectHeight, out var intersectedInfor))
                return false;

            moving.ChangeSpeedOnTunneling(intersectedInfor.contactPoint);
            return true;
        }

        protected virtual bool TryCollideBounds(ObjectCollision objCollision)
        {
            if (!PhysicsUtils.TryGetRayIntersectedInformation(transform.parent, Config.RayLength, objCollision.transform.parent.position, objCollision.Config.Width, objCollision.Config.Height, out var intersectedInfor))
                return false;

            var newDir = Vector3.Reflect(transform.parent.up, intersectedInfor.normal);
            transform.parent.up = newDir;
            return true;
        }

        protected virtual bool TryCollideDeathBound(ObjectCollision objCollision)
        {
            if (!PhysicsUtils.TryGetRayIntersectedInformation(transform.parent, Config.RayLength, objCollision.transform.parent.position, objCollision.Config.Width, objCollision.Config.Height, out var intersectedInfor))
                return false;

            despawning.Despawn();
            return true;
        }

        protected virtual bool TryCollideWithPaddle(ObjectCollision objCollision)
        {
            var paddlePos = objCollision.transform.parent.position;

            if (!PhysicsUtils.TryGetRayIntersectedInformation(transform.parent, Config.RayLength, paddlePos, objCollision.Config.Width, objCollision.Config.Height, out var intersectedInfor))
                return false;

            var dis = paddlePos.x - transform.parent.position.x;

            float addedAngle = (dis / objCollision.Config.Width) * ballConfig.AddedBouncePerAngle;

            var newDir = Quaternion.Euler(0, 0, addedAngle) * Vector3.Reflect(transform.parent.up, intersectedInfor.normal);
            transform.parent.up = newDir;

            return true;
        }
    }
}