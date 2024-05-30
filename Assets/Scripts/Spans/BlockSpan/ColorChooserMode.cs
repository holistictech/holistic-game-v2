using DG.Tweening;
using Interfaces;
using UI.CorsiBlockTypes;

namespace Spans.BlockSpan
{
    public class ColorChooserMode : IBlockSpanStrategy
    {
        public void HighlightBlock(AdaptableBlock targetBlock)
        {
            var image = targetBlock.GetBlockImage();
            var color = targetBlock.GetHighlightColor();
            image.DOColor(color, 1f).SetEase(Ease.Flash).OnComplete(targetBlock.ResetUI);
        }

        public void CheckAnswer()
        {
            throw new System.NotImplementedException();
        }
    }
}
