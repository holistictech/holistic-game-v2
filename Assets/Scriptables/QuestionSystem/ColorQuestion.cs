using UnityEngine;

namespace Scriptables.QuestionSystem
{
    [CreateAssetMenu(fileName = "Question", menuName = "Question/Color")]
    public class ColorQuestion : Question
    {
        public Color QuestionColor;
        public override object GetQuestionItem()
        {
            return QuestionColor;
        }
    }
}
