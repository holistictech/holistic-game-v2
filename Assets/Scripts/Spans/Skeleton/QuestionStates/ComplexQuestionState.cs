using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using Scriptables.QuestionSystem.Images;
using Scriptables.QuestionSystem.Numbers;
using Scriptables.Tutorial;
using Spans.CumulativeSpan;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Helpers;

namespace Spans.Skeleton.QuestionStates
{
    public class ComplexQuestionState : SpanQuestionState
    {
        [SerializeField] private List<TutorialStep> steps;
        [SerializeField] private Image questionFieldParent;
        [SerializeField] private Image questionBox;

        private List<Question> _spanObjects;
        private List<Question> _currentQuestions = new List<Question>();

        private ComplexSpan.ComplexSpan _complexSpan;
        private IComplexSpanStrategy _currentStrategy;

        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                _complexSpan = controller.GetComponent<ComplexSpan.ComplexSpan>();
                _spanObjects = controller.GetSpanObjects();
                base.Enter(controller);
            }

            _currentStrategy = _complexSpan.GetCurrentStrategy();
            _currentStrategy.EnableRequiredModeElements(this);
            EnableUIElements();
            SetCircleUI(_currentStrategy.GetFixedQuestionCount());
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

        private bool _hasMainPlayed;
        public override void ShowQuestion()
        {
            _currentQuestions = new List<Question>();

            if (currentQuestionIndex >= _spanObjects.Count)
            {
                if (_complexSpan.GetIsMainSpanNeeded() || _hasMainPlayed)
                {
                    _spanObjects = spanController.GetSpanObjects();
                    currentQuestionIndex = 0;
                    _hasMainPlayed = false;
                }
                else
                {
                    _complexSpan.SetMainSpanNeeded(true);
                    _hasMainPlayed = true;
                    SwitchNextState();
                    return;
                }
            }

            var question = _spanObjects[currentQuestionIndex];
            if (question is NumberQuestion)
            {
                displayingQuestions = StartCoroutine(ShowNumber(currentQuestionIndex));
            }
            else if (question is ImageQuestion)
            {
                displayingQuestions = StartCoroutine(ShowImage(question, 0, 1));
            }
            else if (question is ClipQuestion)
            {
                displayingQuestions = StartCoroutine(PlayClip(question, 0));
            }
        }

        private IEnumerator ShowNumber(int index)
        {
            var target = index + 2;
            for (int i = index; i < target; i++)
            {
                var question = _spanObjects[i];
                questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{question.GetQuestionItem()}";
                ActivateCircle(i, 1f);
                questionBox.enabled = false;
                _currentQuestions.Add(question);
                currentQuestionIndex++;
                yield return new WaitForSeconds(1f);
                questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"";
            }
            
            SwitchNextState();
        }

        private IEnumerator ShowImage(Question question, int index, int count)
        {
            for (int i = 0; i < count; i ++)
            {
                questionBox.sprite = (Sprite)question.GetQuestionItem();
                questionBox.enabled = true;
                ActivateCircle(index, 1f);
                _currentQuestions.Add(question);
                currentQuestionIndex++;
                yield return new WaitForSeconds(1f);
                questionBox.enabled = false;
            }
            
            SwitchNextState();
        }

        private IEnumerator PlayClip(Question question, int index)
        {
            AudioClip clip = (AudioClip)question.GetQuestionItem();
            ConfigureClipField(question, index);
            yield return new WaitForSeconds(1f);
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = "";
            yield return new WaitForSeconds(clip.length);

            SwitchNextState();
        }

        private void ConfigureClipField(Question question, int index)
        {
            AudioClip clip = (AudioClip)question.GetQuestionItem();
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{question.GetCorrectText()}";
            AudioManager.Instance.PlayAudioClip(clip);
            ActivateCircle(index, 1f);
            _currentQuestions.Add(question);
            currentQuestionIndex++;
        }

        public override void Exit()
        {
            if (displayingQuestions != null)
            {
                StopCoroutine(displayingQuestions);
            }

            ResetPreviousCircles();
        }

        private void ConfigureDisplayedQuestions()
        {
            spanController.SetCurrentDisplayedQuestions(_currentQuestions);
        }

        public override void SwitchNextState()
        {
            DisableUIElements();
            ConfigureDisplayedQuestions();
            spanController.SwitchState();
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
            unitParent.gameObject.SetActive(true);
        }

        public GameObject GetQuestionField()
        {
            return questionFieldParent.gameObject;
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