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
using Utilities.Helpers;

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

        private bool _coroutineRunning;
        protected override void Start()
        {
            SpawnPool();
            base.Start();
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
            EnableUIElements();
            //if(_nBackController.GetCorrectStatus())
              //  HighlightCorrectUnit();
            StatisticsHelper.IncrementDisplayedQuestionCount();
            if(!_coroutineRunning)
                ShowQuestion();
        }
        
        public override void ShowQuestion()
        {
            Debug.Log("ShowQuestion called");
            if (_coroutineRunning)
            {
                Debug.LogWarning("Coroutine is already running. Skipping ShowQuestion call.");
                return; // Prevent starting a new coroutine if one is already running
            }

            _coroutineRunning = true;
            if (IsOneOfNBackModes)
            {
                _currentStrategy.InjectQuestionState(this);
                _currentStrategy.ShowQuestion();
                if (displayingQuestions != null)
                {
                    StopCoroutine(displayingQuestions);
                }
                displayingQuestions = StartCoroutine(IterateBlocks());
            }
            else
            {
                if (displayingQuestions != null)
                {
                    StopCoroutine(displayingQuestions);
                }
                displayingQuestions = StartCoroutine(IterateQuestions());
            }
        }
        
        private bool _initialDisplay = true;
        
        private IEnumerator IterateQuestions()
        {
            Debug.Log("Coroutine started");
            var start = 1;
            if (_initialDisplay)
            {
                start = 0;
                _initialDisplay = false;
            }
            for (int i = start; i < _spanObjects.Count; i++)
            {
                Debug.Log($"Processing question index: {i}");
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
                Debug.Log("Waiting for 1 second before disabling images");
                yield return new WaitForSeconds(1f);
                DisableActiveImages();
                Debug.Log("Waiting for another 1 second after disabling images");
                yield return new WaitForSeconds(1f);
            }

            Debug.Log("Scheduling SwitchNextState call");
            DOVirtual.DelayedCall(1f, () =>
            {
                SwitchNextState();
            });
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
            Image pooledImage = GetAvailableImage();
            pooledImage.gameObject.SetActive(true);
            pooledImage.sprite = (Sprite)question.GetQuestionItem();
            Debug.Log(pooledImage.gameObject.activeSelf);
            _activeQuestionImages.Add(pooledImage);
            ActivateCircle(index);
            _currentQuestions.Add(question);
        }

        private void PlayAudio(Question question)
        {
            AudioManager.Instance.PlayAudioClip((AudioClip)question.GetQuestionItemByType(CommonFields.ButtonType.Sound));
        }
        
        private IEnumerator IterateBlocks()
        {
            var start = _initialDisplay ? 0 : 1;
            _initialDisplay = false;
            for (int i = start; i < _spanObjects.Count; i++)
            {
                ActivateCircle(i);
                blockUIHelper.HighlightTargetBlock(_spanObjects[i]);
                if (IsDualNBack)
                {
                    PlayAudio(_spanObjects[i]);
                }
                
                yield return new WaitForSeconds(2f);
            }
            
            DOVirtual.DelayedCall(1f, () =>
            {
                SwitchNextState();
                
            });
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
                _coroutineRunning = false;
                StopCoroutine(displayingQuestions);
                displayingQuestions = null;
            }
            base.Exit();
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
            if (IsOneOfNBackModes)
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
            if(!IsDualNBack)
                blockUIHelper.gameObject.SetActive(false);
            blockUIHelper.ResetCorsiBlocks();
        }

        private void SpawnPool()
        {
            for (int i = 0; i < SpawnAmount; i++)
            {
                var tempImage = Instantiate(questionImage, horizontalParent.transform);
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
            
            _activeQuestionImages.Clear();
        }

        private bool IsOneOfNBackModes => _currentStrategy is NBackMode || _currentStrategy is DualNBackMode;
        private bool IsDualNBack => _currentStrategy is DualNBackMode;
    }
}
