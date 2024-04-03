using System;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using UnityEngine;

namespace Spans.Skeleton
{
    public abstract class SpanController : MonoBehaviour
    {
        [SerializeField] private List<ISpanState> _states;
        protected SpanStateContext stateContext;
        protected int currentRoundIndex;

        protected int currentSuccessStreak;
        protected int currentFailStreak;
        protected List<Question> currentSpanQuestions;
        private const int _neededStreakCount = 4;
        protected virtual void Start()
        {
            stateContext = new SpanStateContext(this);
            stateContext.Transition(_states[0]);
        }

        public void SwitchState()
        {
            var index = _states.IndexOf(stateContext.CurrentState);
            if (index < _states.Count - 1)
            {
                ISpanState nextState = _states[index];
                stateContext.Transition(nextState);
            }
            else
            {
                stateContext.Transition(_states[1]);
            }
        }
        
        public virtual List<Question> GetSpanObjects()
        {
            return new List<Question>();
        }

        public bool IsAnswerCorrect(List<string> correctAnswers, List<string> givenAnswers)
        {
            //@todo: change this with appropriate to game rules. 
            //maybe a comparator to check correctness percentage.
            for (int i = 0; i < correctAnswers.Count; i++)
            {
                if (!correctAnswers[i].Equals(givenAnswers[i], StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        public ISpanState GetQuestionState()
        {
            return _states[1];
        }

        public virtual int GetRoundTime()
        {
            return 10;
        }

        public int GetRoundIndex()
        {
            return currentRoundIndex;
        }

        public void IncrementRoundIndex()
        {
            currentRoundIndex++;
        }

        public void DecrementRoundIndex()
        {
            currentRoundIndex--;
        }

        public void IncrementSuccessStreak()
        {
            currentSuccessStreak++;
            currentFailStreak = 0;
            if (currentSuccessStreak == _neededStreakCount)
            {
                IncrementRoundIndex();
                currentSuccessStreak = 0;
            }
        }

        public void IncrementFailStreak()
        {
            currentFailStreak++;
            currentSuccessStreak = 0;
            if (currentFailStreak == _neededStreakCount)
            {
                IncrementRoundIndex();
                currentFailStreak = 0;
            }
        }
    }
}
