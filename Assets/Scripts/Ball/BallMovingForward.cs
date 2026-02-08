using UnityEngine;

namespace BullBrukBruker
{
    public class BallMovingForward : ObjectMoving
    {
        protected bool adjustedCurrentSpeed = false;

        protected override void SetObjectMovingConfigRecord()
            => Config = ConfigsManager.Instance.ObjectMovingConfig.GetRecordByKeySearch(GetComponentInParent<BallController>().ID);

        protected override void CalculateTargetPosition()
        {
            targetPosition = transform.position + transform.up;
        }

        public void ChangeSpeedOnTunneling(Vector3 contactPoint)
        {
            if (adjustedCurrentSpeed) return;
            
            float dis = Vector3.Distance(contactPoint, transform.parent.position);
            CurrentSpeed = dis / Time.deltaTime;
            adjustedCurrentSpeed = true;
        }

        public override void Move()
        {
            base.Move();

            if (adjustedCurrentSpeed)
                CurrentSpeed = Config.Speed;
        }
    }
}