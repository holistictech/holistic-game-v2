using System.Collections.Generic;
using UnityEngine;

namespace Spans.Skeleton
{
    public class SpanController : MonoBehaviour
    {
        [SerializeField] private List<ISpanState> _states;
        protected SpanStateContext stateContext;
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
    }
}
