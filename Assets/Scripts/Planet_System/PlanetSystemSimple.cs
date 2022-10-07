using UnityEngine;
using Utils;
using Utils.Extensions;

namespace Planet_System
{
    public class PlanetSystemSimple : BasePlanetSystem
    {
        private void FixedUpdate()
        {
            foreach (var attractedBody in bodies)
            {
                var attraction = Vector3.zero;
                foreach (var attractorBody in bodies)
                {
                    if (attractorBody != attractedBody)
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
}
