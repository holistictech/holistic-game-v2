using System;
using System.Collections.Generic;
using System.Linq;
using Scriptables.QuestionSystem;
using Scriptables.QuestionSystem.Images;
using Spans.ForwardSpan;
using Spans.Skeleton;
using UnityEngine;
using Utilities;
using Utilities.Helpers;
using Random = UnityEngine.Random;

namespace Spans.CumulativeSpan
{
    public class CumulativeChooser : ImageDescription
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
            if (currentGivenAnswers.Count == 0 || currentGivenAnswers.Count != currentSpanQuestions.Count)
            {
                IncrementFailStreak();
                return false;
            }
            
            for (int i = 0; i < currentSpanQuestions.Count; i++)
            {
                if (currentSpanQuestions[i] is NumberQuestion)
                {
                    if ((int)currentSpanQuestions[i].GetQuestionItem() != (int)currentGivenAnswers[i].GetQuestionItem())
                    {
                        IncrementFailStreak();
                        return false;
                    }
                }
                else if (currentSpanQuestions[i] is ImageQuestion)
                {
                    if ((Sprite)currentSpanQuestions[i].GetQuestionItem() != (Sprite)currentGivenAnswers[i].GetQuestionItem())
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
