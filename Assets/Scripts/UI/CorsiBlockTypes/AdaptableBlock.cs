using DG.Tweening;
using Interfaces;
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
            _currentStrategy.HighlightBlock(this);
        }

        public Image GetBlockImage()
        {
            return blockImage;
        }

        public override void ResetUI()
        {
            itemImage.enabled = false;
            blockImage.color = new Color(1, 1, 1, 1);
        }

        public override void SetSelected()
        {
            
        }

        public Color GetHighlightColor()
        {
            //@todo: need of function in prob. answer state which takes current selection from option picker and passes it here. 
            return (Color)blockQuestion.GetQuestionItem();
        }

        public Sprite GetBasketItem()
        {
            return _itemSprite;
        }
    }
}
