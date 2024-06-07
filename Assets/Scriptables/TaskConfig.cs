using System;
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
        public InteractableConfig RewardInteractable;
        public bool _hasCompleted;
        
        public void SetHasCompleted(bool toggle)
        {
            _hasCompleted = toggle;
        }

        public bool GetHasCompleted()
        {
            return _hasCompleted;
        }
    }
}
