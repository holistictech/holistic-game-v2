using UnityEngine;
using Utilities;

namespace Scriptables.QuestionSystem.Images
{
    [CreateAssetMenu(fileName = "Question", menuName = "Question/Image")]
    public class ImageQuestion : Question
    {
        public Sprite Image;
        public override object GetQuestionItem()
        {
            return Image;
        }

        public override object GetQuestionItemByType(CommonFields.ButtonType type)
        {
            throw new System.NotImplementedException();
        }
    }
}
