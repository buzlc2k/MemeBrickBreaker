using System;
using System.Collections;
using System.Linq;
using SABI;
using UnityEngine;

namespace BullBrukBruker
{
    public class BootLoader : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return StartCoroutine(InitBootLoader());

            //Configs = ConfigManager 

            yield return StartCoroutine(InitDataManager());

            yield return StartCoroutine(InitLoadingManager());

            yield return StartCoroutine(InitScreenManager());

            yield return StartCoroutine(InitSceneManager());

            yield return StartCoroutine(InitCanvasManager());

            yield return StartCoroutine(InitGameManager());

            yield return StartCoroutine(InitGridManager());

            yield return StartCoroutine(InitLevelManager());

            yield return StartCoroutine(InitInputManager());

            yield return StartCoroutine(InitAudioManager());

            yield return StartCoroutine(InitRankingManager());

            yield return StartCoroutine(InitMemeManager());

            yield return StartCoroutine(InitRedirectManager());

            yield return StartCoroutine(InitSpawners());

            Debug.Log("====== DONE PROCESS ======");

            StartCoroutine(PS_SceneManager.Instance.LoadScene("Gameplay", 2));
        }

        private IEnumerator InitBootLoader()
        {
            Debug.Log("====== START PROCESS ======");

            FindObjectsByType<BootLoader>(FindObjectsSortMode.None)
                                        .Where(instance => instance != this)
                                        .ToList()
                                        .ForEach(instance => Destroy(instance.gameObject));

            yield return null;

            foreach (Transform child in transform)
                child.gameObject.SetActive(true);

            DontDestroyOnLoad(this);

            while(Application.internetReachability == NetworkReachability.NotReachable)
                yield return null;
                
            yield return null;
        }

        private IEnumerator InitDataManager()
        {
            Debug.Log("====== START INIT DATA MANAGER ======");

            while (DataManager.Instance == null)
                yield return null;

            yield return StartCoroutine(DataManager.Instance.InitDataManager());

            Debug.Log("====== INIT DATA MANAGER DONE ======");
        }

        private IEnumerator InitLoadingManager()
        {
            Debug.Log("====== START INIT LOADING MANAGER ======");

            while (LoadingManager.Instance == null)
                yield return null;

            yield return StartCoroutine(LoadingManager.Instance.InitLoadingManager());

            Debug.Log("====== INIT LOADING MANAGER DONE ======");
        }

        private IEnumerator InitScreenManager()
        {
            Debug.Log("====== START INIT SCREEN MANAGER ======");

            while (ScreenManager.Instance == null)
                yield return null;

            yield return StartCoroutine(ScreenManager.Instance.InitScreenManager());

            Debug.Log("====== INIT SCREEN MANAGER DONE ======");
        }

        private IEnumerator InitSceneManager()
        {
            Debug.Log("====== START INIT SCENE MANAGER ======");

            while (PS_SceneManager.Instance == null)
                yield return null;

            yield return StartCoroutine(PS_SceneManager.Instance.InitSceneManager());

            Debug.Log("====== INIT SCENE MANAGER DONE ======");
        }

        private IEnumerator InitCanvasManager()
        {
            Debug.Log("====== START INIT CANVAS MANAGER ======");

            while (CanvasManager.Instance == null)
                yield return null;

            yield return StartCoroutine(CanvasManager.Instance.InitCanvasManager());

            Debug.Log("====== INIT CANVAS MANAGER DONE ======");
        }

        private IEnumerator InitGameManager()
        {
            Debug.Log("====== START INIT GAME MANAGER ======");

            while (GameManager.Instance == null)
                yield return null;

            yield return StartCoroutine(GameManager.Instance.InitGameManager());

            Debug.Log("====== INIT GAME MANAGER DONE ======");
        }

        private IEnumerator InitGridManager()
        {
            Debug.Log("====== START INIT GRID MANAGER ======");

            while (GridManager.Instance == null)
                yield return null;

            Debug.Log("====== INIT GRID MANAGER DONE ======");
        }

        private IEnumerator InitLevelManager()
        {
            Debug.Log("====== START INIT LEVEL MANAGER ======");

            while (LevelManager.Instance == null)
                yield return null;

            yield return StartCoroutine(LevelManager.Instance.InitLevelManager());

            Debug.Log("====== INIT LEVEL MANAGER DONE ======");
        }

        private IEnumerator InitInputManager()
        {
            Debug.Log("====== START INIT INPUT MANAGER ======");

            while (InputManager.Instance == null)
                yield return null;

            yield return StartCoroutine(InputManager.Instance.InitInputManager());

            Debug.Log("====== INIT INPUT MANAGER DONE ======");
        }

        private IEnumerator InitAudioManager()
        {
            Debug.Log("====== START INIT AUDIO MANAGER ======");

            while (AudioManager.Instance == null)
                yield return null;

            yield return StartCoroutine(AudioManager.Instance.InitAudioManager());

            Debug.Log("====== INIT AUDIO MANAGER DONE ======");
        }

        private IEnumerator InitRankingManager()
        {
            Debug.Log("====== START INIT RANKING MANAGER ======");

            while (RankingManager.Instance == null)
                yield return null;

            Debug.Log("====== INIT RANKING MANAGER DONE ======");
        }

        private IEnumerator InitMemeManager()
        {
            Debug.Log("====== START INIT MEME MANAGER ======");

            while (MemeManager.Instance == null)
                yield return null;

            Debug.Log("====== INIT MEME MANAGER DONE ======");
        }

        private IEnumerator InitRedirectManager()
        {
            Debug.Log("====== START INIT REDIRECT MANAGER ======");

            while (RedirectManager.Instance == null)
                yield return null;

            Debug.Log("====== INIT REDIRECT MANAGER DONE ======");
        }

        private IEnumerator InitSpawners()
        {
            Debug.Log("====== START INIT SPAWNERS ======");

            while (BallSpawner.Instance == null || ItemSpawner.Instance == null)
                yield return null;

            BallSpawner.Instance.InitializeSpawner();
            ItemSpawner.Instance.InitializeSpawner();

            Debug.Log("====== INIT SPAWNERS DONE ======");
        }
    }
}