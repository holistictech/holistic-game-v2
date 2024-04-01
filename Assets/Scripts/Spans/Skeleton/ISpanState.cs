using Spans.Skeleton;
using UnityEngine;

namespace Spans
{
    public interface ISpanState
    {
        public abstract void Handle(SpanController spanController);
    }
}
