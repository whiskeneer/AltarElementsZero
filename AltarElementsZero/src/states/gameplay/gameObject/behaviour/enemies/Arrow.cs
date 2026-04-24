using System.Data;
using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.enemies
{
	class Arrow : IBehaviour
	{
		[Flags]
		private enum FlagTypes : UInt32
		{
			None = 0,
			Broken = 1 << 0
		}

		public static readonly Arrow Instance = new();

		enum State : uint
		{
			GOING_LEFT,
			GOING_RIGHT,
		}

		public void Init(GameObject gameObject){
			gameObject.Type = GameObject.Types.PROJECTILE;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
			gameObject.spritesheetIndex = 0x0d;

			gameObject.Timer = 3 * 60;
			
			switch(gameObject.spawnValue)
			{
				case 1:
					// going right
					gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
					gameObject.currentBoundingBox.Size = new PxSize(14, 3).ToSubpx();
					gameObject.SpriteOffset = new PxSize(18, 6);
					gameObject.currentVelocity = new SubpxVelocity(64,0);
					break;
				default: 
					// going left
					gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
					gameObject.currentBoundingBox.Size = new PxSize(14, 3).ToSubpx();
					gameObject.SpriteOffset = new PxSize(0,6);
					gameObject.currentVelocity = new SubpxVelocity(-64, 0);
					break;
			}
			
		}
		public void Update(GameObject gameObject){
			if( --gameObject.Timer == 0){
				gameObject.Delete();
			}
			else if(((FlagTypes)gameObject.InteractionFlags & FlagTypes.Broken)==FlagTypes.Broken){
				gameObject.Delete();
			}else if(
				gameObject.PushedUp || gameObject.PushedDown ||
				gameObject.PushedLeft || gameObject.PushedRight ||
				gameObject.PushedPreviouslyUp || gameObject.PushedPreviouslyDown ||
				gameObject.PushedPreviouslyLeft || gameObject.PushedPreviouslyRight
			){
				gameObject.Delete();
			}
		}

		public void Interact(GameObject own, GameObject other)
		{
			if(other.Type == GameObject.Types.PUSHABLE){
				if(other.behaviour != Ufo.Instance && 
					other.behaviour != Toki.Instance){

					own.InteractionFlags |= (uint)FlagTypes.Broken;
				}
			}else if(other.Type == GameObject.Types.IMMOBILE ||
				other.Type == GameObject.Types.UNSTOPPABLE){

				own.InteractionFlags |= (uint)FlagTypes.Broken;
			}
		}

	}
}
