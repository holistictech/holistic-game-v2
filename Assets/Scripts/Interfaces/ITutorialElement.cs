using System.Collections;
using System.Collections.Generic;
using Scriptables.Tutorial;
using UnityEngine;
using Utilities;

namespace Interfaces
{
    public interface ITutorialElement
    {
        public void TryShowTutorial(Dictionary<GameObject, TutorialStep> highlights, RectTransform finalHighlight, float offset);
        public IEnumerator WaitInput(RectTransform finalHighlight, float offset);

        public bool CanShowStep(string tutorialKey)
        {
            return PlayerSaveManager.GetPlayerAttribute(tutorialKey, 0) == 0;
        }

        public void SetStepCompleted(string tutorialKey)
        {
            PlayerSaveManager.SavePlayerAttribute(1, tutorialKey);
        }
    }
}
