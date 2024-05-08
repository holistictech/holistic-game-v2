using System.Collections.Generic;
using System.Linq;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;

namespace Spans.CorsiSpan
{
    public class CorsiSpan : SpanController
    {
        public override List<Question> GetSpanObjects()
        {
            ResetQuestionStatus();
            var allQuestions = GetAllAvailableSpanObjects();
            List<Question> roundQuestions = new List<Question>();
            for (int i = 0; i < GetRoundIndex(); i++)
            {
                var randomQuestionIndex = Random.Range(0, allQuestions.Length);
                var randomQuestion = allQuestions[randomQuestionIndex];
                while (randomQuestion.HasSelected())
                {
                    randomQuestionIndex = Random.Range(0, allQuestions.Length);
                    randomQuestion = allQuestions[randomQuestionIndex];
                }
                
                randomQuestion.SetHasSelected(true);
                roundQuestions.Add(randomQuestion);
            }

            currentSpanQuestions = roundQuestions;
            
            return allQuestions.ToList();
        }

        public override bool IsAnswerCorrect()
        {
            if (currentGivenAnswers.Count == 0 || currentGivenAnswers.Count != currentSpanQuestions.Count)
            {
                IncrementFailStreak();
                return false;
            }
            
            for (int i = 0; i < currentSpanQuestions.Count; i++)
            {
                if ((int)currentSpanQuestions[i].GetQuestionItem() != (int)currentGivenAnswers[i].GetQuestionItem())
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
