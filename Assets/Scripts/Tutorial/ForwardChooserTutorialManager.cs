using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scriptables.Tutorial;
using Spans.Skeleton;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Tutorial
{
    public class ForwardChooserTutorialManager : MonoBehaviour
    {
        [SerializeField] private RectTransform tutorialPanel;
        [SerializeField] private GameObject tutorialFrame;
        [SerializeField] private TextMeshProUGUI tutorialStepField;
        private List<GameObject> _objectsToHighlight = new List<GameObject>();
        private List<GameObject> _currentHighlights = new List<GameObject>();
        private List<TutorialStep> _steps = new List<TutorialStep>();
        private string _currentTutorialKey;
        
        public void ActivateTutorial(List<TutorialStep> steps, string key)
        {
            _steps = steps;
            _currentTutorialKey = key;
            tutorialPanel.gameObject.SetActive(true);
            StartCoroutine(IterateTutorialSteps());
        }

        public void SetObjectsToHighlight(List<ISpanState> states)
        {
            foreach (var state in states)
            {
                
                _objectsToHighlight.AddRange(state.GetTutorialObjects());
            }
        }

        private IEnumerator IterateTutorialSteps()
        {
            for (int i = 0; i < _objectsToHighlight.Count; i++)
            {
                if (i == 0)
                {
                    tutorialStepField.text = _steps[i].StepText;
                    continue;
                }
                
                var temp = _objectsToHighlight[i];
                GameObject highlight = Instantiate(tutorialFrame, tutorialPanel);
                RectTransform highlightTransform = highlight.GetComponent<RectTransform>();
        
                if (Camera.main != null)
                {
                    
                }
                
                yield return new WaitForSeconds(7f);
        
                ClearHighlight();
                PlayerSaveManager.SavePlayerAttribute(1, _currentTutorialKey);
                _currentTutorialKey = null;
            }
        }
        
        private void ClearHighlight()
        {
            if (_currentHighlights != null)
            {
                _currentHighlights.ForEach(x => Destroy(x.gameObject));
            }
        }
    }
}
