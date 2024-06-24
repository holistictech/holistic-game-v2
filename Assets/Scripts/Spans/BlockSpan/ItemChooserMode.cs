using System.Collections.Generic;
using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using UI.CorsiBlockTypes;
using UnityEngine;
using Utilities.Helpers;

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
                    .SetLoops(2, LoopType.Incremental).OnComplete(targetBlock.ResetUI);
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
            
            var displayedList = new List<(object sprite, int index)>();
            var givenList = new List<(object sprite, int index)>();
            
            foreach (var question in displayed)
            {
                var color = question.GetQuestionItem();
                var index = (int)question.GetQuestionItemByType(CommonFields.ButtonType.Shape);
                displayedList.Add((color, index));
            }
            
            foreach (var question in given)
            {
                var color = question.GetQuestionItem();
                var index = (int)question.GetQuestionItemByType(CommonFields.ButtonType.Shape);
                givenList.Add((color, index));
            }
            var matchedItems = new HashSet<(object sprite, int index)>();
            
            foreach (var displayedItem in displayedList)
            {
                bool matchFound = false;
                for (int i = 0; i < givenList.Count; i++)
                {
                    var givenItem = givenList[i];
                    if (!matchedItems.Contains(givenItem) && displayedItem.sprite.Equals(givenItem.sprite) && displayedItem.index.Equals(givenItem.index))
                    {
                        matchFound = true;
                        matchedItems.Add(givenItem);
                        break;
                    }
                }
                if (!matchFound)
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
