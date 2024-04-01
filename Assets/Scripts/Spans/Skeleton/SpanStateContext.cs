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
            CurrentState.Handle(_spanController);
        }

        public void Transition(ISpanState state)
        {
            CurrentState = state;
            CurrentState.Handle(_spanController);
        }
    }
}
