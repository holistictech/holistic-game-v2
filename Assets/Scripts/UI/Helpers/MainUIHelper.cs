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
        [SerializeField] private Button playButton;
        [SerializeField] private EnergyUIHelper energyHelper;
        [SerializeField] private List<GameObject> spans;

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
            var index = Random.Range(0, spans.Count);
            var span = spans[index];
            _activeSpan = Instantiate(span, transform);
            _activeSpan.gameObject.SetActive(true);
        }

        private void DestroyActiveSpan()
        {
            Destroy(_activeSpan.gameObject);
            PlayerInventory.Instance.ChangeEnergyAmount(2);
            energyHelper.UpdateEnergyField();
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
