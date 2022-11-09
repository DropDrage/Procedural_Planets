using UnityEngine;

namespace Planet
{
    [RequireComponent(typeof(Planet), typeof(Rigidbody))]
    public class GravityBody : MonoBehaviour
    {
        /// <summary>
        /// Only for convenient access to radius.
        /// </summary>
        public float radius;

        // public float surfaceGravity;
        public Vector3 initialVelocity;
        public string bodyName = "Unnamed";

        private Rigidbody _rb;

        private Transform _transform;


        public float Mass => _rb.mass;
        public Vector3 Position => _transform.position;


        // mass = surfaceGravity * radius * radius / 9.8f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _transform = transform;
        }

        private void OnEnable()
        {
            Initialize();
        }

        public void Initialize()
        {
            radius = GetComponent<Planet>().shapeSettings.planetRadius;
            gameObject.name = bodyName;
            _rb.velocity = initialVelocity;
        }


        public void AddAttraction(Vector3 attraction)
        {
            _rb.AddForce(attraction);
        }

        public void AddAttraction(ref Vector3 attraction)
        {
            _rb.AddForce(attraction);
        }


        /*private void OnDrawGizmos()
    {
        var position = transform.position;
        velocityRotation.Normalize();
        Gizmos.DrawLine(position, position + (initialVelocity + velocityRotation.eulerAngles));
    }*/
    }
}
