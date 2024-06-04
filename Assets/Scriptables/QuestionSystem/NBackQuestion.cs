using UnityEngine;
using Utilities;

namespace Scriptables.QuestionSystem
{
    public class NBackQuestion : Question
    {
        public Sprite ItemSprite;
        public Sprite AlternativeColorSprite;
        public int SpawnAmount = 1;
        
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
