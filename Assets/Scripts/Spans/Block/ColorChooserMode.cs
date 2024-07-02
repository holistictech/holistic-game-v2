using System;
using System.Collections.Generic;
using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using UI.CorsiBlockTypes;
using UnityEngine;
using Utilities.Helpers;
using Random = UnityEngine.Random;

namespace Spans.BlockSpan
{
    public class ColorChooserMode : IBlockSpanStrategy
    {
        public void HighlightBlock(AdaptableBlock targetBlock)
        {
            var image = targetBlock.GetBlockImage();
            var color = targetBlock.GetHighlightColor();
            image.DOColor(color, 1f).SetEase(Ease.Flash).OnComplete(targetBlock.ResetUI);
        }

        public void SetBlockSelected(AdaptableBlock block, Question selection)
        {
            var image = block.GetBlockImage();
            image.color = (Color)selection.GetQuestionItem();
        }

        public bool CheckAnswer(List<Question> displayed, List<Question> given)
        {
            // Check if counts match and neither list is empty
            if (given.Count == 0 || given.Count != displayed.Count)
            {
                return false;
            }

            // Create lists of tuples to map question items to their colors
            var displayedList = new List<(object color, int index)>();
            var givenList = new List<(object color, int index)>();

            // Populate the displayedList with question items and their colors
            foreach (var question in displayed)
            {
                var color = question.GetQuestionItem();
                var index = (int)question.GetQuestionItemByType(CommonFields.ButtonType.Color);
                displayedList.Add((color, index));
            }

            // Populate the givenList with user's answers
            foreach (var question in given)
            {
                var color = question.GetQuestionItem();
                var index = (int)question.GetQuestionItemByType(CommonFields.ButtonType.Color);
                givenList.Add((color, index));
            }

            // Use a HashSet to track matched items
            var matchedItems = new HashSet<(object color, int index)>();

            // Check each item in displayedList against givenList
            foreach (var displayedItem in displayedList)
            {
                bool matchFound = false;
                for (int i = 0; i < givenList.Count; i++)
                {
                    var givenItem = givenList[i];
                    if (!matchedItems.Contains(givenItem) && displayedItem.color.Equals(givenItem.color) && displayedItem.index.Equals(givenItem.index))
                    {
                        matchFound = true;
                        matchedItems.Add(givenItem); // Add to matched items
                        break;
                    }
                }
                if (!matchFound)
                {
                    return false;
                }
            }

            // If all checks pass, return true
            return true;
        }



        public List<Question> GetCorrectQuestions(List<Question> allQuestions, int count = -1)
        {
            var selectedQuestions = new List<Question>();
            var usedQuestionIndices = new HashSet<int>();

            count = count == -1 ? allQuestions.Count : Math.Min(count, allQuestions.Count);
            
            var blueQuestions = new List<Question>();
            var orangeQuestions = new List<Question>();
            foreach (var question in allQuestions)
            {
                var color = (Color)question.GetQuestionItem();
                
                if (IsBlue(color))
                {
                    blueQuestions.Add(question);
                }
                else if (IsOrange(color))
                {
                    orangeQuestions.Add(question);
                }
            }
            blueQuestions.Shuffle();
            orangeQuestions.Shuffle();
            
            if (blueQuestions.Count > 0)
            {
                selectedQuestions.Add(blueQuestions[0]);
                usedQuestionIndices.Add(allQuestions.IndexOf(blueQuestions[0]));
            }
            if (orangeQuestions.Count > 0)
            {
                selectedQuestions.Add(orangeQuestions[0]);
                usedQuestionIndices.Add(allQuestions.IndexOf(orangeQuestions[0]));
            }

            for (int i = selectedQuestions.Count; i < count; i++)
            {
                int randomIndex;
                do
                {
                    randomIndex = Random.Range(0, allQuestions.Count);
                } while (usedQuestionIndices.Contains(randomIndex));

                usedQuestionIndices.Add(randomIndex);
                selectedQuestions.Add(allQuestions[randomIndex]);
            }

            return selectedQuestions;
        }
        
        private bool IsBlue(Color color)
        {
            return Mathf.Approximately(color.r, 0f) && Mathf.Approximately(color.g, 29f / 255f) && Mathf.Approximately(color.b, 1f);
        }

        private bool IsOrange(Color color)
        {
            return Mathf.Approximately(color.r, 1f) && Mathf.Approximately(color.g, 145f / 255f) && Mathf.Approximately(color.b, 4f / 255f);
        }


    }
}
