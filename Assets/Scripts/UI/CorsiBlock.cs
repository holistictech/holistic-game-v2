using System;
using Scriptables.QuestionSystem;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CorsiBlock : MonoBehaviour
    {
        [SerializeField] private Image blockImage;
        [SerializeField] private Button blockButton;

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

        public void AnimateSelf()
        {
            
        }

        public void ResetUI()
        {
            
        }

        private void SetSelected()
        {
        }

        public void DisableSelf()
        {
            gameObject.SetActive(false);
        }

        private void EnableSelf()
        {
            gameObject.SetActive(true);
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
