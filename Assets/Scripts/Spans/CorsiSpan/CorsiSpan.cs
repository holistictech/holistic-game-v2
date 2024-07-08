using System.Collections.Generic;
using System.Linq;
using Scriptables.QuestionSystem;
using Spans.CumulativeSpan;
using Spans.Skeleton;
using UnityEngine;
using Utilities;

namespace Spans.CorsiSpan
{
    public class CorsiSpan : SpanController
    {
        public override List<Question> GetSpanObjects()
        {
            if (!GetCumulativeStatus())
            {
                ResetQuestionStatus();
            }
            var allQuestions = GetAllAvailableSpanObjects();
            List<Question> roundQuestions = new List<Question>();
            var iterations = GetCumulativeStatus() ? GetRoundIndex() - currentSpanQuestions.Count : GetRoundIndex();
            for (int i = 0; i < iterations; i++)
            {
                var randomQuestionIndex = Random.Range(0, allQuestions.Length);
                var randomQuestion = allQuestions[randomQuestionIndex];
                while (randomQuestion.HasSelected())
                {
                    randomQuestionIndex = Random.Range(0, allQuestions.Length);
                    randomQuestion = allQuestions[randomQuestionIndex];
                }
                
                randomQuestion.SetHasSelected(true);
                roundQuestions.Add(randomQuestion);
            }

            if (GetCumulativeStatus())
            {
                currentSpanQuestions.AddRange(roundQuestions);
            }
            else
            {
                currentSpanQuestions = roundQuestions;
            }
            
            return allQuestions.ToList();
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
            if (index < stateList.Count - 2)
            {
                ISpanState nextState = stateList[index+1];
                stateContext.Transition(nextState);
            }
            else
            {
                stateContext.Transition(stateList[1]); // to turn back to question state for NBack scenarios.
            }
        }

        public override bool IsAnswerCorrect()
        {
            if (currentGivenAnswers.Count == 0 || currentGivenAnswers.Count != currentSpanQuestions.Count)
            {
                IncrementFailStreak();
                return false;
            }

            if (GetBackwardStatus())
            {
                currentSpanQuestions.Reverse();
            }
            
            for (int i = 0; i < currentSpanQuestions.Count; i++)
            {
                if ((int)currentSpanQuestions[i].GetQuestionItem() != (int)currentGivenAnswers[i].GetQuestionItem())
                {
                    IncrementFailStreak();
                    return false;
                }
            }
            IncrementSuccessStreak();
            return true;
        }

        protected override void IncrementSuccessStreak()
        {
            if (GetCumulativeStatus())
            {
                if(currentRoundIndex < 9)
                    currentRoundIndex++;
            }
            else
            {
                base.IncrementSuccessStreak();
            }
        }
        
        protected override void IncrementFailStreak()
        {
            if (GetCumulativeStatus())
            {
                currentSpanQuestions.Clear();
                currentDisplayedQuestions.Clear();
                activeUnitCircles.Clear();
                ResetRoundIndex();
                EventBus.Instance.Trigger(new RoundResetEvent());
            }
            else
            {
                base.IncrementFailStreak();
            }
        }
    }
}
