using System;
using DG.Tweening;
using Scriptables.QuestionSystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace UI
{
    public class Option : MonoBehaviour
    {
        [SerializeField] private Image optionBg;
        [SerializeField] private Button optionButton;
        [SerializeField] private Image optionImage; 

        private OptionPicker _parent;
        private Question _question;

        private void OnValidate()
        {
            optionBg = GetComponent<Image>();
            optionButton = GetComponent<Button>();
        }

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
            ResetOption();
        }

        public void ConfigureOption(Question question)
        {
            _question = question;
            
            if (question is ImageQuestion)
            {
                optionImage.sprite = (Sprite)question.GetQuestionItem();
                optionBg.color = Color.grey;
            }
            else if (question is ColorQuestion)
            {
                optionBg.color = (Color)question.GetQuestionItem();
                optionImage.enabled = false;
            }
            
            gameObject.SetActive(true);
        }

        private void MakeSelected()
        {
            _parent.SetOptionActive(this);
            ScaleOption(1.2f);
        }

        public void ScaleOption(float endValue)
        {
            transform.DOScale(new Vector3(endValue, endValue, endValue), 0.2f).SetEase(Ease.OutBack);
        }

        public Question GetQuestion()
        {
            return _question;
        }

        private void ResetOption()
        {
            optionImage.enabled = true;
            optionBg.color = Color.white;
        }

        private void AddListeners()
        {
            optionButton.onClick.AddListener(MakeSelected);
        }

        private void RemoveListeners()
        {
            optionButton.onClick.RemoveListener(MakeSelected);
        }
    }
}
