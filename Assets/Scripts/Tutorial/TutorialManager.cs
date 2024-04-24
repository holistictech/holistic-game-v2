using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Scriptables.Tutorial;
using Spans.Skeleton;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

namespace Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private RectTransform tutorialParent;
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private GameObject tutorialFrame;
        [SerializeField] private GameObject tutorialHand;
        [SerializeField] private TextMeshProUGUI tutorialStepField;
        private List<GameObject> _currentHighlights = new List<GameObject>();
        private string _currentTutorialKey;
        
        
        public void ActivateStateTutorial(Dictionary<GameObject, TutorialStep> steps, Action onComplete)
        {
            tutorialPanel.gameObject.SetActive(true);
            StartCoroutine(DisplayTutorialSteps(steps, () =>
            {
                tutorialPanel.gameObject.SetActive(false);
                onComplete?.Invoke();
            }));
        }

        public IEnumerator DisplayTutorialSteps(Dictionary<GameObject, TutorialStep> steps, Action onComplete)
        {
            foreach (KeyValuePair<GameObject, TutorialStep> step in steps)
            {
                var temp = step.Key;
                GameObject highlight = Instantiate(tutorialFrame, tutorialParent);
                _currentHighlights.Add(highlight);
                RectTransform highlightTransform = highlight.GetComponent<RectTransform>();
                RectTransform tempTransform = temp.GetComponent<RectTransform>();
                if (tempTransform != null && highlightTransform != null)
                {
                    PositionHighlightToTarget(highlightTransform, tempTransform);
                    /*highlightTransform.anchoredPosition = tempTransform.anchoredPosition;
                    highlightTransform.anchorMin = tempTransform.anchorMin;
                    highlightTransform.anchorMax = tempTransform.anchorMax;
                    
                    highlightTransform.sizeDelta = tempTransform.sizeDelta;
                    highlightTransform.pivot = tempTransform.pivot;*/
                    highlightTransform.SetSiblingIndex(0);
                    tutorialStepField.text = step.Value.StepText;
                }
                yield return new WaitForSeconds(4.5f);
                ClearHighlights();
            }
            
            onComplete?.Invoke();
        }

        public void SetTutorialWaitingInput(string text)
        {
            tutorialPanel.gameObject.SetActive(true);
            tutorialPanel.GetComponent<Image>().enabled = false;
            tutorialStepField.text = $"{text}";
        }

        private RectTransform _lastTarget;
        private GameObject _spawnedHand;
        public void HighlightTutorialObject(RectTransform targetTransform, RectTransform parent, float offset)
        {
            if (targetTransform == _lastTarget) return;
            if (_spawnedHand == null)
            {
                _spawnedHand = Instantiate(tutorialHand, parent);
                _currentHighlights.Add(_spawnedHand);
            }
            RectTransform handTransform = _spawnedHand.GetComponent<RectTransform>();
            handTransform.SetParent(parent);
            PositionHighlightToTarget(handTransform, targetTransform, offset);
            handTransform.DOKill();
            handTransform.DOScale(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }

        private void PositionHighlightToTarget(RectTransform highlightTransform, RectTransform target, float offset = 0)
        {
            var anchoredPosition = target.anchoredPosition;
            highlightTransform.anchoredPosition = new Vector2(anchoredPosition.x - offset, anchoredPosition.y + offset);
            highlightTransform.anchorMin = target.anchorMin;
            highlightTransform.anchorMax = target.anchorMax;
            highlightTransform.sizeDelta = target.sizeDelta;
            highlightTransform.pivot = target.pivot;
        }
        
        private void ClearHighlights()
        {
            if (_currentHighlights != null)
            {
                _currentHighlights.ForEach(x => Destroy(x.gameObject));
            }
        }
    }
}
