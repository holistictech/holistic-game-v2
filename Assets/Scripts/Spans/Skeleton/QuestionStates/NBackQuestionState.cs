using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.NBack;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Spans.Skeleton.QuestionStates
{
    public class NBackQuestionState : SpanQuestionState
    {
        [SerializeField] private CorsiBlockUIHelper blockUIHelper;
        [SerializeField] private HorizontalLayoutGroup horizontalParent;
        [SerializeField] private Image questionImage;

        private NBack.NBack _nBackController;
        private INBackStrategy _currentStrategy;
        
        private List<Question> _spanObjects; 
        private List<Question> _currentQuestions = new List<Question>();

        private List<Image> _pooledQuestionImages = new List<Image>();
        private List<Image> _activeQuestionImages = new List<Image>();
        private const int SpawnAmount = 6;

        protected override void Start()
        {
            base.Start();
            SpawnPool();
        }

        private bool _isInitial;
        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                _nBackController = controller.GetComponent<NBack.NBack>();
                _currentStrategy = _nBackController.GetStrategyClass();
                _isInitial = true;
                base.Enter(controller);
                currentQuestionIndex = 0;
            }
            blockUIHelper.ConfigureInput(false);
            _spanObjects = spanController.GetSpanObjects();
            SetCircleUI(_isInitial ? 2 : 1);
            _isInitial = false;
            if(_nBackController.GetCorrectStatus())
                HighlightCorrectUnit();
            EnableUIElements();
            ShowQuestion();
            StatisticsHelper.IncrementDisplayedQuestionCount();
        }
        
        public override void ShowQuestion()
        {
            if (_currentStrategy is NBackMode)
            {
                _currentStrategy.InjectQuestionState(this);
                _currentStrategy.ShowQuestion();
                displayingQuestions = StartCoroutine(IterateBlocks());
            }
            else
            {
                displayingQuestions = StartCoroutine(IterateQuestions());
            }
        }
        
        private bool _initialDisplay = true;
        private IEnumerator IterateQuestions()
        {
            var start = _initialDisplay ? 0 : 1;
            _initialDisplay = false;
            for (int i = start; i < _spanObjects.Count; i++)
            {
                var question = _spanObjects[i];
                if (question.SpawnAmount > 1)
                {
                    ShowMultipleImages(question, currentQuestionIndex);
                }
                else
                {
                    ShowImage(question, currentQuestionIndex); 
                }

                currentQuestionIndex++;
                yield return new WaitForSeconds(1f);
                DisableActiveImages();
                yield return new WaitForSeconds(1f);
            }

            DOVirtual.DelayedCall(1f, SwitchNextState);
        }

        private void ShowMultipleImages(Question question, int index)
        {
            for (int i = 0; i < question.SpawnAmount; i++)
            {
                var availableImage = GetAvailableImage();
                _activeQuestionImages.Add(availableImage);
                availableImage.sprite = (Sprite)question.GetQuestionItem();
                availableImage.gameObject.SetActive(true);
                _currentQuestions.Add(question);
            }
            ActivateCircle(index);
        }

        private void ShowImage(Question question, int index)
        {
            var availableImage = GetAvailableImage();
            _activeQuestionImages.Add(availableImage);
            availableImage.sprite = (Sprite)question.GetQuestionItem();
            availableImage.gameObject.SetActive(true);
            ActivateCircle(index);
            _currentQuestions.Add(question);
        }

        private void PlayAudio(Question question)
        {
            
        }
        
        
        private IEnumerator IterateBlocks()
        {
            var start = _initialDisplay ? 0 : 1;
            _initialDisplay = false;
            for (int i = start; i < _spanObjects.Count; i++)
            {
                ActivateCircle(i);
                blockUIHelper.HighlightTargetBlock(_spanObjects[i]);
                //currentQuestionIndex++;
                yield return new WaitForSeconds(2f);
            }
            
            DOVirtual.DelayedCall(1f, SwitchNextState);
        }

        private void HighlightCorrectUnit()
        {
            HighlightPreviousCircle();
        }
        public CorsiBlockUIHelper GetBlockHelper()
        {
            return blockUIHelper;
        }
        
        public override void Exit()
        {
            if (displayingQuestions != null)
            {
                StopCoroutine(displayingQuestions);
            }
            //ResetPreviousCircles();
            DisableActiveImages();
            DisableUIElements();
        }
        
        public override void SwitchNextState()
        {
            spanController.SetCurrentDisplayedQuestions(_currentQuestions);
            spanController.SwitchState();
        }
        
        public override void EnableUIElements()
        {
            if (_currentStrategy is NBackMode)
            {
                blockUIHelper.gameObject.SetActive(true);
                blockUIHelper.ResetCorsiBlocks();
            }
            else
            {
                horizontalParent.gameObject.SetActive(true);
            }
            unitParent.gameObject.SetActive(true);
        }

        public override void DisableUIElements()
        {
            if(_currentStrategy is not IsIdenticalMode)
                horizontalParent.gameObject.SetActive(false);
            blockUIHelper.gameObject.SetActive(false);
            blockUIHelper.ResetCorsiBlocks();
        }

        private void SpawnPool()
        {
            for (int i = 0; i < SpawnAmount; i++)
            {
                var tempImage = Instantiate(questionImage, horizontalParent.transform);
                tempImage.gameObject.SetActive(false);
                _pooledQuestionImages.Add(tempImage);
            }
        }

        private Image GetAvailableImage()
        {
            foreach (var image in _pooledQuestionImages)
            {
                if (!image.gameObject.activeSelf)
                {
                    return image;
                }
            }

            throw new Exception("Could not find available image");
        }

        private void DisableActiveImages()
        {
            foreach (var image in _activeQuestionImages)
            {
                image.gameObject.SetActive(false);
            }
        }
    }
}
