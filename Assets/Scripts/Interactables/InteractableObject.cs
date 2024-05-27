using System;
using System.Collections.Generic;
using DG.Tweening;
using GridSystem;
using Interfaces;
using Scriptables;
using UnityEngine;
using Utilities;
using Grid = UnityEngine.Grid;
using static Utilities.CommonFields;

namespace Interactables
{
    [Serializable]
    public class InteractableObject : MonoBehaviour, ISpawnable
    {
        private ParticleSystem _buildingDust;
        private ParticleSystem _dust;
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

        public void InjectFields(GridController controller, InteractableConfig config, ParticleSystem effect)
        {
            _gridController = controller;
            _interactableConfig = config;
            _buildingDust = effect;
            _dust = effect;
        }

        public virtual void BuildSelf(CartesianPoint desiredPoint, bool isFirstTime)
        {
            SetObjectMesh();
            SetPosition(desiredPoint);
            _data = new InteractableData(_interactableConfig, desiredPoint);
            if (isFirstTime)
            {
                AnimateBuilding();
                _gridController.AppendSpawnedInteractables(_data);
            }
            else
            {
                _meshRenderer.enabled = true;
            }
        }

        public void BlockCoordinates(List<CartesianPoint> desiredPoints)
        {
            _gridController.BlockCoordinates(desiredPoints);
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

        public void AnimateBuilding()
        {
            SpawnBuildingEffect();
            _dust.Play();
            DOVirtual.DelayedCall(_buildingDust.main.duration - 2f, () =>
            {
                transform.localScale = new Vector3(1, 0, 1);
                _meshRenderer.enabled = true;
                Sequence mySequence = DOTween.Sequence();
                mySequence.Append(transform.DOShakeScale(0.75f, 0.3f));
                mySequence.Join(transform.DOScaleY(1f, .75f).SetEase(Ease.OutQuart));

            });
        }

        private void SpawnBuildingEffect()
        {
            _dust = Instantiate(_buildingDust, transform);
            var localPosition = _dust.transform.localPosition;
            localPosition = new Vector3(localPosition.x,
                localPosition.y + 1f, localPosition.z);
            _dust.transform.localPosition = localPosition;
        }

        public void SetPosition(CartesianPoint point)
        {
            transform.position = new Vector3(point.GetXCoordinate(), 0, point.GetYCoordinate());
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
