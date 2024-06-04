using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables.QuestionSystem;
using Scriptables.QuestionSystem.Images;
using Scriptables.Tutorial;
using Spans.CumulativeSpan;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Spans.Skeleton.QuestionStates
{
    public class CommonQuestionState : SpanQuestionState
    {
        [SerializeField] private List<TutorialStep> steps;
        [SerializeField] private Image questionFieldParent;
        [SerializeField] private Image questionBox;
        
        private List<Question> _spanObjects; 
        private List<Question> _currentQuestions = new List<Question>();
        private bool _isInitial;

        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                base.Enter(controller);
                _isInitial = true;
            }
            _spanObjects = spanController.GetSpanObjects();
            EnableUIElements();
            SetCircleUI(spanController.GetRoundIndex());
            if (spanController.GetTutorialStatus())
            {
                TryShowStateTutorial();
            }
            else
            {
                ShowQuestion();
            }
            StatisticsHelper.IncrementDisplayedQuestionCount();
        }

        public override void ShowQuestion()
        {
            _currentQuestions = new List<Question>();
            
            if (!spanController.GetCumulativeStatus())
            {
                currentQuestionIndex = 0;
            }
            
            /*if (currentQuestionIndex + spanController.GetRoundIndex() >= _spanObjects.Count && !spanController.GetCumulativeStatus())
            {
                _spanObjects = spanController.GetSpanObjects();
            }*/
            
            var question = _spanObjects[currentQuestionIndex];
            if (question is NumberQuestion)
            {
                displayingQuestions = StartCoroutine(ShowNumbers());
            } else if (question is ImageQuestion)
            {
                displayingQuestions = StartCoroutine(ShowImages());
            } else if (question is ClipQuestion)
            {
                displayingQuestions = StartCoroutine(PlayClips());
            }
        }

        private IEnumerator ShowNumbers()
        {
            for (int i = 0; i < spanController.GetRoundIndex(); i++)
            {
                if (currentQuestionIndex >= _spanObjects.Count)
                {
                    break;
                }
                var question = _spanObjects[currentQuestionIndex];
                questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{question.GetQuestionItem()}";
                ActivateCircle(currentQuestionIndex);
                questionBox.enabled = false;
                _currentQuestions.Add(question);
                currentQuestionIndex++;
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
                if (currentQuestionIndex >= _spanObjects.Count)
                {
                    break;
                }
                var question = _spanObjects[currentQuestionIndex];
                questionBox.sprite = (Sprite)question.GetQuestionItem();
                questionBox.enabled = true;
                ActivateCircle(currentQuestionIndex);
                _currentQuestions.Add(question);
                currentQuestionIndex++;
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
                if (currentQuestionIndex >= _spanObjects.Count)
                {
                    break;
                }
                var question = _spanObjects[currentQuestionIndex];
                AudioClip clip = (AudioClip)question.GetQuestionItem();
                AudioManager.Instance.PlayAudioClip(clip);
                ActivateCircle(currentQuestionIndex);
                _currentQuestions.Add(question);
                currentQuestionIndex++;
                yield return new WaitForSeconds(clip.length + 1f);
            } 
            
            DOVirtual.DelayedCall(1f, SwitchNextState);
        }
        

        public override void Exit()
        {
            if (displayingQuestions != null)
            {
                StopCoroutine(displayingQuestions);
            }
            ResetPreviousCircles();
        }

        public void ConfigureDisplayedQuestions()
        {
            spanController.SetCurrentDisplayedQuestions(_currentQuestions);
        }

        public override void SwitchNextState()
        {
            DisableUIElements();
            ConfigureDisplayedQuestions();
            if (spanController.GetBackwardStatus())
            {
                RotateCircles(-180, () =>
                {
                    spanController.SwitchState();
                });
            }
            else
            {
                spanController.SwitchState();
            }
        }

        public override void TryShowStateTutorial()
        {
            var targets = new List<GameObject>()
            {
                questionFieldParent.gameObject
            };

            var dictionary = new Dictionary<GameObject, TutorialStep>().CreateFromLists(targets, steps);
            spanController.TriggerStateTutorial(dictionary, false, ShowQuestion);
        }

        public override void EnableUIElements()
        {
            questionFieldParent.gameObject.SetActive(true);
            unitParent.gameObject.SetActive(true);
        }

        public override void DisableUIElements()
        {
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = "";
            questionBox.sprite = null;
            questionBox.enabled = false;
            questionFieldParent.gameObject.SetActive(false);
        }
    }
}
