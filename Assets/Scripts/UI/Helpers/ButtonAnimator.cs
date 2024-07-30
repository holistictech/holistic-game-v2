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
        [SerializeField] private bool animatable;
        [SerializeField] private Vector3 moveVector;
        
        [Header("Button Click")]
        [SerializeField] private Button button;
        [SerializeField] private AudioClip clickClip;
        
        
        private readonly float _amount = 0.08f;
        private Vector3 initialPosition;
        
        private void Awake()
        {
            AddListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void OnValidate()
        {
            button = GetComponent<Button>();
            initialPosition = transform.localPosition;
        }

        private void AnimateButtonForSwiping(ToggleUIEventButtons eventButtonsData)
        {
            var finalPos = eventButtonsData.Toggle
                ? initialPosition
                : initialPosition + moveVector;
            if(finalPos == transform.localPosition) return;
            transform.DOLocalMove(finalPos, 0.85f).SetEase(Ease.OutBack);
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
            if(button != null)
                button.onClick.AddListener(OnButtonClick);
            if(animatable)
                EventBus.Instance.Register<ToggleUIEventButtons>(AnimateButtonForSwiping);
        }

        private void RemoveListeners()
        {
            if(button != null)
                button.onClick.RemoveListener(OnButtonClick);
            if(animatable)
                EventBus.Instance.Unregister<ToggleUIEventButtons>(AnimateButtonForSwiping);
        }
    }
}
