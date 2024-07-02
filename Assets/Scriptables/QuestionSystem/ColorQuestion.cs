using UnityEngine;
using Utilities;
using Utilities.Helpers;

namespace Scriptables.QuestionSystem
{
    [CreateAssetMenu(fileName = "Question", menuName = "Question/Color")]
    public class ColorQuestion : Question
    {
        public Color QuestionColor;
        public int Index;
        private Color _initialColor;
        public override void SetQuestionItem(object item)
        {
            _initialColor = QuestionColor;
            QuestionColor = (Color)item;
        }

        public override object GetQuestionItem()
        {
            return QuestionColor;
        }

        public override bool IsEqual(Question question)
        {
            throw new System.NotImplementedException();
        }

        private bool _isInitial = true;
        public override void ResetSelf()
        {
            if (!QuestionColor.Equals(_initialColor) && !_isInitial)
            {
                _isInitial = false;
                QuestionColor = _initialColor;
            }
        }

        public override object GetQuestionItemByType(CommonFields.ButtonType type)
        {
            return Index;
        }
    }
}
