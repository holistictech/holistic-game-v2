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
        public override object GetQuestionItem()
        {
            return Clip;
        }

        public override object GetQuestionItemByType(CommonFields.ButtonType type)
        {
            throw new System.NotImplementedException();
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
