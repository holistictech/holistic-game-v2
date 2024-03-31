using UnityEngine;

namespace Spans
{
    public interface ISpanState
    {
        public void Handle(SpanController spanController);
    }
}
