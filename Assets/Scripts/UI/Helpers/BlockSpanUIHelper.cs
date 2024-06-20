using System;
using System.Collections.Generic;
using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using UI.CorsiBlockTypes;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Helpers
{
    public class BlockSpanUIHelper : CorsiBlockUIHelper
    {
        [SerializeField] private CorsiBlock blockImage;
        [SerializeField] private GridLayoutGroup gridLayout;

        private List<CorsiBlock> _blockPool = new List<CorsiBlock>();
        private IBlockSpanStrategy _currentStrategy;
        
        private void Awake()
        {
            InstantiatePool();
        }
        
        public void SetStrategy(IBlockSpanStrategy strategy)
        {
            _currentStrategy = strategy;
        }

        public override void AssignQuestions(List<Question> roundQuestions)
        {
            selectedAnswers.Clear();
            selectedBlocks.Clear();
            foreach (var question in roundQuestions)
            {
                var block = GetAvailableBlock();
                block.ConfigureSelf(question, this);
                block.GetComponent<AdaptableBlock>().SetStrategy(_currentStrategy);
            }
        }

        public void SetConstraintsCount(int helperIndex)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = helperIndex;
            CalculateDynamicCellSize();
        }
        
        private void CalculateDynamicCellSize()
        {
            var rect = gridLayout.GetComponent<RectTransform>().rect;
            float width = rect.width;
            float height = rect.height;
    
            float availableWidth = width - (gridLayout.padding.left + gridLayout.padding.right) - ((gridLayout.constraintCount - 1) * gridLayout.spacing.x);
            float availableHeight = height - (gridLayout.padding.top + gridLayout.padding.bottom) - ((gridLayout.constraintCount - 1) * gridLayout.spacing.y);
    
            float cellWidth = availableWidth / gridLayout.constraintCount;
            float cellHeight = availableHeight / gridLayout.constraintCount;
            float cellSize = Mathf.Min(cellWidth, cellHeight);
            gridLayout.cellSize = new Vector2(cellSize, cellSize);
        }

        public override void HighlightTargetBlock(Question target)
        {
            var block = GetBlockByQuestion(target);
            block.AnimateSelf();
        }

        public void RotateGrid(int amount, Action onComplete)
        {
            Quaternion originalRotation = gridLayout.transform.rotation;

            gridLayout.transform.DORotate(new Vector3(originalRotation.eulerAngles.x, originalRotation.eulerAngles.y, originalRotation.eulerAngles.z + amount), .5f)
                .SetEase(Ease.Linear).OnComplete(
                    () =>
                    {
                        onComplete?.Invoke();
                    });
        }

        public void EnableGrid()
        {
            gridLayout.gameObject.SetActive(true);
        }
        
        public void DisableCorsiBlocks()
        {
            foreach (var block in spawnedBlocks)
            {
                block.DisableSelf();
            }
            
            //ResetCorsiBlocks();
        }

        private void InstantiatePool()
        {
            for (int i = 0; i < 25; i++)
            {
                var tempBlock = Instantiate(blockImage, gridLayout.transform);
                _blockPool.Add(tempBlock);
            }

            spawnedBlocks = _blockPool;
        }

        private CorsiBlock GetAvailableBlock()
        {
            foreach (var block in _blockPool)
            {
                if (!block.isActiveAndEnabled)
                {
                    return block;
                }
            }

            throw new Exception("Could not find available block");
        }
    }
}
