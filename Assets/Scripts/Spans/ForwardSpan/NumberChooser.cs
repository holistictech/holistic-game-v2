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
            int choiceCount = currentRoundIndex;
            List<Question> choices = new List<Question>(GetCurrentQuestions());
            
            for (int i = 0; i < choiceCount; i++)
            {
                var index = Random.Range(0, NumberQuestions.Length);
                var question = NumberQuestions[index];
                while (choices.Contains(question))
                {
                    index = Random.Range(0, NumberQuestions.Length);
                    question = NumberQuestions[index];
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
            
            for (int i = 0; i < currentDisplayedQuestions.Count; i++)
            {
                if (currentDisplayedQuestions[i].CorrectAnswer != currentGivenAnswers[i].CorrectAnswer)
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
