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
        [SerializeField] private Image recordRing;
        [SerializeField] private GameObject answerPopup;
        [SerializeField] private TextMeshProUGUI givenAnswerField;

        private float _maxTime;
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

        public override void PlayTimer(float duration)
        {
            timer.StartTimer(duration, StopRecording);
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
            micButton.interactable = false;
            AnimateRecordRing();
        }

        private void AnimateRecordRing()
        {
            recordRing.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }

        private async void StopRecording()
        {
            string detectedAnswer = await speechRecognition.EndRecording();
            List<string> answerList = detectedAnswer.Split(" ").ToList();
            spanController.SetDetectedAnswers(answerList);
            answerPopup.gameObject.SetActive(true);
            givenAnswerField.text = $"{detectedAnswer}";
            recordRing.DOKill();
            DOVirtual.DelayedCall(1f, SwitchNextState);
            micButton.interactable = true;
        }

        private void StopTemporarily()
        {
            speechRecognition.EndRecording();
            micButton.interactable = true;
            recordRing.DOKill();
        }

        public override void EnableUIElements()
        {
            micButton.gameObject.SetActive(true);
            recordRing.gameObject.SetActive(true);
            base.EnableUIElements();
        }

        public override void DisableUIElements()
        {
            micButton.gameObject.SetActive(false);
            answerPopup.gameObject.SetActive(false);
            recordRing.gameObject.SetActive(false);
            givenAnswerField.text = "";
            base.DisableUIElements();
        }

        public override void AddListeners()
        {
            micButton.onClick.AddListener(StartRecording);
            confirmButton.onClick.AddListener(StopRecording);
            revertButton.onClick.AddListener(StopTemporarily);
        }

        public override void RemoveListeners()
        {
            micButton.onClick.RemoveAllListeners();
            confirmButton.onClick.RemoveListener(StopRecording);
            revertButton.onClick.RemoveListener(StopTemporarily);
        }
    }
}
