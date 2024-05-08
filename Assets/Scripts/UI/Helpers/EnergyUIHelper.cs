using TMPro;
using UnityEngine;

namespace UI.Helpers
{
    public class EnergyUIHelper : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI energyCountField;

        public void SetEnergyCount(int amount)
        {
            energyCountField.text = $"{amount}";
        }
    }
}
