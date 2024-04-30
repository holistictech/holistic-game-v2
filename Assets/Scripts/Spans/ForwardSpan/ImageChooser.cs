using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;

namespace Spans.ForwardSpan
{
    public class ImageChooser : ForwardSpanImageDescription
    {
        public override List<Question> GetChoices()
        {
            int choiceCount = currentRoundIndex;
            if (currentRoundIndex >= 4)
            {
                choiceCount = 9 - currentRoundIndex;
            }
            
            List<Question> choices = new List<Question>(GetCurrentDisplayedQuestions());
            
            for (int i = 0; i < choiceCount; i++)
            {
                var index = Random.Range(0, ImageQuestions.Length);
                var question = ImageQuestions[index];
                while (choices.Contains(question))
                {
                    index = Random.Range(0, ImageQuestions.Length);
                    question = ImageQuestions[index];
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
                if (currentDisplayedQuestions[i].GetQuestionItem() != currentGivenAnswers[i].GetQuestionItem())
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
