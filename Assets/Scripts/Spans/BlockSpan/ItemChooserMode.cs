using DG.Tweening;
using Interfaces;
using UI.CorsiBlockTypes;
using UnityEngine;

namespace Spans.BlockSpan
{
    public class ItemChooserMode : IBlockSpanStrategy
    {
        public void HighlightBlock(AdaptableBlock targetBlock)
        {
            var itemImage = targetBlock.GetBlockImage();
            var itemSprite = targetBlock.GetBasketItem();
            itemImage.sprite = itemSprite;
            itemImage.DOColor(new Color(1, 1, 1, 1), 0.3f).OnComplete(() =>
            {
                itemImage.transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), 0.8f).SetEase(Ease.OutQuad)
                    .SetLoops(2, LoopType.Yoyo).OnComplete(targetBlock.ResetUI);
            });
        }

        public void CheckAnswer()
        {
            throw new System.NotImplementedException();
        }
    }
}
