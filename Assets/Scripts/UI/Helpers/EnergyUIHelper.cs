using TMPro;
using UnityEngine;
using Utilities;

namespace UI.Helpers
{
    public class EnergyUIHelper : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI energyCountField;

        public void UpdateEnergyField()
        {
            energyCountField.text = $"{PlayerInventory.Instance.Energy}";
        }
    }
}
