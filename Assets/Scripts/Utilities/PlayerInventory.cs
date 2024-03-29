using System.Collections.Generic;
using Scriptables;
using UnityEngine;

namespace Utilities
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private Level[] gameLevels;
        private static PlayerInventory _instance;
        
        private string CurrencyKey = "CurrencyKey";
        public int Currency { get; private set; }

        private string ExperienceKey = "ExperienceKey";
        public int Experience { get; private set; }

        private string CurrentLevelKey = "CurrentLevel";
        public int CurrentLevel { get; private set; }
        
        private string CurrentStageKey = "CurrentStage";
        public int CurrentStage { get; private set; }

        public static PlayerInventory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PlayerInventory>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject("PlayerInventory");
                        _instance = singletonObject.AddComponent<PlayerInventory>();
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            Currency = PlayerSaveManager.GetPlayerAttribute(CurrencyKey, 0);
            Experience = PlayerSaveManager.GetPlayerAttribute(ExperienceKey, 0);
            CurrentLevel = PlayerSaveManager.GetPlayerAttribute(CurrentLevelKey, 0);
            CurrentStage = PlayerSaveManager.GetPlayerAttribute(CurrentStageKey, 0);
            Currency = 100;
        }

        public void ChangeCurrencyAmount(int amount)
        {
            Currency += amount;
            PlayerSaveManager.SavePlayerAttribute(Currency, CurrencyKey);
        }

        public void ChangeExperienceAmount(int amount)
        {
            Experience += amount;
            PlayerSaveManager.SavePlayerAttribute(Experience, ExperienceKey);
        }

        public void SetCurrentLevel(int level)
        {
            CurrentLevel = level;
            PlayerSaveManager.SavePlayerAttribute(CurrentLevel, CurrentLevelKey);
        }

        public void IncrementCurrentLevel()
        {
            CurrentLevel++;
            PlayerSaveManager.SavePlayerAttribute(CurrentLevel, CurrentLevelKey);
        }

        public void ChangeCurrentStage(int stage)
        {
            CurrentStage = stage;
            PlayerSaveManager.SavePlayerAttribute(CurrentStage, CurrentStageKey);
        }

        public void IncrementCurrentStage()
        {
            CurrentStage++;
            PlayerSaveManager.SavePlayerAttribute(CurrentStage, CurrentStageKey);
        }

        public List<TaskConfig> GetAvailableTasks()
        {
            var level = PlayerSaveManager.GetPlayerAttribute(CurrentLevelKey, 0);
            return gameLevels[level].GetAvailableTasks();
        }
    }

}
