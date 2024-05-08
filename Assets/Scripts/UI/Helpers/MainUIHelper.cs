using System.Collections.Generic;
using UI.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Helpers
{
    public class MainUIHelper : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private List<GameObject> spans;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void PlayRandomSpan()
        {
            var index = Random.Range(0, spans.Count);
            var span = spans[index];
            var spanPrefab = Instantiate(span, transform);
            spanPrefab.gameObject.SetActive(true);
        }

        private void AddListeners()
        {
            playButton.onClick.AddListener(PlayRandomSpan);
            Task.OnSpanRequested += PlayRandomSpan;
        }

        private void RemoveListeners()
        {
            playButton.onClick.RemoveListener(PlayRandomSpan);
            Task.OnSpanRequested -= PlayRandomSpan;
        }
    }
}
