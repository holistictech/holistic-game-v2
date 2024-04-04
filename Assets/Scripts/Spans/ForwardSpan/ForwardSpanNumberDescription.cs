using System;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;

namespace Spans.ForwardSpan
{
    public class ForwardSpanNumberDescription : SpanController
    {
        [SerializeField] private NumberQuestion[] numbers;
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
            
            ShuffleArray(numbers);
            
            for (int i = 0; i < numbers.Length && pickedNumbers.Count < count; i++)
            {
                if (!pickedIndices.Contains(i))
                {
                    pickedNumbers.Add(numbers[i]);
                    pickedIndices.Add(i);

                    int nextIndex = FindNextNonConsecutiveIndex(i, pickedIndices);
                    if (nextIndex != -1)
                    {
                        pickedNumbers.Add(numbers[nextIndex]);
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
            for (int i = currentIndex + 1; i < numbers.Length; i++)
            {
                if (!pickedIndices.Contains(i) && Math.Abs(numbers[currentIndex].Value - numbers[i].Value) > 1)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
