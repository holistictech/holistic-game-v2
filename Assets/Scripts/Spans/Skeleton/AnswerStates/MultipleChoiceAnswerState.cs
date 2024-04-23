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
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private Choice choicePrefab;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button revertButton;

        private List<Choice> _choicePool = new List<Choice>();
        private List<Question> _givenAnswers = new List<Question>();
        private List<Choice> _selectedChoices = new List<Choice>(); 
        private SpanController _spanController;

        private Coroutine _timer;
        private float _maxTime;
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
                _timer = StartCoroutine(PlayTimer(_maxTime));
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
            }
        }

        private void CalculateDynamicCellSize()
        {
            var width = gridLayoutGroup.GetComponent<RectTransform>().rect.width;
            float available = width - (gridLayoutGroup.padding.left + gridLayoutGroup.padding.right) -
                              (3 * gridLayoutGroup.spacing.x);
            float cellWidth = available / 4;
            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellWidth);
        }
        
        private IEnumerator PlayTimer(float maxTime)
        {
            if (_spanController.GetTutorialStatus())
            {
                StopCoroutine(_timer);
            }
            
            timerBar.maxValue = maxTime;
            float currentTime = maxTime;

            while (currentTime > 0)
            {
                timerBar.value = Mathf.Lerp(timerBar.value, currentTime, Time.deltaTime * 10);
                currentTime -= Time.deltaTime;
                yield return null;
            }

            timerBar.value = 0f;
            SwitchNextState();
        }

        public override void SwitchNextState()
        {
            _spanController.SetSelectedAnswers(_givenAnswers);
            _spanController.SwitchState();
        }

        public override void Exit()
        {
            DisableUIElements();
            RemoveListeners();
            if (_timer != null)
            {
                StopCoroutine(_timer);
            }
        }
        
        public override void TryShowStateTutorial()
        {
            List<GameObject> targets = new List<GameObject>()
            {
                gridLayoutGroup.gameObject,
                revertButton.gameObject,
                confirmButton.gameObject,
                timerBar.gameObject
            };
            var dictionary = new Dictionary<GameObject, TutorialStep>().CreateFromLists(targets, GetTutorialSteps());
            _spanController.TriggerStateTutorial(dictionary, () =>
            {
                _spanController.TriggerTutorialField("Şimdi sıra sende!");
            });
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
        }

        public void AppendGivenAnswers(Question question, Choice choice)
        {
            _givenAnswers.Add(question);
            _selectedChoices.Add(choice);
        }

        private void ResetGivenAnswers()
        {
            _givenAnswers.Remove(_givenAnswers[^1]);
            _selectedChoices[^1].ResetUI();
            _selectedChoices.Remove(_selectedChoices[^1]);
        }

        private void AddListeners()
        { 
            revertButton.onClick.AddListener(ResetGivenAnswers);
            confirmButton.onClick.AddListener(SwitchNextState);
        }

        private void RemoveListeners()
        {
            revertButton.onClick.RemoveListener(ResetGivenAnswers);
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
