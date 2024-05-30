using UI.CorsiBlockTypes;
using UnityEngine;

namespace Interfaces
{
    public interface IBlockSpanStrategy
    {
        public void HighlightBlock(AdaptableBlock targetBlock);
        public void CheckAnswer();
    }
}
