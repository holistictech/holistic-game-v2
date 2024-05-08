using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Samples.Whisper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spans.Skeleton.AnswerStates
{
    public class VoiceAnswerState : SpanAnswerState
    {
        [SerializeField] private Whisper speechRecognition;
        [SerializeField] private Button micButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private GameObject answerPopup;
        [SerializeField] private TextMeshProUGUI givenAnswerField;

        private int _maxTime;
        private Coroutine _timer;
        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                spanController = controller;
                _maxTime = spanController.GetRoundTime();
            }

            AddListeners();
            EnableUIElements();
            PlayTimer(_maxTime);
        }

        public override void PlayTimer(float maxTime)
        {
            timer.StartTimer(maxTime, StopRecording);
        }
        
        public override void SwitchNextState()
        {
            spanController.SwitchState();
        }

        public override void Exit()
        {
            RemoveListeners();

            if (_timer != null)
            {
                StopCoroutine(_timer);
            }
            
            RemoveListeners();
            DisableUIElements();
        }

        private void StartRecording()
        {
            speechRecognition.StartRecording();
            stopButton.onClick.AddListener(StopRecording);
        }

        private async void StopRecording()
        {
            string detectedAnswer = await speechRecognition.EndRecording();
            List<string> answerList = detectedAnswer.Split(" ").ToList();
            spanController.SetDetectedAnswers(answerList);
            answerPopup.gameObject.SetActive(true);
            givenAnswerField.text = $"{detectedAnswer}";
            DOVirtual.DelayedCall(1f, SwitchNextState);
        }

        private void StopTemporarily()
        {
            speechRecognition.EndRecording();
        }

        public override void EnableUIElements()
        {
            micButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
            stopButton.gameObject.SetActive(true);
            base.EnableUIElements();
        }

        public override void DisableUIElements()
        {
            micButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(false);
            answerPopup.gameObject.SetActive(false);
            givenAnswerField.text = "";
            base.DisableUIElements();
        }

        private void AddListeners()
        {
            micButton.onClick.AddListener(StartRecording);
            stopButton.onClick.AddListener(StopRecording);
            cancelButton.onClick.AddListener(StopTemporarily);
        }

        private void RemoveListeners()
        {
            micButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveListener(StopTemporarily);
        }
    }
}
