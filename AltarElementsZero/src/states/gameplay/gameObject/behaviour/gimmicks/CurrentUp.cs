using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{
    class CurrentUp : IBehaviour
    {
        public static readonly CurrentUp Instance = new ();

        public void Init(GameObject gameObject)
        {
            gameObject.Type = GameObject.Types.FLUID;

            gameObject.isVisible = true;
            gameObject.SpriteOffset = new PxSize();
            gameObject.spritesheetIndex = 0x2d;


			gameObject.currentBoundingBox.Size = new TileSize(1, 2).ToPx().ToSubpx();

            gameObject.FluidVelocity = new SubpxVelocity(0, 64 * 10);// -64*40);
            gameObject.FluidCoefficient = 0;

        }

        public void Update(GameObject gameObject)
        {
        }

    }
}
