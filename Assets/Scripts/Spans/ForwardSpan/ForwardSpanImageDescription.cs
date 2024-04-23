using System;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Spans.ForwardSpan
{
    public class ForwardSpanImageDescription : SpanController
    {
        public ImageQuestion[] ImageQuestions;
        
        public override List<Question> GetSpanObjects()
        {
            return GetRandomSprites();
        }

        public override int GetRoundTime()
        {
            return currentRoundIndex * 3 + 2;
        }
        
        private List<Question> GetRandomSprites()
        {
            List<Question> shuffledSprites = new List<Question>(ImageQuestions);
            for (int i = 0; i < shuffledSprites.Count; i++)
            {
                int randomIndex = Random.Range(i, shuffledSprites.Count);
                (shuffledSprites[i], shuffledSprites[randomIndex]) = (shuffledSprites[randomIndex], shuffledSprites[i]);
            }
            
            List<Question> selected = new List<Question>();
            for (int i = 0; i < fetchedQuestionCount; i++)
            {
                selected.Add(shuffledSprites[i]);
            }

            currentSpanQuestions = selected;
            return selected;
        }
        
        public override bool IsAnswerCorrect()
        {
            //@todo: change this with appropriate to game rules. 
            //maybe a comparator to check correctness percentage.
            for (int i = 0; i < currentDisplayedQuestions.Count; i++)
            {
                if (!currentDisplayedQuestions[i].CorrectAnswer.Equals(currentDetectedAnswers[i], StringComparison.OrdinalIgnoreCase))
                {
                    IncrementFailStreak();
                    return false;
                }
            }
            IncrementSuccessStreak();
            return true;
        }
    }
}
