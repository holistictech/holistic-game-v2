using System.Collections.Generic;
using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using UI.CorsiBlockTypes;
using UnityEngine;

namespace Spans.BlockSpan
{
    public class ItemChooserMode : IBlockSpanStrategy
    {
        public void HighlightBlock(AdaptableBlock targetBlock)
        {
            var itemImage = targetBlock.GetItemImage();
            var itemSprite = targetBlock.GetBasketItem();
            itemImage.sprite = itemSprite;
            itemImage.enabled = true;
            itemImage.DOColor(new Color(1, 1, 1, 1), 0.3f).OnComplete(() =>
            {
                itemImage.transform.DOPunchScale(new Vector3(.2f, .2f, .2f), .7f).SetEase(Ease.OutBack)
                    .SetLoops(2, LoopType.Restart).OnComplete(targetBlock.ResetUI);
            });
        }

        public void SetBlockSelected(AdaptableBlock block, Question selection)
        {
            var itemImage = block.GetItemImage();
            itemImage.sprite = (Sprite)selection.GetQuestionItem();
            itemImage.enabled = true;
        }

        public bool CheckAnswer(List<Question> displayed, List<Question> given)
        {
            if (given.Count == 0 || given.Count != displayed.Count)
            {
                return false;
            }
            
            var displayedItems = new HashSet<object>();
            foreach (var question in displayed)
            {
                displayedItems.Add(question.GetQuestionItem());
            }
            
            foreach (var question in given)
            {
                if (!displayedItems.Contains(question.GetQuestionItem()))
                {
                    return false;
                }
            }

            return true;
        }

        public List<Question> GetCorrectQuestions(List<Question> allQuestions, int count = -1)
        {
            var selectedQuestions = new List<Question>();
            for (int i = 0; i < count; i++)
            {
                var randomIndex = Random.Range(0, allQuestions.Count);
                var randomQuestion = allQuestions[randomIndex];
                while (selectedQuestions.Contains(randomQuestion))
                {
                    randomIndex = Random.Range(0, allQuestions.Count);
                    randomQuestion = allQuestions[randomIndex];
                }
                
                selectedQuestions.Add(randomQuestion);
            }

            return selectedQuestions;
        }
    }
}
