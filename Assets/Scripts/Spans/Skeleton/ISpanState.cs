namespace Spans.Skeleton
{
    public interface ISpanState
    {
        public abstract void Enter(SpanController spanController);
        public abstract void Exit();

        public abstract void SwitchNextState();
        public abstract void EnableUIElements();
        public abstract void DisableUIElements();
    }
}
