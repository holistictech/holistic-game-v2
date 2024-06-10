using DG.Tweening;
using UnityEngine;
using Utilities;

namespace Spans.Skeleton.FeedbackState
{
    public class NBackFeedbackState : SpanFeedbackState
    {
        [SerializeField] private AudioClip correctClip;
        [SerializeField] private AudioClip wrongClip;
        protected override void PlayEffects()
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
