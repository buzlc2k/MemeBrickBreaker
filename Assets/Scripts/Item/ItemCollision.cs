using System.Collections;
using UnityEngine;

namespace BullBrukBruker
{
    public class ItemCollision : ObjectCollision
    {
        protected ItemEffecting effecting;
        protected ObjectDespawning despawning;

        protected override void Awake()
        {
            base.Awake();

            SetItemEffecting();
            SetObjectDespawning();
        }

        protected override void SetObjectCollisionConfigRecord()
        {
            Config = ConfigsManager.Instance.ObjectCollisionConfig.GetRecordByKeySearch(GetComponentInParent<ItemController>().ID);
        }

        protected virtual void SetItemEffecting() => effecting = GetComponentInParent<ItemController>().Effecting;
        protected virtual void SetObjectDespawning() => despawning = GetComponentInParent<ItemController>().Despawning;

        protected override void InitializeCollisionHandles()
        {
            collisionHandles = new()
            {
                [ObjectID.Paddle] = objCollision => TryCollideWithPaddle(objCollision),

                [ObjectID.DeathBound] = objCollision => TryCollideDeathBound(objCollision),
            };
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
            if (!PhysicsUtils.TryGetRayIntersectedInformation(transform.parent, Config.RayLength, objCollision.transform.parent.position, objCollision.Config.Width, objCollision.Config.Height, out var intersectedInfor))
                return false;

            effecting.Effect();
            despawning.Despawn();
            return true;
        }
    }
}