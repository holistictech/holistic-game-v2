using UnityEngine;
using Utilities;
using Utilities.Helpers;

namespace Scriptables.QuestionSystem.Images
{
    [CreateAssetMenu(fileName = "Question", menuName = "Question/Image")]
    public class ImageQuestion : Question
    {
        public Sprite Image;
        public override void SetQuestionItem(object item)
        {
            Image = (Sprite)item;
        }

        public override object GetQuestionItem()
        {
            return Image;
        }

        public override bool IsEqual(Question question)
        {
            return Image == (Sprite)question.GetQuestionItem();
        }

        public override object GetQuestionItemByType(CommonFields.ButtonType type)
        {
            throw new System.NotImplementedException();
        }
    }
}
