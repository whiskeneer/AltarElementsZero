using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

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
			gameObject.currentBoundingBox.Size = new SubpxSize();

			//gameObject.spritesheetIndex = 0x28;
			//gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
			//gameObject.SpriteOffset = new PxSize(16, 16);

			gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x28);
			gameObject.atlasReference.Size = new PxSize(32, 32);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(16, 16);

			gameObject.Timer = 17;

		}

		public void Update(GameObject gameObject)
		{
			gameObject.Timer--;

			if(gameObject.Timer > 12)
			{
				//gameObject.spritesheetIndex = 0x28;
				gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x28);
			}
			else if(gameObject.Timer > 8)
			{
				//gameObject.spritesheetIndex = 0x29;
				gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x29);
			}
			else if(gameObject.Timer > 4)
			{
				//gameObject.spritesheetIndex = 0x2A;
				gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x2A);
			}
			else
			{
				//gameObject.spritesheetIndex = 0x2B;
				gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x2B);
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
