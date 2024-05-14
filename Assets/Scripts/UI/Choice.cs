using System;
using Scriptables.QuestionSystem;
using Spans.Skeleton.AnswerStates;
using TMPro;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Choice : MonoBehaviour
    {
        [SerializeField] private Button choice;
        [SerializeField] private Image choiceImage;
        [SerializeField] private TextMeshProUGUI choiceValue;
        [SerializeField] private Sprite numberChoiceSprite;
        
        private GridUIHelper _gridHelper;
        private Question _question;
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        public void ConfigureUI(Question question, GridUIHelper gridHelper)
        {
            _gridHelper = gridHelper;
            _question = question;
            SetChoice();
        }

        private void SetChoice()
        {
            if (_question is NumberQuestion)
            {
                choiceValue.text = $"{_question.GetQuestionItem()}";
                choiceImage.sprite = numberChoiceSprite;
                choiceImage.type = Image.Type.Sliced;
            }
            else if (_question is ImageQuestion)
            {
                choiceImage.sprite = (Sprite)_question.GetQuestionItem();
                choiceImage.type = Image.Type.Simple;
            }
            else if (_question is ClipQuestion)
            {
                var sprite = _question.GetCorrectSprite();
                choiceValue.text = $"{_question.GetCorrectText()}";
                choiceImage.sprite = sprite == null ? numberChoiceSprite : sprite;
            }

            EnableSelf();
        }

        public Question GetAssignedQuestionConfig()
        {
            return _question;
        }

        public void DisableSelf()
        {
            ResetUI();
            gameObject.SetActive(false);
        }

        private void EnableSelf()
        {
            gameObject.SetActive(true);
        }

        public void ResetUI()
        {
            choice.interactable = true;
        }

        private void SetIsSelected()
        {
            choice.interactable = false;
            _gridHelper.SelectChoice(_question, this);
        }

        private void AddListeners()
        {
            choice.onClick.AddListener(SetIsSelected);
        }

        private void RemoveListeners()
        {
            choice.onClick.RemoveListener(SetIsSelected);
        }
    }
}
