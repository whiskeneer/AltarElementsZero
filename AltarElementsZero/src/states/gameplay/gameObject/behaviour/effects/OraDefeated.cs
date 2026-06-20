using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.effects
{
	class OraDefeated : IBehaviour
	{
		public static readonly OraDefeated Instance = new();

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.REGION;
			gameObject.isPersistentAcrossChunks = true;

			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;

			gameObject.currentBoundingBox.Size = new SubpxSize();

			//gameObject.SpriteOffset = new PxSize(16, 16);
			//gameObject.spritesheetIndex = 0x2c;
			//gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
			gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x2c);
			gameObject.atlasReference.Size = new PxSize(32, 32);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(16, 16);


			gameObject.Timer = 17;
			gameObject.Timer2 = 40;
		}

		public void Update(GameObject gameObject)
		{
			if(gameObject.Timer == 0){
				gameObject.currentVelocity = new SubpxVelocity(0,-64);
				gameObject.Timer2--;

				if((gameObject.Timer2 & 8) == 8){
					//gameObject.spritesheetIndex = 0x3e;
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x3e);

				}
				else
				{
					//gameObject.spritesheetIndex = 0x3f;
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x3f);

				}

				if (gameObject.Timer2 == 0){
					GameObject.signalFlags!.EmitGameplayMessage(GameplayMessages.RestartFromCheckpoint);
				}

			}else{
				gameObject.Timer--;
				if (gameObject.Timer > 12)
				{
					//gameObject.spritesheetIndex = 0x2C;
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x2c);
				}
				else if (gameObject.Timer > 8)
				{
					//gameObject.spritesheetIndex = 0x2D;
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x2d);
				}
				else if (gameObject.Timer > 4)
				{
					//gameObject.spritesheetIndex = 0x2E;
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x2e);
				}
				else
				{
					//gameObject.spritesheetIndex = 0x2F;
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x2f);
				}
			}	
		}

		public void Interact(GameObject own, GameObject other)
		{
		}
	}
}
