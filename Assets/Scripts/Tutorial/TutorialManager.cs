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
        
        
        public void ActivateStateTutorial(Dictionary<GameObject, TutorialStep> steps, bool needHand, Action onComplete)
        {
            tutorialPanel.gameObject.SetActive(true);
            StartCoroutine(DisplayTutorialSteps(steps, needHand, () =>
            {
                tutorialPanel.gameObject.SetActive(false);
                onComplete?.Invoke();
            }));
        }

        public IEnumerator DisplayTutorialSteps(Dictionary<GameObject, TutorialStep> steps, bool needHand, Action onComplete)
        {
            foreach (KeyValuePair<GameObject, TutorialStep> step in steps)
            {
                tutorialPanel.GetComponent<Image>().enabled = true;
                var temp = step.Key;
                GameObject highlight = Instantiate(tutorialFrame, tutorialParent);
                _currentHighlights.Add(highlight);
                RectTransform highlightTransform = highlight.GetComponent<RectTransform>();
                RectTransform tempTransform = temp.GetComponent<RectTransform>();
                if (tempTransform != null && highlightTransform != null)
                {
                    PositionHighlightToTarget(highlightTransform, tempTransform);
                    highlightTransform.SetSiblingIndex(0);
                    tutorialStepField.text = step.Value.StepText;
                }

                if (needHand)
                {
                    HighlightTutorialObject(highlightTransform, tempTransform, 150f, false);
                }

                if (step.Value.StepClip != null)
                {
                    AudioManager.Instance.PlayAudioClip(step.Value.StepClip);
                    yield return new WaitForSeconds(step.Value.StepClip.length + 1.5f);
                }
                else
                {
                    yield return new WaitForSeconds(3f);
                }
                
                ClearHighlights();
            }
            
            onComplete?.Invoke();
        }

        public void SetFinalTutorialField(string text, AudioClip clip)
        {
            tutorialPanel.gameObject.SetActive(true);
            tutorialPanel.GetComponent<Image>().enabled = false;
            tutorialStepField.text = $"{text}";
            AudioManager.Instance.PlayAudioClip(clip);
        }

        private RectTransform _lastTarget;
        private GameObject _spawnedHand;
        public void HighlightTutorialObject(RectTransform targetTransform, RectTransform parent, float offset, bool animNeeded)
        {
            if (targetTransform == _lastTarget) return;
            if (_spawnedHand != null)
            {
                Destroy(_spawnedHand.gameObject);
            }
            _spawnedHand = Instantiate(tutorialHand, animNeeded ? parent : tutorialParent);
            _currentHighlights.Add(_spawnedHand);
            RectTransform handTransform = _spawnedHand.GetComponent<RectTransform>();
            PositionHighlightToTarget(handTransform, targetTransform, offset);
            handTransform.DOKill();
            if (animNeeded)
            {
                AnimateHighlight(handTransform);
            }
        }

        private void AnimateHighlight(RectTransform highlight)
        {
            highlight.DOScale(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }

        private void PositionHighlightToTarget(RectTransform highlightTransform, RectTransform target, float offset = 0)
        {
            var anchoredPosition = target.anchoredPosition;
            highlightTransform.anchoredPosition = new Vector2(anchoredPosition.x - offset, anchoredPosition.y - offset/2);
            highlightTransform.anchorMin = target.anchorMin;
            highlightTransform.anchorMax = target.anchorMax;
            highlightTransform.sizeDelta = target.sizeDelta;
            highlightTransform.pivot = target.pivot;
        }
        
        public void ClearHighlights()
        {
            if(_spawnedHand != null)
                _spawnedHand.transform.DOKill();
            if (_currentHighlights != null)
            {
                _currentHighlights.ForEach(x => Destroy(x.gameObject));
            }
        }

        public void DisablePanel()
        {
            tutorialPanel.gameObject.SetActive(false);
        }
    }
}
