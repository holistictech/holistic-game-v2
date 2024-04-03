using System.Collections;
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
        [SerializeField] private Slider timerBar;
        private SpanController _spanController;
        private ISpanState _questionState;

        private int _maxTime;
        private Coroutine _timer;
        public void Enter(SpanController spanController)
        {
            if (_spanController == null)
            {
                _spanController = spanController;
                _questionState = _spanController.GetQuestionState();
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
        }

        public void Exit()
        {
            RemoveListeners();

            if (_timer != null)
            {
                StopCoroutine(_timer);
            }
        }

        public void SwitchNextState()
        {
            _spanController.SwitchState();
        }

        private void StartRecording()
        {
            //@todo: for future development reference, composition may be used for different type of answer detection.
        }

        private void AddListeners()
        {
            micButton.onClick.AddListener(StartRecording);
        }

        private void RemoveListeners()
        {
            micButton.onClick.RemoveListener(StartRecording);
        }
    }
}
