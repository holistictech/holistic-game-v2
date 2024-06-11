using DG.Tweening;
using UnityEngine;
using Utilities;

namespace Spans.Skeleton.FeedbackState
{
    public class NBackFeedbackState : SpanFeedbackState
    {
        [SerializeField] private AudioClip correctClip;
        [SerializeField] private AudioClip wrongClip;

        private NBack.NBack _nBackController;

        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                _nBackController = controller.GetComponent<NBack.NBack>();
                base.Enter(controller);
            }
        }
        
        
        protected override void PlayEffects()
        {
            if (_nBackController.IsEmptyRound())
            {
                SwitchNextState();
            }
            else
            {
                if (spanController.IsAnswerCorrect())
                {
                    AudioManager.Instance.PlayAudioClip(correctClip);
                    ConfigureProgressBar();
                }
                else
                {
                    AudioManager.Instance.PlayAudioClip(wrongClip);
                    progressBar = StartCoroutine(AnimateProgressBar(0, .3f));
                }

                DOVirtual.DelayedCall(1f, SwitchNextState);
            }
            
        }
    }
}
