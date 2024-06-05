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
        private const int SpawnAmount = 3;

        protected override void Start()
        {
            base.Start();
            SpawnPool();
        }

        private bool _isInitial = true;
        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                base.Enter(controller);
                blockUIHelper.GetCorsiBlocks();
                _nBackController = controller.gameObject.GetComponent<NBack.NBack>();
            }

            blockUIHelper.ConfigureInput(false);
            _currentStrategy = _nBackController.GetStrategyClass();
            _spanObjects = spanController.GetSpanObjects();
            EnableUIElements();
            SetCircleUI(spanController.GetRoundIndex());
            //if(!_isInitial)
                //HighlightPreviousCircle();
            //_isInitial = false;
            ShowQuestion();
            StatisticsHelper.IncrementDisplayedQuestionCount();
        }
        
        public override void ShowQuestion()
        {
            currentQuestionIndex = 0;
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

        private IEnumerator IterateQuestions()
        {
            for (int i = 0; i < _spanObjects.Count; i++)
            {
                var question = _spanObjects[currentQuestionIndex];
                for(int j = 0; j < question.SpawnAmount; j++)
                {
                   ShowImage(question); 
                   yield return new WaitForSeconds(1f);
                   DisableActiveImages();
                   yield return new WaitForSeconds(1f);
                }
            }

            DOVirtual.DelayedCall(1f, SwitchNextState);
        }

        private void ShowImage(Question question)
        {
            var availableImage = GetAvailableImage();
            _activeQuestionImages.Add(availableImage);
            availableImage.sprite = (Sprite)question.GetQuestionItem();
            availableImage.gameObject.SetActive(true);
            ActivateCircle(currentQuestionIndex);
            _currentQuestions.Add(question);    
            currentQuestionIndex++;
        }

        private void PlayAudio(Question question)
        {
            
        }
        
        
        private IEnumerator IterateBlocks()
        {
            for (int i = 0; i < _spanObjects.Count; i++)
            {
                if (currentQuestionIndex >= _spanObjects.Count)
                {
                    break;
                }
                ActivateCircle(currentQuestionIndex);
                blockUIHelper.HighlightTargetBlock(_spanObjects[currentQuestionIndex]);
                currentQuestionIndex++;
                yield return new WaitForSeconds(2f);
            }
            
            DOVirtual.DelayedCall(1f, SwitchNextState);
        }

        public void EnableCircle(int index)
        {
            ActivateCircle(index);
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
            ResetPreviousCircles();
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
            horizontalParent.gameObject.SetActive(true);
            unitParent.gameObject.SetActive(true);
        }

        public override void DisableUIElements()
        {
            horizontalParent.gameObject.SetActive(false);
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
