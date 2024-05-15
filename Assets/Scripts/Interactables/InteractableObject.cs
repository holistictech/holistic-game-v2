using System;
using System.Collections.Generic;
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
        private MeshFilter _objectMeshFilter;
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

        public virtual void BuildSelf(CartesianPoint desiredPoint)
        {
            SetObjectMesh();
            SetPosition(desiredPoint);
            _data = new InteractableData(_interactableConfig, desiredPoint);
            _gridController.AppendSpawnedInteractables(_data);
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
            _objectMeshFilter.mesh = _interactableConfig.ObjectMesh;
        }

        public void SetPosition(CartesianPoint point)
        {
            transform.position = new Vector3(point.GetXCoordinate(), 0, point.GetYCoordinate());
        }
    }

    [Serializable]
    public class InteractableData
    {
        internal SerializableMeshData MeshData; // Serialized mesh data
        internal CartesianPoint Point;
        public InteractableConfig Config;
    
        public InteractableData(InteractableConfig config, CartesianPoint point)
        {
            Config = config;
            Point = point;
            // Extract mesh data from the config and store it
            MeshData = SerializeMesh(config.ObjectMesh);
        }

        // Helper method to serialize mesh data
        private SerializableMeshData SerializeMesh(Mesh mesh)
        {
            SerializableMeshData meshData = new SerializableMeshData();
            meshData.vertices = mesh.vertices;
            meshData.triangles = mesh.triangles;
            meshData.uv = mesh.uv;
            // Add any other necessary data for mesh reconstruction
            return meshData;
        }
        
        public Mesh CreateMeshFromData()
        {
            if (MeshData == null)
            {
                Debug.LogError("SerializableMeshData is null. Cannot create mesh.");
                return null;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = MeshData.vertices;
            mesh.triangles = MeshData.triangles;
            mesh.uv = MeshData.uv;
            // Add any other necessary data for mesh reconstruction

            mesh.RecalculateNormals(); // Recalculate normals to ensure proper shading

            return mesh;
        }
    }

    [Serializable]
    public class SerializableMeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uv;
        // Add any other necessary data for mesh reconstruction
    }
}
