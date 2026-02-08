using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BullBrukBruker
{
    public class PS_SceneManager : SingletonMono<PS_SceneManager>
    {
        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public IEnumerator InitSceneManager()
        {
            LoadingManager.Instance.InitLoading(GetType().Name);
            yield return null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => LoadingManager.Instance.StopLoading(GetType().Name);

        public IEnumerator LoadScene(int index, float offsetTime = 0)
        {
            LoadingManager.Instance.InitLoading(GetType().Name);
            float currentTime = 0;
            while (currentTime <= offsetTime)
            {
                currentTime += Time.unscaledDeltaTime;
                yield return null;
            }

            AsyncOperation reloadAsync = SceneManager.LoadSceneAsync(index);
            while (!reloadAsync.isDone)
                yield return null;
        }

        public IEnumerator LoadScene(string name, float offsetTime = 0)
        {
            LoadingManager.Instance.InitLoading(GetType().Name);
            float currentTime = 0;
            while (currentTime <= offsetTime)
            {
                currentTime += Time.unscaledDeltaTime;
                yield return null;
            }

            AsyncOperation reloadAsync = SceneManager.LoadSceneAsync(name);
            while (!reloadAsync.isDone)
                yield return null;
        }
    }
}