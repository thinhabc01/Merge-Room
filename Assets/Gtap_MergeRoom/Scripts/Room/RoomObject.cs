using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private EItem _eItem;
    [SerializeField] private float _ratioScale = 1.05f;
    [SerializeField] private GameObject _model;
    [SerializeField] private GameObject _ghostModel;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private MeshFilter[] _meshFilters;
    [SerializeField] private Material _ghostMaterial;

    private readonly Color _colorGhost = new Color(0.3137f, 0.3137f, 1f, 1f);

    private MaterialPropertyBlock _materialProperty;
    private MeshRenderer _ghostRenderer;
    private MeshRenderer _meshRenderer;
    private Vector3 _initialScale;
    private Vector3 _activeScale;
    private bool _isCompleted;
    private bool _highlight;

    public EItem EItem => _eItem;

    public bool IsCompleted
    {
        get => _isCompleted;
        private set
        {
            _isCompleted = value;

            _model.SetActive(value);
            _ghostModel.SetActive(!value);
        }
    }

    public bool Highlight
    {
        get => _highlight;
        set
        {
            _highlight = value;

            for (int i = 0; i < _ghostRenderer.materials.Length; i++)
            {
                _ghostRenderer.GetPropertyBlock(_materialProperty, i);
                _materialProperty.SetColor("_BaseColor", _highlight ? Color.green : _colorGhost);
                _ghostRenderer.SetPropertyBlock(_materialProperty, i);
            }
        }
    }

    public int ID => _id;

    public event Action<RoomObject> OnComletedRoom;

    private void Awake()
    {
        _materialProperty = new MaterialPropertyBlock();
        _initialScale = transform.localScale;
        _activeScale = _initialScale * _ratioScale;

        if (_meshFilters.Length > 0)
            CombineMesh();

        _meshRenderer = _model.GetComponent<MeshRenderer>();
        _ghostRenderer = _ghostModel.GetComponent<MeshRenderer>();
        ParticleSystem.ShapeModule shape = _particleSystem.shape;

        shape.meshRenderer = _meshRenderer;

        IsCompleted = false;
    }

    private void CombineMesh()
    {
        CombineInstance[] combine = new CombineInstance[_meshFilters.Length];

        int i = 0;
        while (i < _meshFilters.Length)
        {
            combine[i].mesh = _meshFilters[i].sharedMesh;
            combine[i].transform = _meshFilters[i].transform.localToWorldMatrix;
            _meshFilters[i].gameObject.SetActive(false);

            i++;
        }

        _model = new GameObject("Model", typeof(MeshFilter), typeof(MeshRenderer));
        _ghostModel = new GameObject("GhostModel", typeof(MeshFilter), typeof(MeshRenderer));

        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();

        foreach (MeshFilter meshFilter in _meshFilters)
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            if (!meshRenderer ||
                !meshFilter.sharedMesh ||
                meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
            {
                continue;
            }

            for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
            {
                int materialArrayIndex = Contains(materials, meshRenderer.sharedMaterials[s].name);
                if (materialArrayIndex == -1)
                {
                    materials.Add(meshRenderer.sharedMaterials[s]);
                    materialArrayIndex = materials.Count - 1;
                }

                combineInstanceArrays.Add(new ArrayList());

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                combineInstance.subMeshIndex = s;
                combineInstance.mesh = meshFilter.sharedMesh;
                (combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
            }
        }

        Mesh[] meshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        for (int m = 0; m < materials.Count; m++)
        {
            CombineInstance[] combineInstanceArray =
                (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[m] = new Mesh();
            meshes[m].CombineMeshes(combineInstanceArray, true, true);

            combineInstances[m] = new CombineInstance();
            combineInstances[m].mesh = meshes[m];
            combineInstances[m].subMeshIndex = 0;
        }

        _model.GetComponent<MeshFilter>().mesh = new Mesh();
        _model.GetComponent<MeshFilter>().mesh.CombineMeshes(combineInstances, false, false);
        _ghostModel.GetComponent<MeshFilter>().mesh = new Mesh();
        _ghostModel.GetComponent<MeshFilter>().mesh.CombineMeshes(combineInstances, false, false);

        foreach (Mesh oldMesh in meshes)
        {
            oldMesh.Clear();
            DestroyImmediate(oldMesh);
        }

        Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];

        List<Material> ghostMaterials = new List<Material>();

        for (int j = 0; j < materials.Count; j++)
        {
            ghostMaterials.Add(_ghostMaterial);
        }

        Material[] materialsGhostArray = ghostMaterials.ToArray();
        _model.GetComponent<MeshRenderer>().materials = materialsArray;
        _ghostModel.GetComponent<MeshRenderer>().materials = materialsGhostArray;

        foreach (MeshFilter meshFilter in _meshFilters)
        {
            DestroyImmediate(meshFilter.gameObject);
        }
        
        _model.transform.SetParent(transform, true);
        _ghostModel.transform.SetParent(transform, true);
    }


    private int Contains(ArrayList searchList, string searchName)
    {
        for (int i = 0; i < searchList.Count; i++)
        {
            if (((Material)searchList[i]).name == searchName)
            {
                return i;
            }
        }

        return -1;
    }

    private void OnValidate()
    {
        if (ID == 0)
            _id = GetInstanceID();
    }

    public void Completed(bool feedback)
    {
        gameObject.layer = 0;

        Highlight = false;
        IsCompleted = true;

        OnComletedRoom?.Invoke(this);

        if (feedback)
        {
            HapticManager.Instance.PlayLightHaptic();
            _particleSystem.Play();
        }
    }

    public void Reset()
    {
        gameObject.layer = 3;

        Highlight = false;
        IsCompleted = false;
    }

    private void Update()
    {
        transform.localScale =
            Vector3.Lerp(transform.localScale,
                Highlight ? _activeScale : _initialScale,
                Time.smoothDeltaTime * 4f);
    }
}