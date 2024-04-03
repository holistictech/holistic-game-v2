using System;
using System.Collections.Generic;
using Scriptables;
using Scriptables.QuestionSystem;
using UnityEngine;

namespace Spans.Skeleton
{
    public abstract class SpanController : MonoBehaviour
    {
        [SerializeField] private StateHolder states;
        private List<ISpanState> _stateList = new List<ISpanState>();
        protected SpanStateContext stateContext;
        protected int currentRoundIndex = 1;

        protected int currentSuccessStreak;
        protected int currentFailStreak;
        protected List<Question> currentSpanQuestions;
        private List<string> _currentCorrectAnswers;
        private List<string> _currentDetectedAnswers;
        private const int _neededStreakCount = 4;

        protected virtual void Start()
        {
            stateContext = new SpanStateContext(this);
            _stateList = new List<ISpanState>()
            {
                states.InitialState,
                states.QuestionState,
                states.AnswerState,
                states.AnswerState,
                states.GameEndState
            };
            stateContext.Transition(states.InitialState);
        }

        public void SwitchState()
        {
            var index = _stateList.IndexOf(stateContext.CurrentState);
            if (index < _stateList.Count - 1)
            {
                ISpanState nextState = _stateList[index];
                stateContext.Transition(nextState);
            }
            else
            {
                stateContext.Transition(states.QuestionState); // to turn back to question state.
            }
        }

        public virtual List<Question> GetSpanObjects()
        {
            return new List<Question>();
        }

        public bool IsAnswerCorrect()
        {
            //@todo: change this with appropriate to game rules. 
            //maybe a comparator to check correctness percentage.
            for (int i = 0; i < _currentCorrectAnswers.Count; i++)
            {
                if (!_currentCorrectAnswers[i].Equals(_currentDetectedAnswers[i], StringComparison.OrdinalIgnoreCase))
                {
                    IncrementFailStreak();
                    return false;
                }
            }

            IncrementSuccessStreak();
            return true;
        }

        public void SetCorrectAnswers(List<string> corrects)
        {
            _currentCorrectAnswers = corrects;
        }

        public void SetDetectedAnswers(List<string> given)
        {
            _currentDetectedAnswers = given;
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
                DecrementRoundIndex();
                currentFailStreak = 0;
            }
        }
    }
}