using System;
using System.Collections.Generic;
using System.Linq;
using Scriptables.QuestionSystem;
using Spans.ForwardSpan;
using Spans.Skeleton;
using Utilities;
using Random = UnityEngine.Random;

namespace Spans.CumulativeSpan
{
    public class CumulativeChooser : ForwardSpanImageDescription
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
        
        public Question GetUnusedQuestion()
        {
            var imageQuestions = base.GetAllAvailableSpanObjects();
            var randomIndex = Random.Range(0, imageQuestions.Length);
            while (imageQuestions[randomIndex].HasSelected())
            {
                randomIndex = Random.Range(0, imageQuestions.Length);
            }
            imageQuestions[randomIndex].SetHasSelected(true);
            return imageQuestions[randomIndex];
        }
        

        public override List<Question> GetChoices()
        {
            var imageQuestions = base.GetAllAvailableSpanObjects();
            List<Question> choices = new List<Question>(currentSpanQuestions);
            var iterations = _choiceCount - choices.Count;
            
            for (int i = 0; i < iterations; i++)
            {
                var index = Random.Range(0, imageQuestions.Length);
                var question = imageQuestions[index];
                while (choices.Contains(question))
                {
                    index = Random.Range(0, imageQuestions.Length);
                    question = imageQuestions[index];   
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
                if (currentDisplayedQuestions[i] is NumberQuestion)
                {
                    if ((int)currentDisplayedQuestions[i].GetQuestionItem() != (int)currentGivenAnswers[i].GetQuestionItem())
                    {
                        IncrementFailStreak();
                        return false;
                    }
                }
                else if (currentDisplayedQuestions[i] is ImageQuestion)
                {
                    if (currentDisplayedQuestions[i].GetQuestionItem() != currentGivenAnswers[i].GetQuestionItem())
                    {
                        IncrementFailStreak();
                        return false;
                    }
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
            currentDisplayedQuestions.Clear();
            activeUnitCircles.Clear();
            ResetRoundIndex();
            spanEventBus.Trigger(new RoundResetEvent());
        }

        protected override void IncrementSuccessStreak()
        {
            StatisticsHelper.IncrementTrueCount();
            if(currentRoundIndex < 9)
                currentRoundIndex++;
        }
    }
}
