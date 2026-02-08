using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BullBrukBruker
{
    public static class CollectionUtils
    {
        public static void MoveAllTo<T>(ICollection<T> source, ICollection<T> destination)
        {
            destination.Clear();
            foreach (var item in source)
                destination.Add(item);
            source.Clear();
        }
    }
}