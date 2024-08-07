using System.Collections.Generic;
using Scriptables.QuestionSystem;
using TMPro;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Helpers;

namespace UI
{
    public class ComplexChoice : Choice
    {
        private ComplexShapeQuestion _question;

        public override void ConfigureUI(Question que, GridUIHelper grid)
        {
            gridHelper = grid;
            question = que;
            _question = (ComplexShapeQuestion)question;
        }

        protected override void SetChoice()
        {
            choiceImage.sprite = _question.Shape;
            choiceImage.color = _question.QuestionColor;
            choiceValue.text = $"{_question.Index}";
            EnableSelf();
        }
        
        protected override void SetIsSelected()
        {
            gridHelper.SelectChoice(_question, this);
        }
    }
}
