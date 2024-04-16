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

        private List<Choice> _spawnedChoices = new List<Choice>();
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
            }

            AddListeners();
            EnableUIElements();
            
            SpawnChoices();
            _timer = StartCoroutine(PlayTimer(_maxTime));
        }

        private void SpawnChoices()
        {
            var choices = _spanController.GetChoices();

            foreach (var question in choices)
            {
                var tempChoice = Instantiate(choicePrefab, gridLayoutGroup);
                tempChoice.ConfigureUI(question, this);
                _spawnedChoices.Add(tempChoice);
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
            _spanController.SwitchState();
        }

        public override void Exit()
        {
            DestroySpawnedChoices();
            DisableUIElements();
            RemoveListeners();
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
        }

        private void DestroySpawnedChoices()
        {
            foreach (var spawnedChoice in _spawnedChoices)
            {
                Destroy(spawnedChoice.gameObject);
            }
        }

        public void AppendGivenAnswers(Question question)
        {
            _givenAnswers.Add(question);
        }

        private void ResetGivenAnswers()
        {
            _givenAnswers.Clear();
            foreach (var choice in _spawnedChoices)
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
    }
}
