using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables.QuestionSystem;
using Scriptables.Tutorial;
using Spans.CumulativeSpan;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Spans.Skeleton.AnswerStates
{
    public class MultipleChoiceAnswerState : SpanAnswerState
    {
        [SerializeField] private List<TutorialStep> gridStep;
        [SerializeField] private GridUIHelper gridHelper;
        [SerializeField] protected GridLayoutGroup gridLayoutGroup;
        
        private Coroutine _timer;
        private Coroutine _tutorialHighlight;
        
        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                base.Enter(controller);
            }

            maxTime = spanController.GetRoundTime();
            EnableUIElements();
            SetChoiceUI();
            
            if (spanController.GetTutorialStatus())
            {
                timer.EnableSelf();
                TryShowStateTutorial();
            }
            else
            {
                //TryShowHelpers();
                PlayTimer(maxTime);
            }
        }

        public virtual void SetChoiceUI()
        {
            gridHelper.SetConstraintCount(spanController.GetRoundIndex(), spanController.GetCumulativeStatus());
            var choices = spanController.GetChoices();
            var unitCircles = spanController.GetActiveCircles();
            gridHelper.SetActiveCircles(unitCircles);
            gridHelper.SetStartingIndex(spanController.GetStartingUnitIndex());
            gridHelper.ConfigureChoices(choices, this);
        }

        protected override void PlayTimer(float duration)
        {
            timer.StartTimer(duration, SwitchNextState);
        }

        public override void SwitchNextState()
        {
            if (spanController.GetTutorialStatus())
            {
                spanController.ClearTutorialHighlights();
            }
            spanController.SetSelectedAnswers(gridHelper.GetGivenAnswers());
            spanController.SwitchState();
        }

        public override void Exit()
        {
            DisableUIElements();
            if (_tutorialHighlight != null)
            {
                StopCoroutine(_tutorialHighlight);
            }
        }

        private void TryShowHelpers()
        {
            if (!spanController.IsHelperTutorialNeeded())
            {
                PlayTimer(maxTime);
                return;
            }
            
            timer.EnableSelf();
            List<GameObject> secondPart = new List<GameObject>()
            {
                timer.gameObject,
                revertButton.gameObject,
                confirmButton.gameObject
            };
            var secondPartDict = new Dictionary<GameObject, TutorialStep>().CreateFromLists(secondPart, GetTutorialSteps());
            spanController.TriggerStateTutorial(secondPartDict, true,() =>
            {
                //PlayTimer(_maxTime);
            });
        }
        
        public override void TryShowStateTutorial()
        {
            List<GameObject> firstPart = new List<GameObject>()
            {
                gridHelper.gameObject,
            };
            var dictionary = new Dictionary<GameObject, TutorialStep>().CreateFromLists(firstPart, GetGridStep());
            spanController.TriggerStateTutorial(dictionary, false, () =>
            {
                //_spanController.TriggerTutorialField("Şimdi sıra sende!");
                _tutorialHighlight = StartCoroutine(HighlightObjectsForTutorial());
            });
        }

        private int _highlightIndex = 0;
        private int _lastIndex = -1;
        private bool _waitInput;
        private IEnumerator HighlightObjectsForTutorial()
        {
            List<Question> currentQuestions = spanController.GetCurrentDisplayedQuestions();
            while (_highlightIndex < currentQuestions.Count)
            {
                if (_highlightIndex != _lastIndex)
                {
                    var targetRect = GetAppropriateChoice(currentQuestions[_highlightIndex]);
                    spanController.HighlightTarget(targetRect, gridHelper.GetComponent<RectTransform>(), true, 170f);
                    _lastIndex = _highlightIndex;
                    _waitInput = true;
                    yield return new WaitUntil(() => !_waitInput);
                }
            }
            AudioManager.Instance.PlayAudioClip(confirmClip);
            spanController.HighlightTarget(confirmButton.GetComponent<RectTransform>(), GetComponent<RectTransform>(), true, 150f);
        }

        private RectTransform GetAppropriateChoice(Question question)
        {
            return gridHelper.GetAppropriateChoice(question);
        }

        private List<TutorialStep> GetGridStep()
        {
            return gridStep;
        }
        
        public override void EnableUIElements()
        {
            base.EnableUIElements();
            gridLayoutGroup.gameObject.SetActive(true);
            timer.EnableSelf();
        }

        public override void DisableUIElements()
        {
            base.DisableUIElements();
            gridLayoutGroup.gameObject.SetActive(false);
            revertButton.gameObject.SetActive(spanController.GetTutorialStatus());
            if (spanController.GetTutorialStatus())
            {
                timer.EnableSelf();
                spanController.SetTutorialHelperObjects(new List<GameObject>
                {
                    timer.gameObject,
                    revertButton.gameObject,
                });
            }
            else
            {
                timer.StopTimer();
            }
            DisableSpawnedChoices();
        }

        private void DisableSpawnedChoices()
        {
            gridHelper.DisableSpawnedChoices();
        }

        public void OnAnswerGiven()
        {
            if (spanController.GetTutorialStatus())
            {
                _highlightIndex++;
                _waitInput = false;
            }
        }

        public override void RevertLastAnswer()
        {
            gridHelper.RevokeLastSelection();
        }

        public int GetUnitIndexUpdateAmount()
        {
            return spanController is NumberExercise ? 2 : 1;
        }

        public bool CanAnimateNextCircle()
        {
            return spanController is not NumberExercise;
        }
    }
}
