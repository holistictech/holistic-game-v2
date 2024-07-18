using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class MeshContainer : MonoBehaviour
    {
        [SerializeField] private List<Mesh> meshList;

        private static Dictionary<int, Mesh> MeshDictionary = new Dictionary<int, Mesh>();

        private static MeshContainer _instance;
        
        public static MeshContainer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MeshContainer>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject("MeshContainer");
                        _instance = singletonObject.AddComponent<MeshContainer>();
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            BuildDictionary();
        }

        private void BuildDictionary()
        {
            for (int i = 0; i < meshList.Count; i++)
            {
                MeshDictionary.TryAdd(i, meshList[i]);
            }
        }

        public Mesh GetMeshById(int id)
        {
            if (MeshDictionary.Count == 0 )
            {
                BuildDictionary();
            }
            
            if (MeshDictionary.ContainsKey(id))
            {
                MeshDictionary.TryGetValue(id, out Mesh mesh);
                return mesh;
            }
            else
            {
                throw new Exception("ID could not be found :" + id);
            }
        }
    }
}
