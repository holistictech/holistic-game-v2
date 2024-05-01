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
        public NumberQuestion[] NumberQuestions;
        public override List<Question> GetSpanObjects()
        {
            return PickNumbers(fetchedQuestionCount);
        }
        
        public override int GetRoundTime()
        {
            return currentRoundIndex * 3;
        }
        
        private List<Question> PickNumbers(int count)
        {
            List<Question> pickedNumbers = new List<Question>();
            HashSet<int> pickedIndices = new HashSet<int>();
            
            ShuffleArray(NumberQuestions);
            
            for (int i = 0; i < NumberQuestions.Length && pickedNumbers.Count < count; i++)
            {
                if (!pickedIndices.Contains(i))
                {
                    pickedNumbers.Add(NumberQuestions[i]);
                    pickedIndices.Add(i);

                    int nextIndex = FindNextNonConsecutiveIndex(i, pickedIndices);
                    if (nextIndex != -1)
                    {
                        pickedNumbers.Add(NumberQuestions[nextIndex]);
                        pickedIndices.Add(nextIndex);
                    }
                }
            }

            currentSpanQuestions = pickedNumbers;
            return pickedNumbers;
        }
        private void ShuffleArray<T>(T[] array)
        {
            System.Random rng = new System.Random();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (array[k], array[n]) = (array[n], array[k]);
            }
        }
        
        private int FindNextNonConsecutiveIndex(int currentIndex, HashSet<int> pickedIndices)
        {
            for (int i = currentIndex + 1; i < NumberQuestions.Length; i++)
            {
                if (!pickedIndices.Contains(i) && Math.Abs(NumberQuestions[currentIndex].Value - NumberQuestions[i].Value) > 1)
                {
                    return i;
                }
            }
            return -1;
        }
        
        public override bool IsAnswerCorrect()
        {
            //@todo: change this with appropriate to game rules. 
            //maybe a comparator to check correctness percentage.
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
