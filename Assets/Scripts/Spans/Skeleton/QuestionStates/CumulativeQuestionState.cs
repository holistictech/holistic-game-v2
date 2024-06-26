using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables.QuestionSystem;
using Scriptables.Tutorial;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spans.Skeleton.QuestionStates
{
    public class CumulativeQuestionState : SpanQuestionState
    {
        [SerializeField] private List<TutorialStep> steps;
        [SerializeField] private Image questionFieldParent;
        [SerializeField] private Image questionBox;
        
        private List<Question> _spanObjects = new List<Question>(); 
        private List<Question> _currentQuestions = new List<Question>();

        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                base.Enter(controller);
            }

            if (_spanObjects.Count == 0 || currentQuestionIndex >= _spanObjects.Count)
            {
                _spanObjects = spanController.GetSpanObjects();
                currentQuestionIndex = 0;
            }

            EnableUIElements();
            SetCircleUI(spanController.GetRoundIndex());
            ShowQuestion();
        }
        
        public override void ShowQuestion()
        {
            _currentQuestions = new List<Question>();
            displayingQuestions = StartCoroutine(ShowNumbers());
        }
        
        
        private bool _isInitial;
        private IEnumerator ShowNumbers()
        {
            //for (int i = 0; i < spanController.GetRoundIndex(); i++)
            //{
                var question = _spanObjects[currentQuestionIndex];
                questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{question.GetQuestionItem()}";
                ActivateCircle(currentQuestionIndex == 0 ? currentQuestionIndex : currentQuestionIndex + 1, 1f);
                questionBox.enabled = false;
                _currentQuestions.Add(question);
                currentQuestionIndex++;
                yield return new WaitForSeconds(1f);
                questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"";
                yield return new WaitForSeconds(1f);
            //}
            
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
        
        public override void EnableUIElements()
        {
            questionFieldParent.gameObject.SetActive(true);
            unitParent.gameObject.SetActive(true);
        }

        public override void DisableUIElements()
        {
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"";
            questionBox.sprite = null;
            questionBox.enabled = false;
            questionFieldParent.gameObject.SetActive(false);
        }
    }
}
