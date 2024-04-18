using System.Collections.Generic;
using UnityEngine;

namespace Scriptables.Tutorial
{
    
    [CreateAssetMenu(fileName = "New Tutorial Step", menuName = "Tutorial/Step")]
    public class TutorialStep : ScriptableObject
    {
        public AudioClip StepClip;
        public string StepText;
    }
}
