using System;
using System.Collections.Generic;
using Spans.Skeleton;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spans
{
    public class ForwardSpanNumberDescription : SpanController
    {
        private List<object> _currentRoundQuestions;

        public override List<object> GetSpanObjects()
        {
            IncrementRoundIndex();
            return GenerateNonConsecutiveNumbers(currentRoundIndex);
        }
        
        private List<object> GenerateNonConsecutiveNumbers(int generateCount)
        {
            List<object> numbers = new List<object>();
            HashSet<int> generatedNumbers = new HashSet<int>();
            System.Random random = new System.Random();
            
            int previousNumber = random.Next(1, 10);
            numbers.Add(previousNumber);
            generatedNumbers.Add(previousNumber);
            
            while (numbers.Count < generateCount)
            {
                int newNumber;
                do
                {
                    newNumber = random.Next(1, 10);
                }
                while (Mathf.Abs(newNumber - previousNumber) == 1 || generatedNumbers.Contains(newNumber));

                numbers.Add(newNumber);
                previousNumber = newNumber;
            }

            _currentRoundQuestions = numbers;
            return numbers;
        }
    }
}
