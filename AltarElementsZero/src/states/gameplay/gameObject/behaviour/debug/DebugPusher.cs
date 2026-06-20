using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.debug
{
    class DebugPusher : IBehaviour
    {
        public static readonly DebugPusher Instance = new ();

        public void Init(GameObject gameObject)
        {
			gameObject.Type = GameObject.Types.UNSTOPPABLE;
			gameObject.isPersistentAcrossChunks = false;


			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
			//gameObject.spritesheetIndex = 0x2f;
			//gameObject.SpriteOffset = new(0, 0);
			//gameObject.spriteEffects = SpriteEffects.None;
			gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x2f);
			gameObject.atlasReference.Size = new PxSize(32, 32);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);


			gameObject.currentBoundingBox.Size = new PxSize(16,16).ToSubpx();
        }

        public void Update(GameObject gameObject)
        {
            InputHandler? inputHandler = GameObject.inputHandler;
            if (inputHandler != null)
            {
                if (inputHandler.IsDown(Input.Up))
                {
                    gameObject.currentVelocity.Y = -64;
                }
                else if (inputHandler.IsDown(Input.Down))
                {
                    gameObject.currentVelocity.Y = 64;
                }
                else
                {
                    gameObject.currentVelocity.Y = 0;
                }

				if (inputHandler.IsDown(Input.Left))
				{
					gameObject.currentVelocity.X = -64;
				}
				else if (inputHandler.IsDown(Input.Right))
				{
					gameObject.currentVelocity.X = 64;
				}
				else
				{
					gameObject.currentVelocity.X = 0;
				}
			}
        }
		public void Interact(GameObject own, GameObject other)
		{
			return;
		}
	}
}
