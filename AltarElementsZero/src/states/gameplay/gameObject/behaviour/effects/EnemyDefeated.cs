using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.effects
{
	class EnemyDefeated : IBehaviour
	{
		public static readonly EnemyDefeated Instance = new ();

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.REGION;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
			gameObject.spritesheetIndex = 0x28;
			gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

			gameObject.currentBoundingBox.Size = new SubpxSize();
			gameObject.SpriteOffset = new PxSize(16, 16);

			gameObject.Timer = 17;

		}

		public void Update(GameObject gameObject)
		{
			gameObject.Timer--;

			if(gameObject.Timer > 12)
			{
				gameObject.spritesheetIndex = 0x28;
			}
			else if(gameObject.Timer > 8)
			{
				gameObject.spritesheetIndex = 0x29;
			}
			else if(gameObject.Timer > 4)
			{
				gameObject.spritesheetIndex = 0x2A;
			}
			else
			{
				gameObject.spritesheetIndex = 0x2B;
			}

			if (gameObject.Timer == 0)
			{
				gameObject.Delete();
			}
		}

		public void Interact(GameObject own, GameObject other)
		{
		}

	}
}
