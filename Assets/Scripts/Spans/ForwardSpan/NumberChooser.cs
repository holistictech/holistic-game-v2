using System;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Random = UnityEngine.Random;

namespace Spans.ForwardSpan
{
    public class NumberChooser : ForwardSpanNumberDescription
    {
        public override List<Question> GetChoices()
        {
            var numberQuestions = GetAllAvailableSpanObjects();
            int choiceCount = currentRoundIndex;
            if (currentRoundIndex >= 4)
            {
                choiceCount = 9 - currentRoundIndex;
            }
            List<Question> choices = new List<Question>(GetCurrentDisplayedQuestions());
            
            for (int i = 0; i < choiceCount; i++)
            {
                var index = Random.Range(0, numberQuestions.Length);
                var question = numberQuestions[index];
                while (choices.Contains(question))
                {
                    index = Random.Range(0, numberQuestions.Length);
                    question = numberQuestions[index];
                }
                choices.Add(question);
            }
            choices.Shuffle();
            return choices;
        }
        
        public override bool IsAnswerCorrect()
        {
            if (currentGivenAnswers.Count == 0 || currentGivenAnswers.Count != currentDisplayedQuestions.Count)
            {
                IncrementFailStreak();
                return false;
            }
            
            if (isBackwards)
            {
                currentDisplayedQuestions.Reverse();
            }
            
            for (int i = 0; i < currentDisplayedQuestions.Count; i++)
            {
                if ((int)currentDisplayedQuestions[i].GetQuestionItem() != (int)currentGivenAnswers[i].GetQuestionItem())
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
