using System;
using System.Collections.Generic;
using DG.Tweening;
using GridSystem;
using Interfaces;
using Scriptables;
using UnityEngine;
using Utilities;
using Utilities.Helpers;
using Grid = UnityEngine.Grid;
using static Utilities.Helpers.CommonFields;

namespace Interactables
{
    [Serializable]
    public class InteractableObject : MonoBehaviour, ISpawnable
    {
        private MeshFilter _objectMeshFilter;
        private MeshRenderer _meshRenderer;
        private GridController _gridController;
        private InteractableConfig _interactableConfig;
        private InteractableData _data;
        
        protected GridController GridController => _gridController;
        public InteractableConfig InteractableConfig => _interactableConfig;

        public InteractableObject(GridController controller, InteractableConfig config)
        {
            _gridController = controller;
            _interactableConfig = config;
        }

        public void InjectFields(GridController controller, InteractableConfig config)
        {
            _gridController = controller;
            _interactableConfig = config;
        }

        public virtual void BuildSelf(CartesianPoint desiredPoint, bool isFirstTime, float delay)
        {
            SetObjectMesh();
            SetPosition(desiredPoint);
            _data = new InteractableData(_interactableConfig, desiredPoint);
            if (isFirstTime)
            {
                AnimateBuilding(delay);
                _gridController.AppendSpawnedInteractables(_data);
            }
            else
            {
                _meshRenderer.enabled = true;
                SetScale();
            }
            
            gameObject.isStatic = !isFirstTime;
        }

        public void BlockCoordinates(List<CartesianPoint> desiredPoints, InteractableType type)
        {
            _gridController.BlockCoordinates(desiredPoints, type);
        }

        public virtual List<CartesianPoint> CalculateCoordinatesForBlocking(CartesianPoint desiredPoint)
        {
            List<CartesianPoint> points = new List<CartesianPoint>();

            for (int z = desiredPoint.GetYCoordinate() - _interactableConfig.Height / 2; z < desiredPoint.GetYCoordinate() + _interactableConfig.Height / 2; z++)
            {
                for (int x = desiredPoint.GetXCoordinate() - _interactableConfig.Width / 2 ; x < desiredPoint.GetXCoordinate() + _interactableConfig.Width / 2; x++)
                {
                    CartesianPoint temp = new CartesianPoint(x, z);
                    points.Add(temp);
                }
            }
            return points;
        }

        public void SetObjectMesh()
        {
            _objectMeshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _objectMeshFilter.mesh = MeshContainer.Instance.GetMeshById(_interactableConfig.MeshId);
        }

        public void AnimateBuilding(float delay)
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                transform.localScale = new Vector3(_interactableConfig.ScaleAmount, 0, _interactableConfig.ScaleAmount);
                _meshRenderer.enabled = true;
                Sequence mySequence = DOTween.Sequence();
                mySequence.Append(transform.DOShakeScale(0.75f, 0.3f));
                mySequence.Join(transform.DOScaleY(_interactableConfig.ScaleAmount, .75f).SetEase(Ease.OutQuart).OnComplete(
                    SetScale));
            });
        }

        public InteractableType GetInteractableType()
        {
            return _interactableConfig.InteractableType;
        }

        public void SetPosition(CartesianPoint point)
        {
            transform.position = new Vector3(point.GetXCoordinate(), 0, point.GetYCoordinate());
        }

        public void SetScale()
        {
            var amount = _interactableConfig.ScaleAmount;
            transform.localScale = new Vector3(amount, amount, amount);
        }
    }

    [Serializable]
    public class InteractableData
    {
        public CartesianPoint Point;
        public InteractableConfig Config;
    
        public InteractableData(InteractableConfig config, CartesianPoint point)
        {
            Config = config;
            Point = point;
        }
        
    }
}
