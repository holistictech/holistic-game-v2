using UnityEngine;
using Utilities;

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

        public override object GetQuestionItemByType(CommonFields.ButtonType type)
        {
            throw new System.NotImplementedException();
        }
    }
}
