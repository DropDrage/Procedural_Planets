using Planet.Settings;
using UnityEngine;

namespace Planet
{
    public enum FaceRenderMask
    {
        All,
        Top,
        Bottom,
        Left,
        Right,
        Front,
        Back,
    }

    [RequireComponent(typeof(TrailRenderer), typeof(SphereCollider), typeof(GravityBody))]
    public class Planet : MonoBehaviour
    {
        public ShapeSettings shapeSettings;
        public ColorSettings colorSettings;

        public FaceRenderMask faceRenderMask;

        protected SphereCollider sphereCollider;
        protected TrailRenderer trailRenderer;


        private void Awake()
        {
            sphereCollider = GetComponent<SphereCollider>();
            trailRenderer = GetComponent<TrailRenderer>();
        }
    }
}
