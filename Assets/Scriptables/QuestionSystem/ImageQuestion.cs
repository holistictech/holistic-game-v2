using UnityEngine;

namespace Scriptables.QuestionSystem
{
    [CreateAssetMenu(fileName = "Question", menuName = "Question/Image")]
    public class ImageQuestion : Question
    {
        public Sprite Image;
    }
}
