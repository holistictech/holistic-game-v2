using DG.Tweening;
using Interfaces;
using Spans.BlockSpan;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CorsiBlockTypes
{
    public class AdaptableBlock : CorsiBlock
    {
        [SerializeField] private Image itemImage;
        private Sprite _itemSprite;
        private IBlockSpanStrategy _currentStrategy;
        
        public void SetStrategy(IBlockSpanStrategy strategy)
        {
            _currentStrategy = strategy;
        }
        
        public override void AnimateSelf()
        {
            if (_currentStrategy is RegularMode)
            {
                base.AnimateSelf();
            }
            else
            {
                _currentStrategy.HighlightBlock(this);
            }
        }

        public override void ResetUI()
        {
            itemImage.enabled = false;
            blockImage.color = new Color(1, 1, 1, 1);
        }

        protected override void SetSelected()
        {
            if (_currentStrategy is RegularMode)
            {
                base.SetSelected();
            }
            else
            {
                var selection = OptionPicker.GetCurrentSelection();
                _currentStrategy.SetBlockSelected(this, selection);
                AppendSelf(selection);
            }
        }
        
        public Image GetBlockImage()
        {
            return blockImage;
        }

        public Image GetItemImage()
        {
            return itemImage;
        }

        public Color GetHighlightColor()
        {
            return (Color)blockQuestion.GetQuestionItem();
        }

        public Sprite GetBasketItem()
        {
            return (Sprite)blockQuestion.GetQuestionItem();
        }
    }
}
