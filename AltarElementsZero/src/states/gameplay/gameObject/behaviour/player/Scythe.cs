using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.player
{
	class Scythe : IBehaviour
	{

		public static readonly Scythe Instance = new ();

		public enum State : uint
		{
			INACTIVE,
			ACTIVE,
			COOLDOWN
		}

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.RESERVED;
			gameObject.isPersistentAcrossChunks = true;

			gameObject.drawOrder = GameObject.DrawOrderTypes.NONE; // later, FRONT
			// spritesheetIndex
			// SpriteOffset
			// spriteEffects
			// currentBoundingBox.Size
			
			//ref uint cooldownTimer = ref gameObject.Timer;
			ref uint attackTimer = ref gameObject.Timer2;
			gameObject.State = (uint)State.INACTIVE;
			attackTimer = 13;

		}

		public void Update(GameObject gameObject)
		{
			ref uint cooldownTimer = ref gameObject.Timer;
			ref uint attackTimer = ref gameObject.Timer2;

			switch((State)gameObject.State)
			{
				case State.COOLDOWN:
					cooldownTimer--;
					if(cooldownTimer == 0){
						gameObject.State = (uint)State.INACTIVE;
						attackTimer = 13;
					}
					break;
				case State.INACTIVE:
					break;
				case State.ACTIVE:
					attackTimer--;
					if(attackTimer == 0){
						gameObject.State = (uint)State.COOLDOWN;
						cooldownTimer = 12;
					}
					break;
			}

			switch((State)gameObject.State){
				case State.COOLDOWN:
					gameObject.drawOrder = GameObject.DrawOrderTypes.NONE;
					gameObject.currentBoundingBox.Size = new(0,0);
					gameObject.Type = GameObject.Types.RESERVED;
					break;
				case State.INACTIVE:
					gameObject.drawOrder = GameObject.DrawOrderTypes.NONE;
					gameObject.currentBoundingBox.Size = new(0, 0);
					gameObject.Type = GameObject.Types.RESERVED;
					break;
				case State.ACTIVE:
					gameObject.drawOrder = GameObject.DrawOrderTypes.FRONT;
					gameObject.currentBoundingBox.Size = new PxSize(40,11).ToSubpx();
					gameObject.Type = GameObject.Types.REGION;

					if (attackTimer > 6){
						gameObject.spritesheetIndex = 0x30;
					}else{
						gameObject.spritesheetIndex = 0x32;
					}
					break;
			}

		}

		public void Interact(GameObject own, GameObject other)
		{

		}
	}
}
