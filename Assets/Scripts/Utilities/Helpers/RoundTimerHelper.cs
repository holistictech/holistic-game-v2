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
        public void InjectSpanController(SpanController controller, int roundTime)
        {
            _spanController = controller;
            _timer = StartCoroutine(StartTimer(roundTime));
        }
        public IEnumerator StartTimer(int time)
        {
            var roundTime = time;
            for (int i = roundTime; i > 0; i--)
            {
                yield return new WaitForSeconds(1f);
            }
            _spanController.SetSpanCompleted();
        }

        public void StopTimer()
        {
            if (_timer != null)
            {
                StopCoroutine(_timer);
            }
        }
    }
}
