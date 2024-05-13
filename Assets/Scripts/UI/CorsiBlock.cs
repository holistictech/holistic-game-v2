using System;
using DG.Tweening;
using Scriptables.QuestionSystem;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CorsiBlock : MonoBehaviour
    {
        [SerializeField] private Image blockImage;
        [SerializeField] private Sprite eggSprite;
        [SerializeField] private Sprite chickSprite;
        [SerializeField] private ParticleSystem dustEffect;
        [SerializeField] private Button blockButton;
        [SerializeField] private Color highlightColor;
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
            //blockImage.DOColor(highlightColor, 1f).SetEase(Ease.Flash).OnComplete(ResetUI);

            blockImage.transform.DOShakeScale(0.5f, .3f).SetEase(Ease.Flash).OnComplete(() =>
            {
                dustEffect.Play();
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    blockImage.sprite = chickSprite;
                    DOVirtual.DelayedCall(0.9f, ResetUI);
                });
            });
        }

        public void ResetUI()
        {
            blockImage.sprite = eggSprite;
            blockImage.color = Color.white;
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
