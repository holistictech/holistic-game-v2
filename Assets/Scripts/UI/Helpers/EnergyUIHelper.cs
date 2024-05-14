using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

namespace UI.Helpers
{
    public class EnergyUIHelper : MonoBehaviour
    {
        [SerializeField] private Image fieldImage;
        [SerializeField] private Sprite energySprite;
        [SerializeField] private TextMeshProUGUI energyCountField;

        public void UpdateEnergyField()
        {
            energyCountField.text = $"{PlayerInventory.Instance.Energy}";
        }

        public Sprite GetFieldSprite()
        {
            return energySprite;
        }

        public RectTransform GetTargetAnimationPos()
        {
            return fieldImage.rectTransform;
        }
    }
}
