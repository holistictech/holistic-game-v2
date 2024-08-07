using System;
using System.Collections.Generic;
using System.Linq;
using Scriptables.QuestionSystem;
using Scriptables.QuestionSystem.Images;
using Scriptables.QuestionSystem.Numbers;
using Spans.Skeleton.AnswerStates;
using TMPro;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Choice : MonoBehaviour
    {
        [SerializeField] protected Button choice;
        [SerializeField] protected Image choiceImage;
        [SerializeField] protected TextMeshProUGUI choiceValue;
        [SerializeField] protected Sprite numberChoiceSprite;
        
        protected GridUIHelper gridHelper;
        protected Question question;
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        public virtual void ConfigureUI(Question que, GridUIHelper grid)
        {
            gridHelper = grid;
            question = que;
            SetChoice();
        }

        protected virtual void SetChoice()
        {
            if (question is NumberQuestion)
            {
                choiceValue.text = $"{question.GetQuestionItem()}";
                choiceImage.sprite = numberChoiceSprite;
                choiceImage.type = Image.Type.Sliced;
            }
            else if (question is ImageQuestion)
            {
                choiceImage.sprite = (Sprite)question.GetQuestionItem();
                choiceImage.type = Image.Type.Simple;
            }
            else if (question is ClipQuestion)
            {
                var sprite = question.GetCorrectSprite();
                choiceValue.text = $"{question.GetCorrectText()}";
                choiceImage.sprite = sprite == null ? numberChoiceSprite : sprite;
            }

            EnableSelf();
        }

        public Question GetAssignedQuestionConfig()
        {
            return question;
        }

        public virtual void DisableSelf()
        {
            ResetUI();
            gameObject.SetActive(false);
        }

        protected void EnableSelf()
        {
            gameObject.SetActive(true);
        }

        public virtual void ResetUI()
        {
            choice.interactable = true;
        }

        protected void SetIsSelected()
        {
            //choice.interactable = false;
            gridHelper.SelectChoice(question, this);
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
