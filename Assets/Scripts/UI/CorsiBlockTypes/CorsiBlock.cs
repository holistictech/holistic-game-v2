using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        protected Question blockQuestion;

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
            blockQuestion = question;
            EnableSelf();
        }

        public virtual void AnimateSelf()
        {
            blockImage.DOColor(highlightColor, 1f).SetEase(Ease.Flash).OnComplete(ResetUI);
        }

        public virtual void MakeBlockMove()
        {
            
        }

        public virtual IEnumerator MoveSelf()
        {
            yield return null;
        }

        public virtual void ResetUI()
        {
            //blockImage.sprite = null;
            blockImage.color = Color.white;
            blockButton.interactable = true;
        }

        protected virtual void SetSelected()
        {
            blockImage.color = highlightColor;
            blockButton.interactable = false;
            AppendSelf(blockQuestion);
        }

        protected void AppendSelf(Question question)
        {
            _blockHelper.AppendSelectedAnswers(question, this);
        }

        private void EnableSelf()
        {
            gameObject.SetActive(true);
        }

        public void DisableSelf()
        {
            gameObject.SetActive(false);
        }

        public void ConfigureInput(bool toggle)
        {
            blockButton.transition = toggle ? Selectable.Transition.ColorTint : Selectable.Transition.None;
            blockButton.interactable = toggle;
        }

        public Question GetAssignedQuestion()
        {
            return blockQuestion;
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
