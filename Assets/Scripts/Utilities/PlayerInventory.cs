using System.Collections.Generic;
using System.Linq;
using Scriptables;
using UnityEngine;

namespace Utilities
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private Level[] gameLevels;
        [SerializeField] private TaskConfig[] performanceItems; 
        private static PlayerInventory _instance;
        
        private string EnergyKey = "EnergyKey";
        public int Energy { get; private set; }

        private string PerformanceKey = "PerformanceKey";
        public int Performance { get; private set; }

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

            Energy = PlayerSaveManager.GetPlayerAttribute(EnergyKey, 0);
            Performance = PlayerSaveManager.GetPlayerAttribute(PerformanceKey, 0);
            CurrentLevel = PlayerSaveManager.GetPlayerAttribute(CurrentLevelKey, 0);
            CurrentStage = PlayerSaveManager.GetPlayerAttribute(CurrentStageKey, 0);
            Energy = 10;
        }

        public void ChangeCurrencyAmountByType(TaskConfig config)
        {
            if (config.CurrencyType == CommonFields.CurrencyType.Energy)
            {
                ChangeEnergyAmount(-config.Cost);
            }
            else if (config.CurrencyType == CommonFields.CurrencyType.Performance)
            {
                ChangePerformanceAmount(-config.Cost);
            }
        }

        public void ChangeEnergyAmount(int amount)
        {
            Energy += amount;
            PlayerSaveManager.SavePlayerAttribute(Energy, EnergyKey);
        }

        public void ChangePerformanceAmount(int amount)
        {
            Performance += amount;
            PlayerSaveManager.SavePlayerAttribute(Performance, PerformanceKey);
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

        public List<TaskConfig> GetTasksByType(bool type)
        {
            return type ? GetAvailableTasks() : GetPerformanceItems();
        }

        private List<TaskConfig> GetAvailableTasks()
        {
            var level = PlayerSaveManager.GetPlayerAttribute(CurrentLevelKey, 0);
            return gameLevels[level].GetAvailableTasks();
        }

        private List<TaskConfig> GetPerformanceItems()
        {
            return performanceItems.ToList();
        }

        public GameObject GetNextSpan()
        {
            return gameLevels[CurrentLevel].GetSpanByIndex(CurrentStage);
        }
    }

}
