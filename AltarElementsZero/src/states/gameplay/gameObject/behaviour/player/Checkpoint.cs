using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.player
{
	class Checkpoint : IBehaviour
	{
		private const uint LIGHTING_FRAMES = 8;

		public static readonly Checkpoint Instance = new ();

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.REGION;
			gameObject.isPersistentAcrossChunks = false;

			//gameObject.isVisible = true;
			gameObject.drawOrder = GameObject.DrawOrderTypes.BACK;
			gameObject.spritesheetIndex = 0x10;
			gameObject.SpriteOffset = new(8,0);
			gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

			gameObject.currentBoundingBox.Size = new PxSize(16, 16).ToSubpx();

			gameObject.Timer = 0;
			gameObject.Timer2 = LIGHTING_FRAMES;
		}

		public void Update(GameObject gameObject)
		{
			bool activated = (GameObject.signalFlags!.GetCheckpointValue() == gameObject.spawnValue);

			if (activated)
			{
				if(gameObject.Timer2 > 0){
					gameObject.Timer2--;
					gameObject.spritesheetIndex = 0x11;
				}
				else
				{
					if((gameObject.Timer & (1<<3)) != 0)
					{
						gameObject.spritesheetIndex = 0x13;
					}
					else
					{
						gameObject.spritesheetIndex = 0x12;
					}
					gameObject.Timer++;
				}
			}
			else
			{
				gameObject.Timer = 0;
				gameObject.Timer2 = LIGHTING_FRAMES;
				gameObject.spritesheetIndex = 0x10;
			}
		}

		public void Interact(GameObject own, GameObject other){
			bool activated = (GameObject.signalFlags!.GetCheckpointValue() == own.spawnValue);

			if(!activated && object.ReferenceEquals(other.behaviour, Ora.Instance)){
				GameObject.signalFlags!.SetCheckpoint(own.spawnValue, own.currentBoundingBox.Position.ToPx().ToTile());
			}

		}

	}
}
