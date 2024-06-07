using UnityEngine;
using Utilities;
using Utilities.Helpers;

namespace Scriptables.QuestionSystem
{
    [CreateAssetMenu(fileName = "Question", menuName = "Question/NBackQuestion")]
    public class NBackQuestion : Question
    {
        public Sprite ItemSprite;
        public Sprite AlternativeColorSprite;
        
        public override object GetQuestionItem()
        {
            return ItemSprite;
        }

        public override object GetQuestionItemByType(CommonFields.ButtonType type)
        {
            return type == CommonFields.ButtonType.Color ? AlternativeColorSprite : ItemSprite;
        }
    }
}
