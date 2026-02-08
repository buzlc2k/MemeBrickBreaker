using UnityEngine;

namespace BullBrukBruker
{
    public class PathNode
    {
        public Vector3Int Index { get; set; }

        public float GCost { get; set; }
        public float HCost { get; set; }
        public float BiasCost { get; set; }
        public float FCost { get; set; }

        public PathNode PreviousNode { get; set; }

        public PathNode(Vector3Int Index)
        {
            this.Index = Index;

            GCost = Mathf.Infinity;
            HCost = 0;
            BiasCost = 0;
            FCost = 0;

            PreviousNode = null;
        }

        public void CalculateFCost()
            => FCost = GCost + HCost;
    }
}