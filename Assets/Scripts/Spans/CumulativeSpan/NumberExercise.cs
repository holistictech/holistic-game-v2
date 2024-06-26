using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.ForwardSpan;
using Spans.Skeleton;
using UnityEngine;
using Utilities.Helpers;

namespace Spans.CumulativeSpan
{
    public class NumberExercise : NumberDescription
    {
        public override List<Question> GetSpanObjects()
        {
            return PickNumbers(currentRoundIndex / 2);
        }
        
        protected override List<Question> PickNumbers(int count)
        {
            List<Question> pickedQuestions = new List<Question>();
            var numberQuestions = GetAllAvailableSpanObjects();

            var randomIndex = Random.Range(0, numberQuestions.Length);
            while (currentSpanQuestions.Contains(numberQuestions[randomIndex]))
            {
                randomIndex = Random.Range(0, numberQuestions.Length);
            }
            pickedQuestions.Add(numberQuestions[randomIndex]);
            currentSpanQuestions.Add(numberQuestions[randomIndex]);

            return pickedQuestions;
        }
        
        public override List<Question> GetChoices()
        {
            var numberQuestions = GetAllAvailableSpanObjects();
            int choiceCount = 9 - currentRoundIndex / 2;
            /*if (currentRoundIndex >= 4)
            {
                choiceCount = 9 - currentRoundIndex;
            }*/
            List<Question> choices = new List<Question>(GetCurrentDisplayedQuestions());
            
            for (int i = 0; i < choiceCount; i++)
            {
                var index = Random.Range(0, numberQuestions.Length);
                var question = numberQuestions[index];
                while (choices.Contains(question))
                {
                    index = Random.Range(0, numberQuestions.Length);
                    question = numberQuestions[index];
                }
                choices.Add(question);
            }
            choices.Shuffle();
            return choices;
        }
        
        public override void SwitchState()
        {
            if (isSpanFinished)
            {
                Debug.Log("this is finished");
                stateContext.Transition(stateList[^1]);
                return;
            }
            
            var index = stateList.IndexOf(stateContext.CurrentState);
            if (index <= stateList.Count - 3)
            {
                ISpanState nextState = stateList[index+1];
                stateContext.Transition(nextState);
            }
            else
            {
                index = currentGivenAnswers.Count == currentRoundIndex / 2 ? 3 : 1;  
                stateContext.Transition(stateList[index]);
            }
        }
        
        public override bool IsAnswerCorrect()
        {
            foreach (var t in currentGivenAnswers)
            {
                if (currentDisplayedQuestions.Contains(t))
                {
                    IncrementFailStreak();
                    ClearLogicLists();
                    return false;
                }
            }
            IncrementSuccessStreak();
            ClearLogicLists();
            return true;
        }

        private void ClearLogicLists()
        {
            currentGivenAnswers.Clear();
            currentDisplayedQuestions.Clear();
        }
        
        public override void SetCurrentDisplayedQuestions(List<Question> questions)
        {
            currentDisplayedQuestions.AddRange(questions);
        }
        
        public override void SetSelectedAnswers(List<Question> given)
        {
            currentGivenAnswers.AddRange(given);
        }

        protected override void IncrementRoundIndex()
        {
            hasLeveledUp = true;
            currentRoundIndex += 2;
        }
        
        protected override void DecrementRoundIndex()
        {
            currentRoundIndex = Mathf.Max(CommonFields.DEFAULT_ROUND_INDEX, currentRoundIndex -2);
        }
        
        public override int GetStartingUnitIndex()
        {
            return 1;
        }
    }
}
