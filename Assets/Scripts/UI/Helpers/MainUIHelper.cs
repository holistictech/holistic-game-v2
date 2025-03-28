using DG.Tweening;
using Spans.Skeleton;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Helpers;

namespace UI.Helpers
{
    public class MainUIHelper : MonoBehaviour
    {
        [SerializeField] private LoadingScreen loadingScreen;
        [SerializeField] private CurrencyTrailHelper trailHelper;
        [SerializeField] private Button playButton;
        [SerializeField] private CurrencyUIHelper currencyHelper;
        [SerializeField] private CurrencyUIHelper performanceHelper;
        [SerializeField] private GameObject worldParent;
        [SerializeField] private Image selectionPopup;
        [SerializeField] private Button closeButton;
        [SerializeField] private DayDoneUIHelper dayCompletedPopup;

        private GameObject _activeSpan;

        private void OnEnable()
        {
            AddListeners();
            currencyHelper.UpdateCurrencyField();
            performanceHelper.UpdateCurrencyField();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void PlayNextSpan(SpanRequestedEvent request)
        {
            if (PlayerInventory.Instance.HasDayCompleted())
            {
                dayCompletedPopup.EnableSelf();
                return;
            }
            
            var span = PlayerInventory.Instance.GetNextSpan();
            PlaySelectedSpan(span);
        }

        private void EnableSpanChooser()
        {
            selectionPopup.gameObject.SetActive(true);
            //PlayNextSpan(new SpanRequestedEvent());
        }

        private void DisableSpanChooser()
        {
            selectionPopup.gameObject.SetActive(false);
        }

        public void PlaySelectedSpan(GameObject span)
        {
            _activeSpan = Instantiate(span, transform);
            _activeSpan.gameObject.SetActive(true);
            selectionPopup.gameObject.SetActive(false);
            Camera.main.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            worldParent.gameObject.SetActive(false);
        }

        private void DestroyActiveSpan(int earnedPerformance)
        {
            worldParent.gameObject.SetActive(true);
            Destroy(_activeSpan.gameObject);
            Camera.main.gameObject.transform.rotation = Quaternion.Euler(new Vector3(30, 45, 0));
            DOVirtual.DelayedCall(0.3f, () =>
            {
                trailHelper.AnimateCurrencyIncrease(currencyHelper, 1, () =>
                {
                    PlayerInventory.Instance.ChangeEnergyAmount(1);
                    PlayerInventory.Instance.IncrementCurrentStage();
                    currencyHelper.UpdateCurrencyField();
                    trailHelper.AnimateCurrencyIncrease(performanceHelper, earnedPerformance, () =>
                    {
                        PlayerInventory.Instance.ChangePerformanceAmount(earnedPerformance);
                        performanceHelper.UpdateCurrencyField();
                        EventBus.Instance.Trigger(new SpanCompletedEvent());
                    });
                });
            });
        }

        private void TriggerLoadingScreen(GameLoadingEvent eventData)
        {
            Debug.Log("Event triggered with: " + eventData.HasFinished);
            loadingScreen.ConfigureScreen(eventData.HasFinished);
        }

        private void AddListeners()
        {
            playButton.onClick.AddListener(EnableSpanChooser);
            closeButton.onClick.AddListener(DisableSpanChooser);
            EventBus.Instance.Register<SpanRequestedEvent>(PlayNextSpan);
            EventBus.Instance.Register<GameLoadingEvent>(TriggerLoadingScreen);
            SpanController.OnSpanFinished += DestroyActiveSpan;
        }

        private void RemoveListeners()
        {
            playButton.onClick.RemoveListener(EnableSpanChooser);
            closeButton.onClick.RemoveListener(DisableSpanChooser);
            EventBus.Instance.Unregister<SpanRequestedEvent>(PlayNextSpan);
            EventBus.Instance.Unregister<GameLoadingEvent>(TriggerLoadingScreen);
            SpanController.OnSpanFinished += DestroyActiveSpan;
        }
    }
}
