using UnityEngine;
using UnityEngine.Serialization;
using Utilities.Helpers;

namespace Scriptables.QuestionSystem
{
    [CreateAssetMenu(fileName = "Question", menuName = "Question/DualNBackQuestion")]
    public class DualNBackQuestion : Question
    {
        public int Value;
        public AudioClip QuestionClip;
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
            return QuestionClip == (AudioClip)question.GetQuestionItem();
        }

        public override object GetQuestionItemByType(CommonFields.ButtonType type)
        {
            return QuestionClip;
        }
    }
}
