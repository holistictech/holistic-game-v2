using System;
using System.Collections;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Spans.Skeleton.AnswerStates
{
    public class MultipleChoiceAnswerState : SpanAnswerState
    {
        [SerializeField] private RectTransform gridLayoutGroup;
        [SerializeField] private Choice choicePrefab;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button resetButton;

        private List<Choice> _choicePool = new List<Choice>();
        private List<Question> _givenAnswers = new List<Question>();
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
            _timer = StartCoroutine(PlayTimer(_maxTime));
        }

        private void SpawnChoices()
        {
            for (int i = 0; i < 18; i++)
            {
                var tempChoice = Instantiate(choicePrefab, gridLayoutGroup);
                _choicePool.Add(tempChoice);
            }
        }

        private void SetChoiceUI()
        {
            _givenAnswers.Clear();
            var choices = _spanController.GetChoices();
            
            foreach (var choice in choices)
            {
                var temp = GetAvailableChoice();
                temp.ConfigureUI(choice, this);
            }
        }
        
        private IEnumerator PlayTimer(float maxTime)
        {
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
        
        public override void EnableUIElements()
        {
            base.EnableUIElements();
            gridLayoutGroup.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(true);
            resetButton.gameObject.SetActive(true);
        }

        public override void DisableUIElements()
        {
            base.DisableUIElements();
            gridLayoutGroup.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(false);
            resetButton.gameObject.SetActive(false);
            DisableSpawnedChoices();
        }

        private void DisableSpawnedChoices()
        {
            foreach (var spawnedChoice in _choicePool)
            {
                spawnedChoice.DisableSelf();
            }
        }

        public void AppendGivenAnswers(Question question)
        {
            _givenAnswers.Add(question);
        }

        private void ResetGivenAnswers()
        {
            _givenAnswers.Clear();
            foreach (var choice in _choicePool)
            {
                choice.ResetUI();
            }
        }

        private void AddListeners()
        { 
            resetButton.onClick.AddListener(ResetGivenAnswers);
            confirmButton.onClick.AddListener(SwitchNextState);
        }

        private void RemoveListeners()
        {
            resetButton.onClick.RemoveListener(ResetGivenAnswers);
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
