using UnityEngine;
using Utilities;
using Utilities.Helpers;

namespace Scriptables.QuestionSystem
{
    [CreateAssetMenu(fileName = "Question", menuName = "Question/Number")]
    public class NumberQuestion : Question
    {
        public int Value;
        public override object GetQuestionItem()
        {
            return Value;
        }

        public override object GetQuestionItemByType(CommonFields.ButtonType type)
        {
            throw new System.NotImplementedException();
        }
    }
}
