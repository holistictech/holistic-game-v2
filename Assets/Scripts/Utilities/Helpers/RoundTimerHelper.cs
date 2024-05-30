using System.Collections;
using Spans.Skeleton;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace Utilities
{
    public class RoundTimerHelper : MonoBehaviour
    {
        private SpanController _spanController;
        private Coroutine _timer;
        private int a;
        public void InjectSpanController(SpanController controller)
        {
            _spanController = controller;
            _timer = StartCoroutine(StartTimer());
        }
        public IEnumerator StartTimer()
        {
            var roundTime = (int)CommonFields.ROUND_DURATION;
            for (int i = roundTime; i > 0; i--)
            {
                yield return new WaitForSeconds(1f);
            }
            _spanController.SetSpanCompleted();
        }
    }
}
