using Spans.Skeleton;
using UnityEngine;

namespace Spans
{
    public class SpanStateContext
    {
        public ISpanState CurrentState { get; set; }

        private readonly SpanController _spanController;

        public SpanStateContext(SpanController spanController)
        {
            _spanController = spanController;
        }

        public void Transition()
        {
            CurrentState.Enter(_spanController);
        }

        public void Transition(ISpanState state)
        {
            if(CurrentState != null)
                CurrentState.Exit();
            CurrentState = state;
            CurrentState.Enter(_spanController);
        }
    }
}
