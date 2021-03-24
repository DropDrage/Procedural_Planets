using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    public float radius;

    public float orbitRadius;

    // public float surfaceGravity;
    public Vector3 initialVelocity;
    public string bodyName = "Unnamed";

    private Rigidbody _rb;
    private Planet _planet;


    public Vector3 Velocity { get; private set; }
    public float Mass => _rb.mass;
    public Rigidbody Rigidbody => _rb;
    public Vector3 Position => _rb.position;

    [SerializeField] private Quaternion velocityRotation;


    // mass = surfaceGravity * radius * radius / 9.8f;

    private void Awake()
    {
        _planet = GetComponent<Planet>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        radius = _planet.shapeSettings.planetRadius;
        gameObject.name = bodyName;
        _rb.AddForce(initialVelocity, ForceMode.VelocityChange);
        // Velocity = initialVelocity;
    }

    public void AddAttraction(IEnumerable<GravityBody> allBodies)
    {
        foreach (var otherBody in allBodies.Where(body => body != this))
        {
            var distance = otherBody.Position - _rb.position;
            var sqrDst = distance.sqrMagnitude;
            var forceDir = distance.normalized;

            var acceleration = forceDir * Universe.GravitationConstant * (Mass * otherBody.Mass) / sqrDst;
            // Velocity += acceleration;
            _rb.AddForce(acceleration, ForceMode.Force);
        }
    }

    public void UpdatePosition()
    {
        _rb.AddForce(Velocity, ForceMode.Force);
    }


    /*private void OnDrawGizmos()
    {
        var position = transform.position;
        velocityRotation.Normalize();
        Gizmos.DrawLine(position, position + (initialVelocity + velocityRotation.eulerAngles));
    }*/
}