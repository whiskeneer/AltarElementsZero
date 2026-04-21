using AltarElementsZero.src.states.gameplay.vectors;

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
			gameObject.spritesheetIndex = 0x2c;
			gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

			gameObject.currentBoundingBox.Size = new SubpxSize();
			gameObject.SpriteOffset = new PxSize(16, 16);

			gameObject.Timer = 17;
			gameObject.Timer2 = 40;
		}

		public void Update(GameObject gameObject)
		{
			if(gameObject.Timer == 0){
				gameObject.currentVelocity = new SubpxVelocity(0,-64);
				gameObject.Timer2--;

				if((gameObject.Timer2 & 8) == 8){
					gameObject.spritesheetIndex = 0x3e;

				}
				else
				{
					gameObject.spritesheetIndex = 0x3f;

				}

				if (gameObject.Timer2 == 0){
					GameObject.signalFlags!.EmitGameplayMessage(GameplayMessages.RestartFromCheckpoint);
				}

			}else{
				gameObject.Timer--;
				if (gameObject.Timer > 12)
				{
					gameObject.spritesheetIndex = 0x2C;
				}
				else if (gameObject.Timer > 8)
				{
					gameObject.spritesheetIndex = 0x2D;
				}
				else if (gameObject.Timer > 4)
				{
					gameObject.spritesheetIndex = 0x2E;
				}
				else
				{
					gameObject.spritesheetIndex = 0x2F;
				}
			}	
		}

		public void Interact(GameObject own, GameObject other)
		{
		}
	}
}
