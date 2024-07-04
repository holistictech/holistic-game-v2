using System;
using System.Collections;
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
        [SerializeField] private bool isNoteExercise;
        protected int choiceCount = 9;
        public override List<Question> GetSpanObjects()
        {
            List<Question> roundQuestions = new List<Question>();
            if (!isNoteExercise)
            {
                for (int i = 0; i < currentRoundIndex - currentSpanQuestions.Count; i++)
                {
                    roundQuestions.Add(GetUnusedQuestion());
                }
                currentSpanQuestions.AddRange(roundQuestions);
            }
            else
            {
                var allQuestions = GetAllAvailableSpanObjects();
                for (int i = 0; i < 4; i++)
                {
                    roundQuestions.Add(allQuestions[i]);
                }

                currentSpanQuestions = roundQuestions;
            }
            
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
        
        public override List<Question> GetCurrentSpanQuestions()
        {
            if (!isNoteExercise)
            {
                return base.GetCurrentSpanQuestions();
            }
            
            var allQuestions = currentSpanQuestions;
            List<Question> corrects = new List<Question>();
            for (int i = 0; i < currentRoundIndex - currentDisplayedQuestions.Count; i++)
            {
                var randomIndex = Random.Range(0, allQuestions.Count);
                var tempQuestion = allQuestions[randomIndex];
                corrects.Add(tempQuestion);
            }
            
            return corrects;
        }

        public override List<Question> GetChoices()
        {
            var imageQuestions = base.GetAllAvailableSpanObjects();
            List<Question> choices = new List<Question>(currentSpanQuestions);
            var iterations = choiceCount - choices.Count;
            
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
            var listToBeChecked = isNoteExercise ? currentDisplayedQuestions : currentSpanQuestions;
            if (currentGivenAnswers.Count == 0 || currentGivenAnswers.Count != listToBeChecked.Count)
            {
                IncrementFailStreak();
                return false;
            }

            if (isBackwards)
            {
                listToBeChecked = ReverseDisplayedQuestions();
            }

            for (int i = 0; i < listToBeChecked.Count; i++)
            {
                Question questionItem = listToBeChecked[i];
                Question answerItem = currentGivenAnswers[i];

                if (!questionItem.IsEqual(answerItem))
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

        private bool _isInitial = true;

        private List<Question> ReverseDisplayedQuestions()
        {
            if (_isInitial)
            {
                currentDisplayedQuestions.Reverse();
                _isInitial = false;
            }
            else
            {
                // Get the last given question
                var lastGiven = currentDisplayedQuestions[^1];
                currentDisplayedQuestions.RemoveAt(currentDisplayedQuestions.Count - 1);
                //currentDisplayedQuestions.Reverse();
                currentDisplayedQuestions.Insert(0, lastGiven);
            }

            return currentDisplayedQuestions;
        }


        protected override void IncrementFailStreak()
        {
            currentSpanQuestions.Clear();
            currentDisplayedQuestions.Clear();
            activeUnitCircles.Clear();
            ResetRoundIndex();
            eventBus.Trigger(new RoundResetEvent());
        }

        protected override void IncrementSuccessStreak()
        {
            StatisticsHelper.IncrementTrueCount();
            if(currentRoundIndex < 9)
                currentRoundIndex++;
        }
    }
}
