using System.Collections.Generic;
using System.Linq;
using Scriptables.QuestionSystem;
using Spans.ForwardSpan;
using UnityEngine;

namespace Spans.CumulativeSpan
{
    public class CumulativeImageChooser : ForwardSpanImageDescription
    {
        private const int _choiceCount = 9;

        public override List<Question> GetSpanObjects()
        {
            List<Question> roundQuestions = new List<Question>();
            for (int i = 0; i < currentRoundIndex - currentSpanQuestions.Count; i++)
            {
                roundQuestions.Add(GetUnusedQuestion());
            }

            currentSpanQuestions.AddRange(roundQuestions);
            return currentSpanQuestions;
        }

        private Question GetUnusedQuestion()
        {
            var randomIndex = Random.Range(0, ImageQuestions.Length);
            while (ImageQuestions[randomIndex].HasSelected())
            {
                randomIndex = Random.Range(0, ImageQuestions.Length);
            }
            ImageQuestions[randomIndex].SetHasSelected(true);
            return ImageQuestions[randomIndex];
        }
        
        public override List<Question> GetChoices()
        {
            List<Question> choices = new List<Question>(currentSpanQuestions);
            var iterations = _choiceCount - choices.Count;
            
            for (int i = 0; i < iterations; i++)
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
        
        public override void SetCurrentDisplayedQuestions(List<Question> questions)
        {
            currentDisplayedQuestions.AddRange(questions);
        }

        protected override void IncrementFailStreak()
        {
            currentSpanQuestions.Clear();
            ResetRoundIndex();
        }

        protected override void IncrementSuccessStreak()
        {
            if(currentRoundIndex < 9)
                currentRoundIndex++;
        }
    }
}
