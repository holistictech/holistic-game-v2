using System;
using DG.Tweening;
using Spans.Skeleton.AnswerStates;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitCircle : MonoBehaviour
    {
        [SerializeField] private Image circleImage;
        [SerializeField] private Color activeColor;

        private int _circleIndex = -1;
        private Tween _jump;
        
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        public void ConfigureUI(int index)
        {
            _circleIndex = index;
            circleImage.DOColor(activeColor, 1f).SetEase(Ease.Flash).OnComplete(ResetSelf);
        }

        public void OnAnswerGiven()
        {
            circleImage.color = activeColor;
            ResetScale();
        }

        public void OnAnswerRevoked()
        {
            ResetSelf();
        }

        public void AnimateCircle()
        {
            _jump = circleImage.transform.DOScale(new Vector3(1f, 1f, 1f), .4f).SetEase(Ease.OutBack)
                .SetLoops(-1, LoopType.Yoyo);
            _jump.Play();
        }

        private void DisableCircle()
        {
            ResetSelf();
            DisableSelf();
        }

        public void EnableSelf()
        {
            ResetScale();
            gameObject.SetActive(true);
        }

        public void DisableSelf()
        {
            _circleIndex = -1;
            gameObject.SetActive(false);
        }

        public void ResetSelf()
        {
            ResetScale();
            circleImage.color = Color.white;
        }

        private void ResetScale()
        {
            _jump.Kill();
            transform.localScale = new Vector3(.8f, .8f, .8f);
        }

        private void AddListeners()
        {
            GridUIHelper.OnChoiceSelected += DisableCircle;
        }

        private void RemoveListeners()
        {
            GridUIHelper.OnChoiceSelected -= DisableCircle;
        }
    }
}
