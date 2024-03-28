using System;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "TaskConfig", menuName = "Tasks/Task")]
    public class TaskConfig : ScriptableObject
    {
        public Sprite TaskSprite;
        public String Mission;
        public int Cost;
        public InteractableConfig RewardInteractable;
    }
}
