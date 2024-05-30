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
        private Color _highlightColor;
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

        public void SetColor(Color color)
        {
            _highlightColor = color;
        }

        public Color GetHighlightColor()
        {
            return _highlightColor;
        }

        public Sprite GetBasketItem()
        {
            return _itemSprite;
        }
    }
}
