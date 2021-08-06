using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(InputHandler))]
public class HighlightOnMouseOver : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;

    [CanBeNull] private GameObject _highlightedTarget;
    [CanBeNull] private List<MeshRenderer> _highlightedMeshRenderers;

    private List<MeshRenderer> HighlightedMeshRenderers
    {
        set
        {
            if (_highlightedMeshRenderers?.Count > 0)
            {
                foreach (var highlightedMeshRenderer in _highlightedMeshRenderers)
                {
                    RemoveHighlightMaterial(highlightedMeshRenderer);
                }
            }

            _highlightedMeshRenderers = value;
        }
    }

    private InputHandler _inputHandler;


    private void Awake()
    {
        _inputHandler = GetComponent<InputHandler>();
    }

    private void LateUpdate()
    {
        if (_inputHandler.isRightClicked)
        {
            ResetTarget();
            return;
        }

        var ray = Camera.main!.ScreenPointToRay(_inputHandler.mousePosition);
        if (!Physics.Raycast(ray, out var hit, Camera.main.farClipPlane))
        {
            ResetTarget();
            return;
        }

        var meshRenderers = new List<MeshRenderer>(6);
        var target = hit.collider.gameObject;
        target.GetComponentsInChildren(meshRenderers);
        if (meshRenderers.Count == 0)
        {
            ResetTarget();
            return;
        }


        if (target != _highlightedTarget)
        {
            foreach (var meshRenderer in meshRenderers)
            {
                AddHighlightMaterial(meshRenderer);
            }

            SetTarget(target, meshRenderers);
            print("Highlight " + hit.collider.gameObject.name);
        }
    }


    private void SetTarget(GameObject target, List<MeshRenderer> meshRenderers)
    {
        _highlightedTarget = target;
        HighlightedMeshRenderers = meshRenderers;
    }

    private void ResetTarget()
    {
        SetTarget(null, null);
    }


    private void AddHighlightMaterial(Renderer targetRenderer)
    {
        var materials = targetRenderer.materials;
        var materialsWithHighlight = new Material[materials.Length + 1];
        Array.Copy(materials, 0, materialsWithHighlight, 1, materials.Length);
        materialsWithHighlight[0] = highlightMaterial;
        targetRenderer.materials = materialsWithHighlight;
    }

    private void RemoveHighlightMaterial(Renderer targetRenderer)
    {
        var materials = targetRenderer.materials;
        var materialsWithoutHighlightIndex = 0;
        var materialsWithoutHighlight = new Material[materials.Length - 1];
        foreach (var material in materials)
        {
            if (material.shader != highlightMaterial.shader)
            {
                materialsWithoutHighlight[materialsWithoutHighlightIndex++] = material;
            }
        }

        targetRenderer.materials = materialsWithoutHighlight;
    }
}