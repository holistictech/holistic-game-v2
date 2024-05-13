using Scriptables.QuestionSystem;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CorsiBlockTypes
{
    public class CorsiBlock : MonoBehaviour
    {
        [SerializeField] protected Image blockImage;
        [SerializeField] protected Button blockButton;
        [SerializeField] protected Color highlightColor;
        private CorsiBlockUIHelper _blockHelper;
        private Question _blockQuestion;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        public void ConfigureSelf(Question question, CorsiBlockUIHelper helper)
        {
            if(_blockHelper == null)
                _blockHelper = helper;
            _blockQuestion = question;
            EnableSelf();
        }

        public virtual void AnimateSelf()
        {
        }

        public virtual void ResetUI()
        {
        }

        private void SetSelected()
        {
            blockImage.color = highlightColor;
            _blockHelper.AppendSelectedAnswers(_blockQuestion, this);
        }

        private void EnableSelf()
        {
            gameObject.SetActive(true);
        }

        public void ConfigureInput(bool toggle)
        {
            blockButton.transition = toggle ? Selectable.Transition.ColorTint : Selectable.Transition.None;
            blockButton.interactable = toggle;
        }

        public Question GetAssignedQuestion()
        {
            return _blockQuestion;
        }

        private void AddListeners()
        {
            blockButton.onClick.AddListener(SetSelected);
        }

        private void RemoveListeners()
        {
            blockButton.onClick.RemoveListener(SetSelected);
        }
    }
}
