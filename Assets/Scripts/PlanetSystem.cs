using Planet;
using UnityEngine;
using Utils;

public class PlanetSystem : MonoBehaviour
{
    [SerializeField] public GravityBody[] bodies;


    private void FixedUpdate()
    {
        foreach (var body in bodies)
        {
            var attraction = Vector3.zero;
            foreach (var otherBody in bodies)
            {
                if (otherBody != body)
                {
                    var distance = otherBody.Position - body.Position;
                    var sqrDst = distance.sqrMagnitude;
                    var forceDir = distance.normalized;

                    attraction += forceDir * (Universe.GravitationConstant * (body.Mass * otherBody.Mass)) / sqrDst;
                }
            }
            body.AddAttraction(attraction);
        }
    }
}
