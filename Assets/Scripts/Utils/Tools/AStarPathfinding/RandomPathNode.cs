using UnityEngine;

namespace BullBrukBruker
{
    public class RandomPathNode : PathNode
    {
        public RandomPathNode(Vector3Int Index) : base(Index)
        {
            BiasCost = GetRandomBiasCost();
        }

        private int GetRandomBiasCost()
        {
            int baseCost = Random.Range(-1000, 1001);

            float multiplierChance = Random.value;

            if (multiplierChance < 0.3f)
                return baseCost;
            else if (multiplierChance < 0.6f)
                return baseCost * Random.Range(5, 16);
            else if (multiplierChance < 0.75f)
                return baseCost * Random.Range(30, 51);
            else
                return baseCost * Random.Range(80, 101);
        }
    }
}