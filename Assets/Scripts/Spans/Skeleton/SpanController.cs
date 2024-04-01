using UnityEngine;

namespace Spans.Skeleton
{
    public class SpanController : MonoBehaviour
    {
        protected SpanStateContext stateContext;
        protected virtual void Start()
        {
            stateContext = new SpanStateContext(this);
        }
    }
}
