using System.Collections.Generic;
using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using UI.CorsiBlockTypes;
using UnityEngine;

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

        public List<Question> GetCorrectQuestions(List<Question> allQuestions)
        {
            var selectedQuestions = new List<Question>();
            for (int i = 0; i < 2; i++)
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
