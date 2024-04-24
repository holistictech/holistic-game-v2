using System;
using Scriptables.QuestionSystem;
using Spans.Skeleton.AnswerStates;
using TMPro;
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

        private MultipleChoiceAnswerState _answerState;
        private Question _question;
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        public void ConfigureUI(Question question, MultipleChoiceAnswerState answerState)
        {
            _answerState = answerState;
            _question = question;
            SetChoice();
        }

        private void SetChoice()
        {
            if (_question is NumberQuestion)
            {
                choiceValue.text = $"{_question.GetQuestionItem()}";
                choiceImage.sprite = numberChoiceSprite;
            }
            else if (_question is ImageQuestion)
            {
                choiceImage.sprite = (Sprite)_question.GetQuestionItem();
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
            _answerState.AppendGivenAnswers(_question, this);
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
