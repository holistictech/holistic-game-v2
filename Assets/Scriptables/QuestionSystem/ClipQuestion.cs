using UnityEngine;
using Utilities;
using Utilities.Helpers;

namespace Scriptables.QuestionSystem
{
    [CreateAssetMenu(fileName = "Question", menuName = "Question/Clip")]
    public class ClipQuestion : Question
    {
        public AudioClip Clip;
        public Sprite ClipSprite;
        public override void SetQuestionItem(object item)
        {
            throw new System.NotImplementedException();
        }

        public override object GetQuestionItem()
        {
            return Clip;
        }

        public override bool IsEqual(Question question)
        {
            return question.GetQuestionItem().Equals(Clip);
        }

        public override object GetQuestionItemByType(CommonFields.ButtonType type)
        {
            return CorrectAnswerString;
        }

        public override Sprite GetCorrectSprite()
        {
            return IsAnswerStringMUST ? null : ClipSprite;
        }

        public override string GetCorrectText()
        {
            return IsAnswerStringMUST ? CorrectAnswerString : "";
        }
    }
}
