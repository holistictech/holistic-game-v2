using System.Collections.Generic;
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
        
        public virtual List<object> GetSpanObjects()
        {
            return new List<object>();
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
        }

        public void IncrementFailStreak()
        {
            currentFailStreak++;
            currentSuccessStreak = 0;
        }
    }
}
