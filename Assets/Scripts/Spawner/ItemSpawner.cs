using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BullBrukBruker
{
    public class ItemSpawner : SingletonMono<ItemSpawner>
    {
        [SerializeField] private List<GameObject> itemPrefabs;
        [SerializeField] private Transform itemHolder;
        private Dictionary<ItemID, ObjectPooler<ItemController>> itemPoolers;

        public void InitializeSpawner()
        {
            itemPoolers = new();

            foreach (var itemPrefab in itemPrefabs)
            {
                var itemController = itemPrefab.GetComponent<ItemController>();
                itemPoolers.Add(itemController.Type, new ObjectPooler<ItemController>(itemController, itemHolder, 10));
            }

        }

        public void SpawnItem(ItemID type, Vector3 position)
        {
            if (!itemPoolers.TryGetValue(type, out var pooler)) return;

            pooler.Get(position);
        }
    }
}