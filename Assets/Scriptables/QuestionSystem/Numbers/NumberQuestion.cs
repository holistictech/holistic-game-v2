using UnityEngine;
using Utilities.Helpers;

namespace Scriptables.QuestionSystem.Numbers
{
    [CreateAssetMenu(fileName = "Question", menuName = "Question/Number")]
    public class NumberQuestion : Question
    {
        public int Value;
        public override void SetQuestionItem(object item)
        {
            throw new System.NotImplementedException();
        }

        public override object GetQuestionItem()
        {
            return Value;
        }

        public override bool IsEqual(Question question)
        {
            return Value == (int)question.GetQuestionItem();
        }

        public override object GetQuestionItemByType(CommonFields.ButtonType type)
        {
            throw new System.NotImplementedException();
        }
    }
}
