using System;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.ForwardSpan;
using UnityEngine;

namespace Spans.CumulativeSpan
{
    public class RunningSpan : NumberChooser
    {
        private const int MaxFetchCount = 5;
        private int _startingUnitIndex = 0;
        public override List<Question> GetSpanObjects()
        {
            int random = UnityEngine.Random.Range(Mathf.Min(currentRoundIndex+1, MaxFetchCount + 1), Mathf.Max(currentRoundIndex+1, MaxFetchCount + 1));
            return PickNumbers(random);
        }
        
        public override void SetCurrentDisplayedQuestions(List<Question> questions)
        {
            currentDisplayedQuestions = new List<Question>();
            _startingUnitIndex = questions.Count - currentRoundIndex;
            for (int i = _startingUnitIndex; i < questions.Count; i++)
            {
                currentDisplayedQuestions.Add(questions[i]);
            }
        }
        
        public override bool IsAnswerCorrect()
        {
            if (currentGivenAnswers.Count == 0 || currentGivenAnswers.Count != currentDisplayedQuestions.Count)
            {
                IncrementFailStreak();
                return false;
            }
            
            for (int i = 0; i < currentDisplayedQuestions.Count; i++)
            {
                if (!currentDisplayedQuestions[i].GetCorrectText().Equals(currentGivenAnswers[i].GetCorrectText()))
                {
                    IncrementFailStreak();
                    return false;
                }
            }
            IncrementSuccessStreak();
            return true;
        }

        public override int GetStartingUnitIndex()
        {
            return _startingUnitIndex;
        }
    }
}
