#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Extensions;

namespace Camera_Controls
{
    [RequireComponent(typeof(InputHandler), typeof(Camera))]
    public class HighlightOnMouseOver : MonoBehaviour
    {
        [SerializeField] private Material highlightMaterial;

        private InputHandler _inputHandler;

        private GameObject? _highlightedTarget;
        private Camera _camera;

        private List<MeshRenderer>? _highlightedMeshRenderers;

        private List<MeshRenderer>? HighlightedMeshRenderers
        {
            set
            {
                if (value.IsNotNullOrEmpty())
                {
                    foreach (var meshRenderer in value)
                    {
                        AddHighlightMaterial(meshRenderer);
                    }
                }
                if (_highlightedMeshRenderers.IsNotNullOrEmpty())
                {
                    foreach (var highlightedMeshRenderer in _highlightedMeshRenderers)
                    {
                        RemoveHighlightMaterial(highlightedMeshRenderer);
                    }
                }

                _highlightedMeshRenderers = value;
            }
        }

        private Ray ray;


        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _inputHandler = GetComponent<InputHandler>();
        }

        private void FixedUpdate()
        {
            if (_inputHandler.isRightClicked)
            {
                ResetTarget();
                return;
            }

            ray = _camera.ScreenPointToRay(InputHandler.MousePosition);
            if (!Physics.Raycast(ray, out var hit, _camera.farClipPlane))
            {
                ResetTarget();
                return;
            }

            var target = hit.collider.gameObject;
            if (target != _highlightedTarget)
            {
                var meshRenderers = new List<MeshRenderer>(6);
                target.GetComponentsInChildren(meshRenderers);
                if (meshRenderers.IsEmpty())
                {
                    ResetTarget();
                    return;
                }

                SetTarget(target, meshRenderers);
                print($"Highlight {hit.collider.gameObject.name}");
            }
        }

        private void ResetTarget()
        {
            SetTarget(null, null);
        }

        private void SetTarget(GameObject? target, List<MeshRenderer>? meshRenderers)
        {
            _highlightedTarget = target;
            HighlightedMeshRenderers = meshRenderers;
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


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(ray.origin, ray.origin + ray.direction * 10000);
        }
    }
}
