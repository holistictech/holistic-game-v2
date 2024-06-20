using System;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;
using UnityEngine.Serialization;

namespace Spans.ForwardSpan
{
    public class ForwardSpanNumberDescription : SpanController
    {
        public override List<Question> GetSpanObjects()
        {
            return PickNumbers(currentRoundIndex);
        }
        
        public override float GetRoundTime()
        {
            return currentRoundIndex * 3;
        }
        
        private List<Question> PickNumbers(int count)
        {
            List<Question> pickedNumbers = new List<Question>();
            HashSet<int> pickedIndices = new HashSet<int>();
            var numberQuestions = GetAllAvailableSpanObjects();
            System.Random rng = new System.Random();

            while (pickedNumbers.Count < count && pickedIndices.Count < numberQuestions.Length)
            {
                int randomIndex = rng.Next(numberQuestions.Length);

                if (!pickedIndices.Contains(randomIndex))
                {
                    pickedNumbers.Add(numberQuestions[randomIndex]);
                    pickedIndices.Add(randomIndex);

                    int nextIndex = FindNextNonConsecutiveIndex(randomIndex, pickedIndices, numberQuestions.Length, rng);
                    if (nextIndex != -1)
                    {
                        pickedNumbers.Add(numberQuestions[nextIndex]);
                        pickedIndices.Add(nextIndex);
                    }
                }
            }

            currentSpanQuestions = pickedNumbers;
            return pickedNumbers;
        }

        private int FindNextNonConsecutiveIndex(int currentIndex, HashSet<int> pickedIndices, int arrayLength, System.Random rng)
        {
            List<int> availableIndices = new List<int>();

            for (int i = 0; i < arrayLength; i++)
            {
                if (!pickedIndices.Contains(i) && Math.Abs(currentIndex - i) > 1)
                {
                    availableIndices.Add(i);
                }
            }

            if (availableIndices.Count > 0)
            {
                return availableIndices[rng.Next(availableIndices.Count)];
            }

            return -1;
        }
        
        public override bool IsAnswerCorrect()
        {
            //@todo: change this with appropriate to game rules. 
            //maybe a comparator to check correctness percentage.
            
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
                if (!currentDisplayedQuestions[i].CorrectAnswerString.Equals(currentDetectedAnswers[i], StringComparison.OrdinalIgnoreCase))
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
