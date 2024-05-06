using System;
using System.Collections;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Scriptables.Tutorial;
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
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button revertButton;
        
        private SpanController _spanController;
        private Coroutine _timer;
        private Coroutine _tutorialHighlight;
        private float _maxTime;
        
        private int _circleIndex = 1;
        
        public override void Enter(SpanController controller)
        {
            if (_spanController == null)
            {
                _spanController = controller;
                _maxTime = _spanController.GetRoundTime();
            }

            AddListeners();
            EnableUIElements();
            SetChoiceUI();
            
            if (_spanController.GetTutorialStatus())
            {
                timer.EnableSelf();
                TryShowStateTutorial();
            }
            else
            {
                TryShowHelpers();
                //PlayTimer(_maxTime);
            }
        }

        private void SetChoiceUI()
        {
            gridHelper.SetConstraintCount(_spanController.GetRoundIndex(), _spanController.GetCumulativeStatus());
            var choices = _spanController.GetChoices();
            var unitCircles = _spanController.GetActiveCircles();
            gridHelper.SetActiveCircles(unitCircles);
            gridHelper.ConfigureChoices(choices, this);
        }
        
        public override void PlayTimer(float maxTime)
        {
            timer.StartTimer(maxTime, SwitchNextState);
        }

        public override void SwitchNextState()
        {
            if (_spanController.GetTutorialStatus())
            {
                _spanController.ClearTutorialHighlights();
            }
            _spanController.SetSelectedAnswers(gridHelper.GetGivenAnswers());
            _spanController.SwitchState();
        }

        public override void Exit()
        {
            DisableUIElements();
            RemoveListeners();
            timer.StopTimer();
            if (_tutorialHighlight != null)
            {
                StopCoroutine(_tutorialHighlight);
            }
        }

        private void TryShowHelpers()
        {
            if (!_spanController.IsHelperTutorialNeeded())
            {
                PlayTimer(_maxTime);
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
            _spanController.TriggerStateTutorial(secondPartDict, true,() =>
            {
                _spanController.SetHelperTutorialCompleted();
                PlayTimer(_maxTime);
            });
        }
        
        public override void TryShowStateTutorial()
        {
            List<GameObject> firstPart = new List<GameObject>()
            {
                gridHelper.gameObject,
            };
            var dictionary = new Dictionary<GameObject, TutorialStep>().CreateFromLists(firstPart, GetGridStep());
            _spanController.TriggerStateTutorial(dictionary, false, () =>
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
            List<Question> currentQuestions = _spanController.GetCurrentDisplayedQuestions();
            while (_highlightIndex < currentQuestions.Count)
            {
                if (_highlightIndex != _lastIndex)
                {
                    var targetRect = GetAppropriateChoice(currentQuestions[_highlightIndex]);
                    _spanController.HighlightTarget(targetRect, gridHelper.GetComponent<RectTransform>(), true, 170f);
                    _lastIndex = _highlightIndex;
                    _waitInput = true;
                    yield return new WaitUntil(() => !_waitInput);
                }       
            }
            _spanController.HighlightTarget(confirmButton.GetComponent<RectTransform>(), GetComponent<RectTransform>(), true, 150f);
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
            confirmButton.gameObject.SetActive(true);
            revertButton.gameObject.SetActive(true);
        }

        public override void DisableUIElements()
        {
            base.DisableUIElements();
            gridLayoutGroup.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(false);
            revertButton.gameObject.SetActive(false);
            gridHelper.DisableSpawnedChoices();
            timer.StopTimer();

            DisableSpawnedChoices();
        }

        private void DisableSpawnedChoices()
        {
            gridHelper.DisableSpawnedChoices();
        }

        public void OnAnswerGiven()
        {
            if (_spanController.GetTutorialStatus())
            {
                _highlightIndex++;
                _waitInput = false;
            }
        }

        private void RevertLastAnswer()
        {
            gridHelper.RevokeLastSelection();
        }

        private void AddListeners()
        { 
            revertButton.onClick.AddListener(RevertLastAnswer);
            confirmButton.onClick.AddListener(SwitchNextState);
        }

        private void RemoveListeners()
        {
            revertButton.onClick.RemoveListener(RevertLastAnswer);
            confirmButton.onClick.RemoveListener(SwitchNextState);
        }
    }
}
