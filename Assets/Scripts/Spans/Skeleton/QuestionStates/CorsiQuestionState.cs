using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Spans.Skeleton.QuestionStates
{
    public class CorsiQuestionState : SpanQuestionState
    {
        [SerializeField] private LayoutGroup blockParent;
        [SerializeField] private CorsiBlock blockPrefab;
        private const int _corsiBlockCount = 9;

        private void SpawnCorsiBlocks()
        {
            for (int i = 0; i < _corsiBlockCount; i++)
            {
                var tempBlock = Instantiate(blockPrefab, blockParent.transform);
            }
        }
    }
}
