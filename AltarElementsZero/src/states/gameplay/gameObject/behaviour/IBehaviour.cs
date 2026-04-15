using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour
{
    interface IBehaviour
    {
        void Init(GameObject gameObject);
        void Update(GameObject gameObject);

        // Note: Interactions MUST be idempotent!!! 
        void Interact(GameObject own, GameObject other);

    }
}
