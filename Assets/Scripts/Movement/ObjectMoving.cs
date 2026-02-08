using UnityEngine;

namespace BullBrukBruker
{
    public abstract class ObjectMoving : MonoBehaviour
    {
        [Header("ObjMovement")]
        protected Vector3 targetPosition = Vector3.zero;
        
        public float CurrentSpeed { get; protected set; }

        //Depedencies
        public ObjectMovingConfigRecord Config { get; protected set; }

        protected virtual void Awake()
        {
            SetObjectMovingConfigRecord();

            CurrentSpeed = Config.Speed;
        }

        protected abstract void SetObjectMovingConfigRecord();

        protected abstract void CalculateTargetPosition();

        public virtual void Move()
        {
            CalculateTargetPosition();

            if(transform.parent.position.Equals(targetPosition)) return;

            transform.parent.position = Vector3.MoveTowards(transform.parent.position, 
                                                            targetPosition, 
                                                            CurrentSpeed * Time.deltaTime);
        }
    }
}