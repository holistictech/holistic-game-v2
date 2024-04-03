using UnityEngine;
using UnityEngine.UI;

namespace Spans.Skeleton
{
    public class SpanFeedbackState : MonoBehaviour, ISpanState
    {
        [SerializeField] private ParticleSystem successEffect;
        [SerializeField] private ParticleSystem failEffect;
        [SerializeField] private Slider progressBar;
        private SpanController _spanController;
        public void Enter(SpanController spanController)
        {
            if (_spanController == null)
            {
                _spanController = spanController;
            }
            PlayEffects();
        }

        private void PlayEffects()
        {
            if (_spanController.IsAnswerCorrect())
            {
                successEffect.Play();
                progressBar.maxValue = _spanController.GetRoundIndex();
                progressBar.value += progressBar.maxValue / 4;
            }
            else
            {
                failEffect.Play();
                progressBar.value = 0;
            }
            
            _spanController.SwitchState();
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }

        public void SwitchNextState()
        {
            _spanController.SwitchState();
        }
    }
}
