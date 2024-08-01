using DG.Tweening;
using UnityEngine;
using Utilities;

namespace UI.CorsiBlockTypes
{
    public class XylophoneBlock : CorsiBlock
    {
        protected override void SetSelected()
        {
            blockImage.DOColor(highlightColor, .2f).SetEase(Ease.Flash).OnComplete(ResetUI);
            AudioManager.Instance.PlayAudioClip((AudioClip)blockQuestion.GetQuestionItem());
            AppendSelf(blockQuestion);
        }
    }
}
