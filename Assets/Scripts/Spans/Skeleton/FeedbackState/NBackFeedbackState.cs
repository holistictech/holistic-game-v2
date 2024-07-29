using DG.Tweening;
using UnityEngine;
using Utilities;

namespace Spans.Skeleton.FeedbackState
{
    public class NBackFeedbackState : SpanFeedbackState
    {
        [SerializeField] private AudioClip correctClip;
        [SerializeField] private AudioClip wrongClip;

        public override void Enter(SpanController controller)
        {
            Debug.Log("got in feedback state");
            if (spanController == null)
            {
                base.Enter(controller);
            }
            else
            {
                PlayEffects();
            }
            
            EnableUIElements();
        }
        
        
        protected override void PlayEffects()
        {
            if (spanController.IsEmptyRound())
            {
                SwitchNextState();
                Debug.Log("Switching from feedback state without checking answer is correct or not");
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
