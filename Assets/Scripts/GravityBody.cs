using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    /**
     * Only for convenient access to radius.
     */
    public float radius;

    public float orbitRadius;

    // public float surfaceGravity;
    public Vector3 initialVelocity;
    public string bodyName = "Unnamed";

    private Rigidbody _rb;
    private Planet _planet;

    private Transform _transform;


    public float Mass => _rb.mass;
    public Vector3 Position => _transform.position;


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
        _transform = transform;
        // Velocity = initialVelocity;
    }

    public void AddAttraction(IEnumerable<GravityBody> allBodies)
    {
        foreach (var otherBody in allBodies.Where(body => body != this))
        {
            var distance = otherBody.Position - Position;
            var sqrDst = distance.sqrMagnitude;
            var forceDir = distance.normalized;

            var acceleration = forceDir * Universe.GravitationConstant * (Mass * otherBody.Mass) / sqrDst;
            // Velocity += acceleration;
            _rb.AddForce(acceleration, ForceMode.Force);
        }
    }


    /*private void OnDrawGizmos()
    {
        var position = transform.position;
        velocityRotation.Normalize();
        Gizmos.DrawLine(position, position + (initialVelocity + velocityRotation.eulerAngles));
    }*/
}