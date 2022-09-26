using Planet;
using UnityEngine;
using Utils;
using Utils.Extensions;

public class PlanetSystem : MonoBehaviour
{
    [SerializeField] public GravityBody[] bodies;


    private void FixedUpdate()
    {
        foreach (var attractedBody in bodies)
        {
            var attraction = Vector3.zero;
            foreach (var attractorBody in bodies)
            {
                if (attractorBody != attractedBody) //ToDo swap with first to eliminate this check
                {
                    var distance = attractorBody.Position - attractedBody.Position;
                    var sqrDistance = distance.sqrMagnitude;
                    var attractionMagnitude = Universe.GravitationConstant
                        * (attractedBody.Mass * attractorBody.Mass)
                        / sqrDistance;
                    var attractionForce = distance.normalized * attractionMagnitude;

                    attraction.Add(ref attractionForce);
                }
            }
            attractedBody.AddAttraction(ref attraction);
        }
    }
}
