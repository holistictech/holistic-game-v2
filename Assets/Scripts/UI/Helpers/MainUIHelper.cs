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

        private void PlayRandomSpan()
        {
            /*var index = Random.Range(0, spans.Count);
            var span = spans[0];
            _activeSpan = Instantiate(span, transform);
            _activeSpan.gameObject.SetActive(true);*/
            selectionPopup.gameObject.SetActive(true);
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
                energyHelper.UpdateEnergyField();
            });
        }

        private void AddListeners()
        {
            playButton.onClick.AddListener(PlayRandomSpan);
            Task.OnSpanRequested += PlayRandomSpan;
            SpanController.OnSpanFinished += DestroyActiveSpan;
        }

        private void RemoveListeners()
        {
            playButton.onClick.RemoveListener(PlayRandomSpan);
            Task.OnSpanRequested -= PlayRandomSpan;
            SpanController.OnSpanFinished += DestroyActiveSpan;
        }
    }
}
