using UnityEngine;

namespace BullBrukBruker
{
    public class ItemMovingBackward : ObjectMoving
    {
        protected override void SetObjectMovingConfigRecord()
            => Config = ConfigsManager.Instance.ObjectMovingConfig.GetRecordByKeySearch(GetComponentInParent<ItemController>().ID);

        protected override void CalculateTargetPosition()
        {
            targetPosition = transform.position - transform.up;
        }
    }
}