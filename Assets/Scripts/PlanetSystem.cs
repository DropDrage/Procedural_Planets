using Planet;
using UnityEngine;
using Utils;

public class PlanetSystem : MonoBehaviour
{
    [SerializeField] public GravityBody[] bodies;


    private void FixedUpdate()
    {
        for (var i = 0; i < bodies.Length; i++)
        {
            var attractedBody = bodies[i];

            var attraction = Vector3.zero;
            for (var otherBodyI = 0; otherBodyI < bodies.Length; otherBodyI++)
            {
                var attractorBody = bodies[otherBodyI];
                if (attractorBody != attractedBody) //ToDo swap with first to eliminate this check
                {
                    var distance = attractorBody.Position - attractedBody.Position;
                    var sqrDst = distance.sqrMagnitude;
                    var forceDir = distance.normalized;

                    // ToDo test without struct recreation
                    attraction += forceDir *
                        (Universe.GravitationConstant * (attractedBody.Mass * attractorBody.Mass)) / sqrDst;
                }
            }
            attractedBody.AddAttraction(attraction);
        }
    }
}
