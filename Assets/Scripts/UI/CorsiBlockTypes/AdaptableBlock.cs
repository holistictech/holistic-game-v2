using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.BlockSpan;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CorsiBlockTypes
{
    public class AdaptableBlock : CorsiBlock
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private Sprite basketSprite;
        private Sprite _itemSprite;
        private IBlockSpanStrategy _currentStrategy;
        
        public void SetStrategy(IBlockSpanStrategy strategy)
        {
            _currentStrategy = strategy;
            if (_currentStrategy is ItemChooserMode)
            {
                blockImage.sprite = basketSprite;
            }
        }
        
        public override void AnimateSelf()
        {
            _currentStrategy.HighlightBlock(this);
        }

        public override void ResetUI()
        {
            itemImage.enabled = false;
            blockImage.color = new Color(1, 1, 1, 1);
            blockButton.transition = Selectable.Transition.ColorTint;
            blockButton.interactable = true;
        }

        protected override void SetSelected()
        {
            Question question; 
            if (_currentStrategy is RegularMode)
            {
                question = blockQuestion;
            }
            else
            {
                question = OptionPicker.GetCurrentSelection();
                blockQuestion.SetQuestionItem(question.GetQuestionItem());
                question = blockQuestion;
            }
            
            _currentStrategy.SetBlockSelected(this, question);
            blockButton.transition = Selectable.Transition.None;
            blockButton.interactable = false;
            AppendSelf(question);
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
            return _currentStrategy is RegularMode ? highlightColor : (Color)blockQuestion.GetQuestionItem();
        }

        public Sprite GetBasketItem()
        {
            return (Sprite)blockQuestion.GetQuestionItem();
        }
    }
}
