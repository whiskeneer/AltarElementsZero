using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.enemies;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.player
{
	class Scythe : IBehaviour
	{

		public static readonly Scythe Instance = new ();

		[Flags]
		public enum FlagTypes : UInt32
		{
			None = 0,
			Bounce = 1 << 0,
		}
		public enum State : uint
		{
			INACTIVE,
			ACTIVE,
			ACTIVE_DOWN,
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


			gameObject.atlasReference.Start = LegacyMapper.StartFromOraSpritesheetIndex(0x00);
			gameObject.atlasReference.Size = new PxSize(64, 32);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);

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
				case State.ACTIVE_DOWN:
					attackTimer--;
					if (attackTimer == 0)
					{
						gameObject.State = (uint)State.COOLDOWN;
						cooldownTimer = 1;
					}
					break;
				case State.ACTIVE:
					attackTimer--;
					if(attackTimer == 0){
						gameObject.State = (uint)State.COOLDOWN;
						cooldownTimer = 1;
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
						//gameObject.spritesheetIndex = 0x30;
						gameObject.atlasReference.Start = LegacyMapper.StartFromOraSpritesheetIndex(0x30);
					}
					else
					{
						//gameObject.spritesheetIndex = 0x32;
						gameObject.atlasReference.Start = LegacyMapper.StartFromOraSpritesheetIndex(0x32);
					}
					break;
				case State.ACTIVE_DOWN:
					gameObject.drawOrder = GameObject.DrawOrderTypes.FRONT;
					gameObject.currentBoundingBox.Size = new PxSize(17, 16).ToSubpx();
					gameObject.Type = GameObject.Types.REGION;

					if ((attackTimer & 4) == 4)
					{
						//gameObject.spritesheetIndex = 0x34;
						gameObject.atlasReference.Start = LegacyMapper.StartFromOraSpritesheetIndex(0x34);
					}
					else
					{
						//gameObject.spritesheetIndex = 0x36;
						gameObject.atlasReference.Start = LegacyMapper.StartFromOraSpritesheetIndex(0x36);
					}
					break;
			}

		}

		public void Interact(GameObject own, GameObject other)
		{
			if(own.State == (uint)State.ACTIVE_DOWN){
				if (object.ReferenceEquals(other.behaviour, Toki.Instance) || 
					object.ReferenceEquals(other.behaviour,Ufo.Instance) ||
					object.ReferenceEquals(other.behaviour,BreakableTile.Instance))
				{
					own.InteractionFlags |= (UInt32)FlagTypes.Bounce;
				}
			}
		}
	}
}
