using UnityEngine;

namespace Scriptables.QuestionSystem
{
    [CreateAssetMenu(fileName = "Question", menuName = "Question/Number")]
    public class NumberQuestion : Question
    {
        public int Value;
    }
}
