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
            var allImageQuestions = base.GetAllAvailableSpanObjects();
            int choiceCount = currentRoundIndex;
            if (currentRoundIndex >= 4)
            {
                choiceCount = 9 - currentRoundIndex;
            }
            
            List<Question> choices = new List<Question>(GetCurrentDisplayedQuestions());
            
            for (int i = 0; i < choiceCount; i++)
            {
                var index = Random.Range(0, allImageQuestions.Length);
                var question = allImageQuestions[index];
                while (choices.Contains(question))
                {
                    index = Random.Range(0, allImageQuestions.Length);
                    question = allImageQuestions[index];
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
                if ((Sprite)currentDisplayedQuestions[i].GetQuestionItem() != (Sprite)currentGivenAnswers[i].GetQuestionItem())
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
