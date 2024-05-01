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

        public override object GetCorrectSprite()
        {
            return ClipSprite;
        }
    }
}
