using UnityEngine;

public class PlanetSystem : MonoBehaviour
{
    [SerializeField] public GravityBody[] bodies;


    private void FixedUpdate()
    {
        foreach (var body in bodies)
        {
            body.AddAttraction(bodies);
        }
    }
}