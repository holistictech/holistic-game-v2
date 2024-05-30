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

        public List<Question> GetCorrectQuestions(List<Question> allQuestions)
        {
            var selectedQuestions = new List<Question>();
            for (int i = 0; i < 3; i++)
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
