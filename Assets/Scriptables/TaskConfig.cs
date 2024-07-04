using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Helpers;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "TaskConfig", menuName = "Tasks/Task")]
    public class TaskConfig : ScriptableObject
    {
        public Sprite TaskSprite;
        public String Mission;
        public int Cost;
        public CommonFields.CurrencyType CurrencyType;
        public CommonFields.WarningType WarningType;
        public string WarningString;
        public InteractableConfig RewardInteractable;
        public List<TaskConfig> Dependencies;
        public bool _hasCompleted;
        public bool Rotatable;
        
        public void SetHasCompleted(bool toggle)
        {
            _hasCompleted = toggle;
        }

        public bool GetHasCompleted()
        {
            return _hasCompleted;
        }

        public bool CanBeCompleted()
        {
            foreach (var task in Dependencies)
            {
                if (!task.GetHasCompleted())
                    return false;
            }

            return true;
        }

        public void SetWarningString()
        {
            var dependencyName = GetDependencyTaskName();
            WarningString = $"Bu görevi tamamlamak için önce {dependencyName} görevini tamamlamalısın";
        }

        public string GetWarningString()
        {
            return WarningString;
        }

        private string GetDependencyTaskName()
        {
            foreach (var task in Dependencies)
            {
                if (!task.GetHasCompleted())
                {
                    return task.Mission;
                }
            }
            
            return "";
        }
    }
}
