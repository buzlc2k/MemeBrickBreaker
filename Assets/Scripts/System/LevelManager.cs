using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BullBrukBruker
{
    public class LevelManager : SingletonMono<LevelManager>
    {
        [SerializeField] GameObject PaddlePrefab;

        public int RemainAttemps { get; private set; }
        public int CurrentIndex { get; private set; }
        public PaddelController CurrentPaddle { get; private set; }
        public List<GameObject> CurrentItems { get; private set; }
        public List<GameObject> CurrentBalls { get; private set; }

        private Action<object> loadCurrentLevel;
        private Action<object> loadLevel;
        private Action<object> loadNextLevel;

        public IEnumerator InitLevelManager()
        {
            CurrentIndex = DataManager.Instance.GetCurrentLevel();

            yield return null;
        }

        private void OnEnable()
        {
            InitializeDelegate();
            RegisterEvent();
        }

        private void OnDisable()
        {
            UnRegisterEvent();
        }

        private void InitializeDelegate()
        {
            loadCurrentLevel ??= (param) =>
            {
                LoadCurrentLevel();
            };

            loadLevel ??= (param) =>
            {
                LoadLevel((int)param);
            };

            loadNextLevel ??= (param) =>
            {
                LoadNextLevel();
            };
        }

        private void RegisterEvent()
        {
            Observer.AddListener(EventID.PlayButton_Clicked, loadCurrentLevel);
            Observer.AddListener(EventID.LevelButton_Clicked, loadLevel);
            Observer.AddListener(EventID.ReplayButton_Clicked, loadCurrentLevel); 
            Observer.AddListener(EventID.NextLevelButton_Clicked, loadNextLevel);
        }

        private void UnRegisterEvent()
        {
            Observer.RemoveListener(EventID.PlayButton_Clicked, loadCurrentLevel);
            Observer.RemoveListener(EventID.LevelButton_Clicked, loadLevel);
            Observer.RemoveListener(EventID.ReplayButton_Clicked, loadCurrentLevel);
            Observer.RemoveListener(EventID.NextLevelButton_Clicked, loadNextLevel);
        }

        public void LoadCurrentLevel()
        {
            LoadLevel(CurrentIndex);
        }

        public void LoadNextLevel()
        {
            LoadLevel(CurrentIndex + 1);
        }

        public void LoadLevel(int index)
        {
            StartCoroutine(C_LoadLevel(index));
        }

        private IEnumerator C_LoadLevel(int index)
        {
            LoadingManager.Instance.InitLoading(GetType().Name);

            if (!TryPrepareLevel(index))
                yield break;

            yield return StartCoroutine(C_LoadLevelObjects());

            yield return StartCoroutine(C_LoadLevelGrid(CurrentIndex));

            yield return new WaitForSecondsRealtime(.5f);

            LoadingManager.Instance.StopLoading(GetType().Name);

            LoadAttemps();
        }

        private bool TryPrepareLevel(int index)
        {            
            return index switch
            {
                var i when i > ConfigsManager.Instance.LevelConfig.GetTotalLevels() => HandleOutOfLevels(),
                var i => ProcessValidLevel(i)
            };
        }

        private bool HandleOutOfLevels()
        {
            Observer.PostEvent(EventID.OutOfLevels, null);
            
            if (DataManager.Instance.GetStarsPerLevel().Count <= ConfigsManager.Instance.LevelConfig.GetTotalLevels())
                DataManager.Instance.AddStarsPerLevelAndScore(RemainAttemps);
                
            return false;
        }

        private bool ProcessValidLevel(int index)
        {
            CurrentIndex = index;
            var dataManager = DataManager.Instance;
            
            if (CurrentIndex > dataManager.GetHighestLevel())
            {
                dataManager.WriteHighestLevel(CurrentIndex);
                dataManager.AddStarsPerLevelAndScore(RemainAttemps);
            }
            
            dataManager.WriteCurrentLevel(CurrentIndex);
            return true;
        }

        private IEnumerator C_LoadLevelObjects()
        {
            LoadPaddel();

            yield return null;

            LoadBalls();

            yield return null;

            LoadItem();
        }

        private void LoadPaddel()
        {
            CurrentPaddle = CurrentPaddle != null ? CurrentPaddle : Instantiate(PaddlePrefab).GetComponent<PaddelController>();
            CurrentPaddle.InitPaddle();
        }

        private void LoadBalls()
        {
            CurrentBalls ??= new();
            CurrentBalls.ForEach(ball => ball.SetActive(false));
            CurrentBalls.Clear();
        }

        private void LoadItem()
        {
            CurrentItems ??= new();
            CurrentItems.ForEach(item => item.SetActive(false));
            CurrentItems.Clear();
        }

        private IEnumerator C_LoadLevelGrid(int index)
        {
            LevelConfig levelConfig = null;

            while (levelConfig == null)
            {
                levelConfig = ConfigsManager.Instance.LevelConfig;
                yield return null;
            }

            GridManager.Instance.InitGridCells(levelConfig.GetRecord(index).Cells);
        }

        private void LoadAttemps() => RemainAttemps = 3;

        public void AddBall(GameObject ballAdded) => CurrentBalls?.Add(ballAdded);

        public void RemoveBall(GameObject ballRemoved) => CoroutineUtils.StartSafeCourotine(this, C_RemoveActiveBall(ballRemoved));

        private IEnumerator C_RemoveActiveBall(GameObject ballRemoved)
        {
            yield return new WaitForEndOfFrame();
            if (CurrentBalls == null) yield break;

            CurrentBalls.Remove(ballRemoved);

            if (CurrentBalls.Count == 0)
                TryStartNextAttempt();
        }

        private void TryStartNextAttempt()
        {
            RemainAttemps--;

            Observer.PostEvent(EventID.StartNextAttempt, RemainAttemps);            
            if (RemainAttemps > 0)
                LoadItem();
        }
        
        public void AddItem(GameObject item) => CurrentItems?.Add(item);

        public void RemoveItem(GameObject itemRemoved) => CoroutineUtils.StartSafeCourotine(this, C_RemoveActiveItem(itemRemoved));

        private IEnumerator C_RemoveActiveItem(GameObject itemRemoved)
        {
            yield return new WaitForEndOfFrame();
            if (CurrentItems == null) yield break;

            CurrentItems?.Remove(itemRemoved);
        }
    }
}