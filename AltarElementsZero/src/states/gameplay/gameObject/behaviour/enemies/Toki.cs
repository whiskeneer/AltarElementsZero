using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.effects;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.player;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.enemies
{
    class Toki : IBehaviour
    {
		[Flags]
		public enum FlagTypes : UInt32
		{
			None = 0,
			Hurt = 1 << 0,
		}

        public static readonly Toki Instance = new();

        enum State : uint
        {
            GOING_LEFT,
            GOING_RIGHT,
            ATTACKING_LEFT,
            ATTACKING_RIGHT,
            SHIFTING_TO_LEFT,
            SHIFTING_TO_RIGHT,
			STILL_LEFT,
        }

        public void Init(GameObject gameObject)
        {
			ref uint AnimationTimer = ref gameObject.Timer;

			gameObject.Type = GameObject.Types.PUSHABLE;
			gameObject.isPersistentAcrossChunks = false;


			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;

			gameObject.currentBoundingBox.Size = new PxSize(12, 12).ToSubpx();

			//gameObject.SpriteOffset = new PxSize(10, 20);
   //         gameObject.spritesheetIndex = 0;
   //         gameObject.spriteEffects = SpriteEffects.None;
			gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x00);
			gameObject.atlasReference.Size = new PxSize(32, 32);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(10, 20);


			if (gameObject.spawnValue == 0)
			{
				gameObject.State = (uint)State.GOING_LEFT;
				AnimationTimer = 60 * 4;
			}
			else if(gameObject.spawnValue == 1)
			{
				gameObject.State = (uint)State.GOING_RIGHT;
				AnimationTimer = 60 * 4;
			}
			else
			{
				gameObject.State = (uint)State.STILL_LEFT;
			}
		}

        public void Update(GameObject gameObject)
        {
			ref uint AnimationTimer = ref gameObject.Timer;
			ref uint CooldownTimer = ref gameObject.Timer2;

			if (CooldownTimer > 0) CooldownTimer--;

			
			if((State)gameObject.State == State.ATTACKING_LEFT && AnimationTimer == 32){
				GameObject.signalFlags!.CreateGameObject(
					Arrow.Instance,
					0,
					new SubpxPosition
					(
						gameObject.currentBoundingBox.Position.X - 3*64,// - 1,
						gameObject.currentBoundingBox.Position.Y
					)
				);
				CooldownTimer = 90;
			}
			if ((State)gameObject.State == State.ATTACKING_RIGHT && AnimationTimer == 32)
			{
				GameObject.signalFlags!.CreateGameObject(
					Arrow.Instance,
					1,
					new SubpxPosition
					(
						gameObject.currentBoundingBox.Position.X + 1*64,// - 1,
						gameObject.currentBoundingBox.Position.Y
					)
				);
				CooldownTimer = 90;
			}

			if ((State)gameObject.State == State.GOING_LEFT && CooldownTimer == 0){
				ObjectBoundingBox regionToCheck = gameObject.currentBoundingBox;
				regionToCheck.Position.X -= 5 * 16 * 64;
				regionToCheck.Size.X += 5 * 16 * 64;
				if (regionToCheck & GameObject.signalFlags!.GetPlayerPosition())
				{
					AnimationTimer = 64;
					gameObject.State = (uint)State.ATTACKING_LEFT;
				}
			}
			if((State)gameObject.State == State.GOING_RIGHT && CooldownTimer == 0)
			{
				ObjectBoundingBox regionToCheck = gameObject.currentBoundingBox;
				//regionToCheck.Position.X -= 5 * 16 * 64;
				regionToCheck.Size.X += 5 * 16 * 64;
				if (regionToCheck & GameObject.signalFlags!.GetPlayerPosition())
				{
					AnimationTimer = 64;
					gameObject.State = (uint)State.ATTACKING_RIGHT;
				}
			}

			if (--AnimationTimer == 0)
            {
                switch ((State)gameObject.State)
                {
                    case State.GOING_LEFT:
                        gameObject.State = (uint)State.SHIFTING_TO_RIGHT;
                        AnimationTimer = 24;
                        break;
                    case State.GOING_RIGHT:
						gameObject.State = (uint)State.SHIFTING_TO_LEFT;
						AnimationTimer = 24;
						break;
                    case State.SHIFTING_TO_LEFT:
						gameObject.State = (uint)State.GOING_LEFT;
						AnimationTimer = 60*4;
						break;
                    case State.SHIFTING_TO_RIGHT:
						gameObject.State = (uint)State.GOING_RIGHT;
						AnimationTimer = 60 * 4;
						break;
					case State.ATTACKING_LEFT:
						gameObject.State = (uint)State.GOING_LEFT;
						AnimationTimer = 60 * 4;
						break;
					case State.ATTACKING_RIGHT:
						gameObject.State = (uint)State.GOING_RIGHT;
						AnimationTimer = 60 * 4;
						break;
				}
            }

			switch ((State)gameObject.State)
			{
				case State.GOING_LEFT:
					gameObject.GroundImpulse= -16;
					//gameObject.spritesheetIndex = (AnimationTimer >>4)&3;
					//gameObject.spriteEffects = SpriteEffects.None;
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex((AnimationTimer >> 4) & 3);
					gameObject.atlasReference.Effects = SpriteEffects.None;
					break;
				case State.GOING_RIGHT:
					gameObject.GroundImpulse= 16;
					//gameObject.spritesheetIndex = (AnimationTimer >> 4) & 3;
					//gameObject.spriteEffects = SpriteEffects.FlipHorizontally;
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex((AnimationTimer >> 4) & 3);
					gameObject.atlasReference.Effects = SpriteEffects.FlipHorizontally;
					break;

				case State.ATTACKING_LEFT:
					gameObject.GroundImpulse = 0;
					//gameObject.spriteEffects = SpriteEffects.None;
					gameObject.atlasReference.Effects = SpriteEffects.None;

					if (AnimationTimer >= 32 - 8 && AnimationTimer < 32 + 8){
						//gameObject.spritesheetIndex = 7;
						gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(7);
					}
					else
					{
						//gameObject.spritesheetIndex = 6;
						gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(6);
					}
					break;
				case State.ATTACKING_RIGHT:
					gameObject.GroundImpulse = 0;
					//gameObject.spriteEffects = SpriteEffects.FlipHorizontally;
					gameObject.atlasReference.Effects = SpriteEffects.FlipHorizontally;
					if (AnimationTimer >= 32 - 8 && AnimationTimer < 32 + 8)
					{
						//gameObject.spritesheetIndex = 7;
						gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(7);
					}
					else
					{
						//gameObject.spritesheetIndex = 6;
						gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(6);
					}
					break;
				case State.SHIFTING_TO_RIGHT:
					gameObject.GroundImpulse = 0;
                    if(AnimationTimer > 16)
                    {
						//gameObject.spritesheetIndex = 4;
						//gameObject.spriteEffects = SpriteEffects.None;
						gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(4);
						gameObject.atlasReference.Effects = SpriteEffects.None;
					}
                    else if(AnimationTimer > 8)
                    {
						//gameObject.spritesheetIndex = 5;
						//gameObject.spriteEffects = SpriteEffects.None;
						gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(5);
						gameObject.atlasReference.Effects = SpriteEffects.None;
					}
                    else
                    {
						//gameObject.spritesheetIndex = 4;
						//gameObject.spriteEffects = SpriteEffects.FlipHorizontally;
						gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(4);
						gameObject.atlasReference.Effects = SpriteEffects.FlipHorizontally;
					}

					break;
                case State.SHIFTING_TO_LEFT:
					gameObject.GroundImpulse = 0;
					if (AnimationTimer > 16)
					{
						//gameObject.spritesheetIndex = 4;
						//gameObject.spriteEffects = SpriteEffects.FlipHorizontally;
						gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(4);
						gameObject.atlasReference.Effects = SpriteEffects.FlipHorizontally;
					}
					else if (AnimationTimer > 8)
					{
						//gameObject.spritesheetIndex = 5;
						//gameObject.spriteEffects = SpriteEffects.None;
						gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(5);
						gameObject.atlasReference.Effects = SpriteEffects.None;
					}
					else
					{
						//gameObject.spritesheetIndex = 4;
						//gameObject.spriteEffects = SpriteEffects.None;
						gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(4);
						gameObject.atlasReference.Effects = SpriteEffects.None;
					}
					break;
				case State.STILL_LEFT:
					gameObject.GroundImpulse = 0;
					//gameObject.spritesheetIndex = 0;
					//gameObject.spriteEffects = SpriteEffects.None;
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0);
					gameObject.atlasReference.Effects = SpriteEffects.None;
					break;
				default:
					gameObject.GroundImpulse = 0;
					break;
			}

			gameObject.SimulateRegularObjectPhysics();

			//

			if(((FlagTypes)gameObject.InteractionFlags & FlagTypes.Hurt) == FlagTypes.Hurt)
			{
				//gameObject.Delete();	
				SubpxPosition center = gameObject.currentBoundingBox.Center();
				gameObject.Delete();
				gameObject.behaviour = EnemyDefeated.Instance;
				gameObject.Init();
				gameObject.currentBoundingBox.Position = center;
			}
		}

		public void Interact(GameObject own, GameObject other)
		{
			IBehaviour otherBehaviour = other.behaviour;
			if(otherBehaviour == Scythe.Instance)
			{
				own.InteractionFlags |= (UInt32)FlagTypes.Hurt;
			}
			if (otherBehaviour == Barrel.Instance && (Barrel.State)other.State != Barrel.State.STOPPED)
			{
				own.InteractionFlags |= (UInt32)FlagTypes.Hurt;
			}
		}

	}
}
