using System.Collections.Generic;
using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using UI.CorsiBlockTypes;
using UnityEngine;

namespace Spans.BlockSpan
{
    public class RegularMode : IBlockSpanStrategy
    {
        public void HighlightBlock(AdaptableBlock targetBlock)
        {
            var blockImage = targetBlock.GetBlockImage();
            var color = targetBlock.GetHighlightColor();
            blockImage.DOColor(color, 1f).SetEase(Ease.Flash).OnComplete(targetBlock.ResetUI);
        }

        public void SetBlockSelected(AdaptableBlock block, Question selection)
        {
            var blockImage = block.GetBlockImage();
            var color = block.GetHighlightColor();
            blockImage.color = color;
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
            List<Question> corrects = new List<Question>();
            for (int i = 0; i < count; i++)
            {
                var randomIndex = Random.Range(0, allQuestions.Count);
                var tempQuestion = allQuestions[randomIndex];
                while (corrects.Contains(tempQuestion))
                {
                    randomIndex = Random.Range(0, allQuestions.Count);
                    tempQuestion = allQuestions[randomIndex];
                }
                
                corrects.Add(tempQuestion);
            }
            
            return corrects;
        }
    }
}
