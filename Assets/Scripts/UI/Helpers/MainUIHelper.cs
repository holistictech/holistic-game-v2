using System.Collections.Generic;
using Spans.Skeleton;
using UI.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

namespace UI.Helpers
{
    public class MainUIHelper : MonoBehaviour
    {
        [SerializeField] private CurrencyTrailHelper trailHelper;
        [SerializeField] private Button playButton;
        [SerializeField] private CurrencyUIHelper currencyHelper;
        [SerializeField] private CurrencyUIHelper performanceHelper;
        [SerializeField] private List<GameObject> spans;
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

        private void PlayNextSpan()
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
            //PlayNextSpan();
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
        }

        private void DestroyActiveSpan(int earnedPerformance)
        {
            Camera.main.gameObject.transform.rotation = Quaternion.Euler(new Vector3(30, 45, 0));
            Destroy(_activeSpan.gameObject);
            trailHelper.AnimateCurrencyIncrease(currencyHelper, 1, () =>
            {
                PlayerInventory.Instance.ChangeEnergyAmount(1);
                PlayerInventory.Instance.IncrementCurrentStage();
                currencyHelper.UpdateCurrencyField();
                trailHelper.AnimateCurrencyIncrease(performanceHelper, earnedPerformance, () =>
                {
                    PlayerInventory.Instance.ChangePerformanceAmount(earnedPerformance);
                    performanceHelper.UpdateCurrencyField();
                });
            });
        }

        private void AddListeners()
        {
            playButton.onClick.AddListener(EnableSpanChooser);
            closeButton.onClick.AddListener(DisableSpanChooser);
            Task.OnSpanRequested += EnableSpanChooser;
            SpanController.OnSpanFinished += DestroyActiveSpan;
        }

        private void RemoveListeners()
        {
            playButton.onClick.RemoveListener(EnableSpanChooser);
            closeButton.onClick.RemoveListener(DisableSpanChooser);
            Task.OnSpanRequested -= EnableSpanChooser;
            SpanController.OnSpanFinished += DestroyActiveSpan;
        }
    }
}
