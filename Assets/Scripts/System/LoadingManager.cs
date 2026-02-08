using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BullBrukBruker
{
    public class LoadingManager : SingletonMono<LoadingManager>
    {
        private HashSet<string> loadingObjects;
        public bool IsLoading { get; private set; }

        public IEnumerator InitLoadingManager()
        {
            loadingObjects = new();
            IsLoading = false;

            yield return null;
        }

        public void InitLoading(string objLoading)
        {
            if (!IsLoading)
                IsLoading = true;

            loadingObjects.Add(objLoading);
        }

        public void StopLoading(string objLoading)
        {
            loadingObjects.Remove(objLoading);

            if(loadingObjects.Count == 0)
                IsLoading = false;
        }
    }
}