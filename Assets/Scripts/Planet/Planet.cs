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

        [Tooltip("Used only in Autogens")]
        public FaceRenderMask faceRenderMask;

        public SphereCollider sphereCollider;
        public TrailRenderer trailRenderer;


        private void Awake()
        {
            sphereCollider = GetComponent<SphereCollider>();
            trailRenderer = GetComponent<TrailRenderer>();
        }
    }
}
