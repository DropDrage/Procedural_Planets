using Planet;
using UnityEngine;

namespace Planet_System
{
    public abstract class BasePlanetSystem : MonoBehaviour
    {
        [SerializeField] protected GravityBody[] bodies;

        public virtual GravityBody[] Bodies
        {
            set => bodies = value;
        }
    }
}
