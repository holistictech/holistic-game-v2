using UnityEngine;
using UnityEngine.Serialization;
using Utilities.Helpers;

namespace Scriptables.QuestionSystem
{
    [CreateAssetMenu(fileName = "Question", menuName = "Question/BlockImage")]
    public class BlockImageQuestion : Question
    {
        public Sprite Sprite;
        public int Index;
        private Sprite _initialSprite;
        
        public override void SetQuestionItem(object item)
        {
            Sprite = (Sprite)item;
        }

        public override object GetQuestionItem()
        {
            return Sprite;
        }

        public override bool IsEqual(Question question)
        {
            throw new System.NotImplementedException();
        }

        private bool _isInitial = true;

        public override void ResetSelf()
        {
            if (Sprite != _initialSprite && !_isInitial)
            {
                Sprite = _initialSprite;
                _isInitial = false;
            }
        }
        
        public override object GetQuestionItemByType(CommonFields.ButtonType type)
        {
            return Index;
        }
    }
}
