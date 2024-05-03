using UnityEngine;

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

        public override Sprite GetCorrectSprite()
        {
            return IsAnswerStringMUST ? ClipSprite : null;
        }

        public override string GetCorrectText()
        {
            return IsAnswerStringMUST ? CorrectAnswerString : "";
        }
    }
}
