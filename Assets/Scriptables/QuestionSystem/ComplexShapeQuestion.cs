using UnityEngine;
using Utilities.Helpers;

namespace Scriptables.QuestionSystem
{
    [CreateAssetMenu(fileName = "Question", menuName = "Question/ComplexShape")]
    public class ComplexShapeQuestion : Question
    {
        public Color QuestionColor;
        public int Index;
        public Sprite Shape;
        
        public override void SetQuestionItem(object item)
        {
            QuestionColor = (Color)item;
        }

        public override object GetQuestionItem()
        {
            return QuestionColor;
        }

        public override bool IsEqual(Question question)
        {
            var complex = (ComplexShapeQuestion)question;

            return QuestionColor == complex.QuestionColor && Index == complex.Index && Shape == complex.Shape;
        }

        public override object GetQuestionItemByType(CommonFields.ButtonType type)
        {
            return Shape;
        }
    }
}
