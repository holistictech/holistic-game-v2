using System;
using System.Collections.Generic;
using DG.Tweening;
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
        
        private void Awake()
        {
            InstantiatePool();
        }

        public override void AssignQuestions(List<Question> roundQuestions)
        {
            selectedAnswers.Clear();
            selectedBlocks.Clear();
            foreach (var question in roundQuestions)
            {
                var block = GetAvailableBlock();
                block.ConfigureSelf(question, this);
            }
        }

        public void SetConstraintsCount(int helperIndex)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = helperIndex;
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
            ResetCorsiBlocks();
        }
        
        public void DisableCorsiBlocks()
        {
            foreach (var block in spawnedBlocks)
            {
                block.DisableSelf();
            }
        }

        private void InstantiatePool()
        {
            for (int i = 0; i < 9; i++)
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
