using System;
using DG.Tweening;
using Spans.Skeleton;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Utilities.Helpers.CommonFields;

namespace UI.Helpers
{
    public class ButtonAnimator : MonoBehaviour
    {
        [Header("Animation Attributes")] 
        [SerializeField] private Vector3 moveVector;
        
        [Header("Button Click")]
        [SerializeField] private Button button;
        [SerializeField] private AudioClip clickClip;
        private readonly float _amount = 0.08f;
        
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void OnValidate()
        {
            button = GetComponent<Button>();
        }

        private void AnimateButtonForSwiping(ToggleUIEvent eventData)
        {
            var localPos = transform.localPosition; 
            var finalPos = eventData.Toggle
                ? localPos
                : localPos + moveVector; 
            transform.DOLocalMove(finalPos, 0.35f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                moveVector *= -1;
            });
        }

        private void OnButtonClick()
        {
            Sequence buttonSequence = DOTween.Sequence();
            if(clickClip != null)
                AudioManager.Instance.PlayAudioClip(clickClip);
            buttonSequence.Append(button.transform.DOPunchScale(new Vector3(-_amount, -_amount, -_amount), 0.15f).SetEase(Ease.OutQuad).OnComplete(
                () =>
                {
                    button.transform.localScale = new Vector3(1, 1, 1);
                }));
        }

        private void AddListeners()
        {
            button.onClick.AddListener(OnButtonClick);
            EventBus.Instance.Register<ToggleUIEvent>(AnimateButtonForSwiping);
        }

        private void RemoveListeners()
        {
            button.onClick.RemoveListener(OnButtonClick);
            EventBus.Instance.Unregister<ToggleUIEvent>(AnimateButtonForSwiping);
        }
    }
}
