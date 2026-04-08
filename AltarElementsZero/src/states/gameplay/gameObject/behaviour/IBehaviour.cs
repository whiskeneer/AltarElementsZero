using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour
{
    interface IBehaviour
    {
        bool Exists(GameObject gameObject);
        bool IsSolid(GameObject gameObject);
        bool HurtsPlayer(GameObject gameObject);
        bool IsFixed(GameObject gameObject);
        bool IsAffectedByGravity(GameObject gameObject);

        //

        bool IsVisible(GameObject gameObject);
        uint GetSpritesheetIndex(GameObject gameObject);
        SpriteEffects GetSpriteEffects(GameObject gameObject);

        void Update(GameObject gameObject);
        void Init(GameObject gameObject);


    }
}
