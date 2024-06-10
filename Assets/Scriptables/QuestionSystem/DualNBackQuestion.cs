using UnityEngine;
using UnityEngine.Serialization;
using Utilities.Helpers;

namespace Scriptables.QuestionSystem
{
    [CreateAssetMenu(fileName = "Question", menuName = "Question/DualNBackQuestion")]
    public class DualNBackQuestion : Question
    {
        public int Value;
        [FormerlySerializedAs("questionClip")] public AudioClip QuestionClip;
        public override object GetQuestionItem()
        {
            return Value;
        }

        public override object GetQuestionItemByType(CommonFields.ButtonType type)
        {
            return QuestionClip;
        }
    }
}
