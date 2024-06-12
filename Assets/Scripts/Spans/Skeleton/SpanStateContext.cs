namespace Spans.Skeleton
{
    public class SpanStateContext
    {
        public ISpanState CurrentState { get; set; }

        private readonly SpanController _spanController;

        public SpanStateContext(SpanController spanController)
        {
            _spanController = spanController;
        }

        private void Transition()
        {
            CurrentState.Enter(_spanController);
        }

        public void Transition(ISpanState state)
        {
            CurrentState?.Exit();
            CurrentState = state;
            Transition();
        }
    }
}
