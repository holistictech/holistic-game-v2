using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Scriptables.QuestionSystem;
using Scriptables.Tutorial;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

namespace Spans.Skeleton
{
    public class SpanQuestionState : MonoBehaviour, ISpanState
    {
        [SerializeField] private List<TutorialStep> steps;
        [SerializeField] private Image questionFieldParent;
        [SerializeField] private Image questionBox;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private HorizontalLayoutGroup unitParent;
        [SerializeField] private UnitCircle unit;
        
        private SpanController _spanController;
        private List<Question> _spanObjects;
        private List<UnitCircle> _spawnedUnitPool;
        private List<UnitCircle> _activeCircles;
        private int _currentQuestionIndex;
        private List<Question> _currentQuestions = new List<Question>();
        private Coroutine _displayingQuestions;

        private void Start()
        {
            SpawnUnitCircles();
        }

        public void Enter(SpanController spanController)
        {
            if (_spanController == null)
            {
                _spanController = spanController;
            }
            _spanObjects = _spanController.GetSpanObjects();
            EnableUIElements();
            SetCircleUI(_spanController.GetRoundIndex());
            if (_spanController.GetTutorialStatus())
            {
                TryShowStateTutorial();
            }
            else
            {
                ShowQuestion();
            }
        }

        private void ShowQuestion()
        {
            _currentQuestions = new List<Question>();
            if (_currentQuestionIndex + _spanController.GetRoundIndex() >= _spanObjects.Count && !_spanController.GetCumulativeStatus())
            {
                _spanObjects = _spanController.GetSpanObjects();
                _currentQuestionIndex = 0;
            }
            /*else if (_spanController.GetCumulativeStatus())
            {
                _currentQuestionIndex = 0;
            }*/
            
            var question = _spanObjects[_currentQuestionIndex];
            if (question is NumberQuestion)
            {
                _displayingQuestions = StartCoroutine(ShowNumbers());
            } else if (question is ImageQuestion)
            {
                _displayingQuestions = StartCoroutine(ShowImages());
            } else if (question is ClipQuestion)
            {
                _displayingQuestions = StartCoroutine(PlayClips());
            }
        }

        private IEnumerator ShowNumbers()
        {
            for (int i = 0; i < _spanObjects.Count; i++)
            {
                if (_currentQuestionIndex >= _spanObjects.Count)
                {
                    break;
                }
                var question = _spanObjects[_currentQuestionIndex];
                questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{question.GetQuestionItem()}";
                ActivateCircle(i);
                questionBox.enabled = false;
                _currentQuestions.Add(question);
                _currentQuestionIndex++;
                yield return new WaitForSeconds(1f);
                questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"";
                yield return new WaitForSeconds(1f);
            }
            
            DOVirtual.DelayedCall(1f, SwitchNextState);
        }

        private IEnumerator ShowImages()
        {
            for (int i = 0; i < _spanObjects.Count; i++)
            {
                if (_currentQuestionIndex >= _spanObjects.Count)
                {
                    break;
                }
                var question = _spanObjects[_currentQuestionIndex];
                questionBox.sprite = (Sprite)question.GetQuestionItem();
                questionBox.enabled = true;
                ActivateCircle(_currentQuestionIndex);
                _currentQuestions.Add(question);
                _currentQuestionIndex++;
                yield return new WaitForSeconds(1f);
                questionBox.enabled = false;
                yield return new WaitForSeconds(1f);
            }

            DOVirtual.DelayedCall(1f, SwitchNextState);
        }

        private IEnumerator PlayClips()
        {
            for (int i = 0; i < _spanObjects.Count; i++)
            {
                if (_currentQuestionIndex >= _spanObjects.Count)
                {
                    break;
                }
                var question = _spanObjects[_currentQuestionIndex];
                audioSource.Play((ulong)question.GetQuestionItem());
                ActivateCircle(i);
                _currentQuestions.Add(question);
                _currentQuestionIndex++;
                yield return new WaitForSeconds((ulong)question.GetQuestionItem() + 1f);
            } 
            
            DOVirtual.DelayedCall(1f, SwitchNextState);
        }

        private void SetCircleUI(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var tempCircle = GetAvailableUnitCircle();
                tempCircle.EnableSelf();
                _activeCircles.Add(tempCircle);
            }

            _spanController.SetActiveCircles(_activeCircles);
        }

        private void SpawnUnitCircles()
        {
            _spawnedUnitPool = new List<UnitCircle>();
            _activeCircles = new List<UnitCircle>();
            for (int i = 0; i < 9; i++)
            {
                var tempCircle = Instantiate(unit, unitParent.transform);
                _spawnedUnitPool.Add(tempCircle);
                tempCircle.DisableSelf();
            }
        }

        private void ActivateCircle(int index)
        {
            if (_activeCircles != null && _activeCircles.Count > index)
            {
                if (_spanController.GetBackwardStatus())
                {
                    var temp = _spanController.GetRoundIndex() - index;
                    _activeCircles[index].ConfigureUI(temp);
                }
                else
                {
                    _activeCircles[index].ConfigureUI(index+1);
                }
            }
        }

        public void Exit()
        {
            if (_displayingQuestions != null)
            {
                StopCoroutine(_displayingQuestions);
            }
            ResetPreviousCircles();
        }

        public void SwitchNextState()
        {
            DisableUIElements();
            _spanController.SetCurrentDisplayedQuestions(_currentQuestions);
            if (_spanController.GetBackwardStatus())
            {
                RotateCircles(() =>
                {
                    _spanController.SwitchState();
                });
            }
            else
            {
                _spanController.SwitchState();
            }
        }

        public void TryShowStateTutorial()
        {
            var targets = new List<GameObject>()
            {
                questionFieldParent.gameObject
            };

            var dictionary = new Dictionary<GameObject, TutorialStep>().CreateFromLists(targets, steps);
            _spanController.TriggerStateTutorial(dictionary, false, ShowQuestion);
        }

        public void EnableUIElements()
        {
            questionFieldParent.gameObject.SetActive(true);
            unitParent.gameObject.SetActive(true);
        }

        public void DisableUIElements()
        {
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = "";
            questionBox.sprite = null;
            questionBox.enabled = false;
            questionFieldParent.gameObject.SetActive(false);
        }

        private void RotateCircles(Action onComplete)
        {
            unitParent.transform.DORotate(new Vector3(0, 0, 180), .5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        private void ResetPreviousCircles()
        {
            foreach (var circle in _activeCircles)
            {
                circle.ResetSelf();
            }
            
            _activeCircles.Clear();
        }

        private UnitCircle GetAvailableUnitCircle()
        {
            foreach (var circle in _spawnedUnitPool)
            {
                if (!circle.isActiveAndEnabled)
                {
                    return circle;
                }
            }

            throw new Exception("No available unit circle");
            return null;
        }
    }
}
