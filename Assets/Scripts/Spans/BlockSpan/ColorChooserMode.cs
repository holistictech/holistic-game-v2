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

        public void SetBlockSelected(AdaptableBlock block, Question selection)
        {
            var image = block.GetBlockImage();
            image.color = (Color)selection.GetQuestionItem();
        }

        public bool CheckAnswer(List<Question> displayed, List<Question> given)
        {
            if (given.Count == 0 || given.Count != displayed.Count)
            {
                return false;
            }
            
            for (int i = 0; i < displayed.Count; i++)
            {
                if ((Color)displayed[i].GetQuestionItem() != (Color)given[i].GetQuestionItem())
                {
                    return false;
                }
            }
            return true;
        }

        public List<Question> GetCorrectQuestions(List<Question> allQuestions, int count = -1)
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
