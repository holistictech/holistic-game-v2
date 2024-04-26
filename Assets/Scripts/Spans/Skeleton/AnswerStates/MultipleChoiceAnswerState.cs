using System;
using System.Collections;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Scriptables.Tutorial;
using Tutorial;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;
using Vector2 = UnityEngine.Vector2;

namespace Spans.Skeleton.AnswerStates
{
    public class MultipleChoiceAnswerState : SpanAnswerState
    {
        [SerializeField] private List<TutorialStep> gridStep;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private Choice choicePrefab;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button revertButton;

        private List<Choice> _choicePool = new List<Choice>();
        private List<Question> _givenAnswers = new List<Question>();
        private List<Choice> _activeChoices = new List<Choice>();
        private List<Choice> _selectedChoices = new List<Choice>(); 
        private SpanController _spanController;

        private Coroutine _timer;
        private Coroutine _tutorialHighlight;
        private float _maxTime;

        public static event Action<int> OnChoiceSelected;
        public override void Enter(SpanController controller)
        {
            if (_spanController == null)
            {
                _spanController = controller;
                _maxTime = _spanController.GetRoundTime();
                SpawnChoices();
            }

            AddListeners();
            EnableUIElements();
            SetChoiceUI();
            
            if (_spanController.GetTutorialStatus())
            {
                TryShowStateTutorial();
            }
            else
            {
                PlayTimer(_maxTime);
            }
        }

        private void SpawnChoices()
        {
            for (int i = 0; i < 18; i++)
            {
                var tempChoice = Instantiate(choicePrefab, gridLayoutGroup.transform);
                _choicePool.Add(tempChoice);
            }
        }

        private void SetChoiceUI()
        {
            _givenAnswers.Clear();
            var choices = _spanController.GetChoices();
            CalculateDynamicCellSize();
            
            foreach (var choice in choices)
            {
                var temp = GetAvailableChoice();
                temp.ConfigureUI(choice, this);
                _activeChoices.Add(temp);
            }
        }

        private void CalculateDynamicCellSize()
        {
            SetConstraintCount();
            RectTransform gridRectTransform = gridLayoutGroup.GetComponent<RectTransform>();

            var rect = gridRectTransform.rect;
            float width = rect.width;
            float height = rect.height;
    
            float availableWidth = width - (gridLayoutGroup.padding.left + gridLayoutGroup.padding.right) - ((gridLayoutGroup.constraintCount - 1) * gridLayoutGroup.spacing.x);
            float availableHeight = height - (gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom) - ((gridLayoutGroup.constraintCount - 1) * gridLayoutGroup.spacing.y);
    
            float cellWidth = availableWidth / gridLayoutGroup.constraintCount;
            float cellHeight = availableHeight / gridLayoutGroup.constraintCount;
            float cellSize = Mathf.Min(cellWidth, cellHeight);
            gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        }


        private void SetConstraintCount()
        {
            var index = _spanController.GetRoundIndex();
            if (index == 2)
            {
                gridLayoutGroup.constraintCount = 2;
            }else
            {
                gridLayoutGroup.constraintCount = 3;
            }
        }
        
        public override void PlayTimer(float maxTime)
        {
            if (_spanController.GetTutorialStatus()) return;
            timer.StartTimer(maxTime, SwitchNextState);
        }

        public override void SwitchNextState()
        {
            if (_spanController.GetTutorialStatus())
            {
                _spanController.ClearTutorialHighlights();
                _spanController.SetTutorialCompleted();
            }
            OnChoiceSelected?.Invoke(0);
            _spanController.SetSelectedAnswers(_givenAnswers);
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
        
        public override void TryShowStateTutorial()
        {
            List<GameObject> secondPart = new List<GameObject>()
            {
                timer.gameObject,
                revertButton.gameObject
            };
            var secondPartDict = new Dictionary<GameObject, TutorialStep>().CreateFromLists(secondPart, GetTutorialSteps());
            _spanController.TriggerStateTutorial(secondPartDict, true,() =>
            {
                List<GameObject> firstPart = new List<GameObject>()
                {
                    gridLayoutGroup.gameObject,
                };
                var dictionary = new Dictionary<GameObject, TutorialStep>().CreateFromLists(firstPart, GetGridStep());
                _spanController.TriggerStateTutorial(dictionary, false, () =>
                {
                    _spanController.TriggerTutorialField("Şimdi sıra sende!");
                    _tutorialHighlight = StartCoroutine(HighlightObjectsForTutorial());
                });
            });
        }

        private void TryShowSecondPartTutorial()
        {
            List<GameObject> secondPart = new List<GameObject>()
            {
                revertButton.gameObject,
                timer.gameObject
            };
            var secondPartDict = new Dictionary<GameObject, TutorialStep>().CreateFromLists(secondPart, GetTutorialSteps());
            _spanController.TriggerStateTutorial(secondPartDict, true, () =>
            {
                _spanController.SetTutorialCompleted();
                SwitchNextState();
            });
        }

        private int _highlightIndex = 0;
        private int _lastIndex = -1;
        private bool _waitInput;
        private IEnumerator HighlightObjectsForTutorial()
        {
            List<Question> currentQuestions = _spanController.GetCurrentQuestions();
            while (_highlightIndex < currentQuestions.Count)
            {
                if (_highlightIndex != _lastIndex)
                {
                    var targetRect = GetAppropriateChoice(currentQuestions[_highlightIndex]);
                    _spanController.HighlightTarget(targetRect, gridLayoutGroup.GetComponent<RectTransform>(), true, gridLayoutGroup.cellSize.x);
                    _lastIndex = _highlightIndex;
                    _waitInput = true;
                    yield return new WaitUntil(() => !_waitInput);
                }       
            }
            _spanController.HighlightTarget(confirmButton.GetComponent<RectTransform>(), GetComponent<RectTransform>(), true, 150f);
        }

        private RectTransform GetAppropriateChoice(Question question)
        {
            foreach (var choice in _activeChoices)
            {
                var config = choice.GetAssignedQuestionConfig();
                if (config.GetQuestionItem() == question.GetQuestionItem())
                {
                    return choice.GetComponent<RectTransform>();
                }
            }

            throw new ArgumentException("Could not find such question in spawned choices");
            return null;
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
            DisableSpawnedChoices();
        }

        private void DisableSpawnedChoices()
        {
            foreach (var spawnedChoice in _choicePool)
            {
                spawnedChoice.DisableSelf();
            }
            _activeChoices.Clear();
        }

        public void AppendGivenAnswers(Question question, Choice choice)
        {
            _givenAnswers.Add(question);
            if (_spanController.GetTutorialStatus())
            {
                _highlightIndex++;
                _waitInput = false;
            }
            OnChoiceSelected?.Invoke(_givenAnswers.Count);
            _selectedChoices.Add(choice);
        }

        private void RevertLastAnswer()
        {
            if (_givenAnswers.Count == 0) return;
            OnChoiceSelected?.Invoke(-(_givenAnswers.Count));
            _givenAnswers.Remove(_givenAnswers[^1]);
            _selectedChoices[^1].ResetUI();
            _selectedChoices.Remove(_selectedChoices[^1]);
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

        private Choice GetAvailableChoice()
        {
            for (int i = 0; i < _choicePool.Count; i++)
            {
                if (!_choicePool[i].isActiveAndEnabled)
                {
                    return _choicePool[i];
                }
            }

            throw new Exception("No available choice. Need to spawn");
            return null;
        }
    }
}
