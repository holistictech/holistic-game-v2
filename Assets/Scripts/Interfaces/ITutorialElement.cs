using System.Collections;
using System.Collections.Generic;
using Scriptables.Tutorial;
using UnityEngine;
using Utilities;

namespace Interfaces
{
    public interface ITutorialElement
    {
        public void TryShowTutorial(Dictionary<GameObject, TutorialStep> highlights, RectTransform finalHighlight);
        public IEnumerator WaitInput(RectTransform finalHighlight);

        public bool CanShowStep(string tutorialKey)
        {
            return PlayerSaveManager.GetPlayerAttribute(tutorialKey, 0) == 0;
        }
    }
}
