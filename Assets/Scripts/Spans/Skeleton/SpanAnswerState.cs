using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Samples.Whisper;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Spans.Skeleton
{
    public class SpanAnswerState : MonoBehaviour, ISpanState
    {
        [SerializeField] private Whisper speechRecognition;
        [SerializeField] private Button micButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Slider timerBar;
        private SpanController _spanController;

        private int _maxTime;
        private Coroutine _timer;
        public void Enter(SpanController spanController)
        {
            if (_spanController == null)
            {
                _spanController = spanController;
                _maxTime = _spanController.GetRoundTime();
            }

            AddListeners();
            _timer = StartCoroutine(PlayTimer());
        }

        private IEnumerator PlayTimer()
        {
            timerBar.maxValue = _maxTime;
            for (int i = _maxTime; i > 0; i--)
            {
                timerBar.value = i;
                yield return new WaitForSeconds(1f);
            }
            
            StopRecording();
        }

        public void Exit()
        {
            RemoveListeners();

            if (_timer != null)
            {
                StopCoroutine(_timer);
            }
            
            RemoveListeners();
        }

        public void SwitchNextState()
        {
            _spanController.SwitchState();
        }

        private void StartRecording()
        {
            //@todo: for future development reference, composition may be used for different type of answer detection.
            speechRecognition.StartRecording();
            stopButton.onClick.AddListener(StopRecording);
        }

        private async void StopRecording()
        {
            string detectedAnswer = await speechRecognition.EndRecording();
            List<string> answerList = detectedAnswer.Split(" ").ToList();
            _spanController.SetDetectedAnswers(answerList);
            SwitchNextState();
        }

        private void StopTemporarily()
        {
            speechRecognition.EndRecording();
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
