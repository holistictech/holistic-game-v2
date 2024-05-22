using System.Collections.Generic;
using Spans.Skeleton;
using UI.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.Helpers
{
    public class MainUIHelper : MonoBehaviour
    {
        [SerializeField] private CurrencyTrailHelper trailHelper;
        [SerializeField] private Button playButton;
        [SerializeField] private EnergyUIHelper energyHelper;
        [SerializeField] private List<GameObject> spans;
        [SerializeField] private Image selectionPopup;
        [SerializeField] private Button closeButton;

        private GameObject _activeSpan;

        private void OnEnable()
        {
            AddListeners();
            energyHelper.UpdateEnergyField();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void PlayNextSpan()
        {
            var span = PlayerInventory.Instance.GetNextSpan();
            PlaySelectedSpan(span);
        }

        private void EnableSpanChooser()
        {
            /*var index = Random.Range(0, spans.Count);
            var span = spans[0];
            _activeSpan = Instantiate(span, transform);
            _activeSpan.gameObject.SetActive(true);*/
            selectionPopup.gameObject.SetActive(true);
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
        }

        private void DestroyActiveSpan()
        {
            Destroy(_activeSpan.gameObject);
            trailHelper.AnimateTrail(energyHelper, () =>
            {
                PlayerInventory.Instance.ChangeEnergyAmount(1);
                PlayerInventory.Instance.IncrementCurrentStage();
                energyHelper.UpdateEnergyField();
            });
        }

        private void AddListeners()
        {
            playButton.onClick.AddListener(PlayNextSpan);
            closeButton.onClick.AddListener(DisableSpanChooser);
            Task.OnSpanRequested += PlayNextSpan;
            SpanController.OnSpanFinished += DestroyActiveSpan;
        }

        private void RemoveListeners()
        {
            playButton.onClick.RemoveListener(PlayNextSpan);
            closeButton.onClick.RemoveListener(DisableSpanChooser);
            Task.OnSpanRequested -= PlayNextSpan;
            SpanController.OnSpanFinished += DestroyActiveSpan;
        }
    }
}
