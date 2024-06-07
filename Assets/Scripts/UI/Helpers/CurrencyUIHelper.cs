using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;
using Utilities.Helpers;

namespace UI.Helpers
{
    public class CurrencyUIHelper : MonoBehaviour
    {
        [SerializeField] private Image fieldImage;
        [SerializeField] private Sprite currencySprite;
        [SerializeField] private TextMeshProUGUI currencyCountField;
        [SerializeField] private CommonFields.CurrencyType currencyType;

        public void UpdateCurrencyField()
        {
            currencyCountField.text = $"{(currencyType == CommonFields.CurrencyType.Energy ? PlayerInventory.Instance.Energy : PlayerInventory.Instance.Performance)}";
        }

        public Sprite GetFieldSprite()
        {
            return currencySprite;
        }

        public RectTransform GetTargetAnimationPos()
        {
            return fieldImage.rectTransform;
        }
    }
}
