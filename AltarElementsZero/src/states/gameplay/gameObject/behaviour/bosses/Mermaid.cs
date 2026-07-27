using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.player;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.bosses
{

	class Boss1MovingPlatform : IBehaviour
	{
		public static readonly Boss1MovingPlatform Instance = new();

		public static byte GetSpawnValueFromParameters(int tileOffsetX, bool movingRight, uint floorLevel)
		{
			byte b = 0;
			if (movingRight) b |= 0x80;
			b |= (byte)((floorLevel << 4)&0x30); // for sprite selection
			b |= (byte)(tileOffsetX & 0x0f);
			return b;
		}
		public static void GetParametersFromSpawnValue(byte spawnValue, out int tileOffsetX,out bool movingRight,out uint floorLevel)
		{
			movingRight = (spawnValue & 0x80) == 0x80;
			floorLevel = (uint)(spawnValue >> 4) & 0x3;
			tileOffsetX = (spawnValue & 0xf);
			if ((tileOffsetX & 0x8) == 0x8) tileOffsetX |= ~(0xf);
		}

		private enum State
		{
			GOING_RIGHT,
			GOING_LEFT,
		}
		public void Init(GameObject gameObject)
		{
			GetParametersFromSpawnValue(gameObject.spawnValue, out int tileOffsetX, out bool movingRight, out uint floorLevel);

			gameObject.Type = GameObject.Types.UNSTOPPABLE;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
			gameObject.atlasReference.Start = new PxPosition(512 + 32 * floorLevel, 864);
			gameObject.atlasReference.Size = new PxSize(32, 16);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);

			gameObject.currentBoundingBox.Position += new TilePosition((uint)(tileOffsetX & uint.MaxValue), 0).ToPx().ToSubpx();
			gameObject.previousBoundingBox.Position = gameObject.currentBoundingBox.Position;

			gameObject.currentBoundingBox.Size = new PxSize(32, 16).ToSubpx();

			gameObject.State = movingRight switch 
			{	
				true => (uint)State.GOING_RIGHT,
				_ => (uint)State.GOING_LEFT,
			};
		}
		public void Update(GameObject gameObject)
		{
			GameObject.signalFlags!.GetChunkLimits(out uint _, out uint _, out uint limitLeft, out uint limitRight);

			switch ((State)gameObject.State)
			{
				case State.GOING_LEFT:
					gameObject.currentVelocity = new SubpxVelocity(-(1 << Configuration.Px.SubpxPower),0);
					if (gameObject.currentBoundingBox.Position.X + gameObject.currentBoundingBox.Size.X <= limitLeft)
					{
						gameObject.previousBoundingBox.Position.X += (uint)16 << Configuration.Tile.SubpxPower;
						gameObject.currentBoundingBox.Position.X += (uint)16 << Configuration.Tile.SubpxPower;
					}
					break;
				case State.GOING_RIGHT:
					gameObject.currentVelocity = new SubpxVelocity((1 << Configuration.Px.SubpxPower),0);
					if(gameObject.currentBoundingBox.Position.X >= limitRight)
					{
						gameObject.previousBoundingBox.Position.X -= (uint)16 << Configuration.Tile.SubpxPower;
						gameObject.currentBoundingBox.Position.X -= (uint)16 << Configuration.Tile.SubpxPower;
					}
					break;
			}

		}
		public void Interact(GameObject own, GameObject other)
		{

		}
	}


	class Trident : IBehaviour
	{
		public static readonly Trident Instance = new();

		public enum SpawnValue
		{
			FACING_RIGHT,
			FACING_LEFT,
		}

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.REGION;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.FRONT;

			gameObject.atlasReference.Start = new PxPosition(320, 544);
			gameObject.atlasReference.Size = new PxSize(24, 9);
			gameObject.atlasReference.Effects = (SpawnValue)gameObject.spawnValue switch
			{
				SpawnValue.FACING_RIGHT => SpriteEffects.FlipHorizontally,
				_ => SpriteEffects.None,
			};
			gameObject.atlasReference.Offset = new PxPosition(1, 3);

			gameObject.currentBoundingBox.Size = new PxSize(22, 3).ToSubpx();

			gameObject.Timer = 0;
			//Console.WriteLine("I EXIST");

		}
		public void Update(GameObject gameObject)
		{
			gameObject.Timer++;
			if(gameObject.Timer >= Mermaid.ATTACKING_HOLDING_TIME)
			{
				gameObject.atlasReference.Start = new PxPosition(320, 560);

				if((SpawnValue)gameObject.spawnValue == SpawnValue.FACING_RIGHT)
				{
					gameObject.currentBoundingBox.Position.X += (uint)(2 << Configuration.Px.SubpxPower);
					GameObject.signalFlags!.GetChunkLimits(out uint _, out uint _, out uint _, out uint limitRight);
					if(gameObject.currentBoundingBox.Position.X > limitRight)
					{
						gameObject.Delete();
					}
				}
				else
				{
					gameObject.currentBoundingBox.Position.X -= (uint)(2 << Configuration.Px.SubpxPower);
					GameObject.signalFlags!.GetChunkLimits(out uint limitLeft, out uint _, out uint _, out uint _);
					if (gameObject.currentBoundingBox.Position.X < limitLeft - gameObject.currentBoundingBox.Size.X)
					{
						gameObject.Delete();
					}
				}

			}else{


			}

		}
		public void Interact(GameObject own, GameObject other)
		{

		}
	}
	class Mermaid : IBehaviour
	{
		public static readonly Mermaid Instance = new();

		private enum State
		{
			INIT,
			ATTACKING_FROM_LEFT,
			ATTACKING_FROM_RIGHT,
			DEFEATED,
		}
		private enum SubState
		{
			NONE,
			HURT,
		}

		[Flags]
		public enum FlagTypes : UInt32
		{
			None = 0,
			Hurt = 1 << 0,
		}

		const uint ATTACKING_COOLDOWN_TIME = 90;
		const uint ATTACKING_CHARGING_TIME = 20;
		public const uint ATTACKING_HOLDING_TIME = 20;
		const uint ATTACKING_THROWING_TIME = 40;

		const uint ATTACKING_COOLDOWN_END = ATTACKING_COOLDOWN_TIME;
		const uint ATTACKING_CHARGING_END = ATTACKING_COOLDOWN_END + ATTACKING_CHARGING_TIME;
		const uint ATTACKING_HOLDING_END = ATTACKING_CHARGING_END + ATTACKING_HOLDING_TIME;
		const uint ATTACKING_THROWING_END = ATTACKING_HOLDING_END + ATTACKING_THROWING_TIME;


		public void Init(GameObject gameObject)
		{
			ref uint actionTimer = ref gameObject.Timer;
			ref uint animationTimer = ref gameObject.Timer3;
			ref uint health = ref gameObject.Timer4;
		
			gameObject.Type = GameObject.Types.REGION;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.NONE;
			gameObject.atlasReference.Start = new PxPosition(256, 512);
			gameObject.atlasReference.Size = new PxSize(32, 32);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);

			gameObject.currentBoundingBox.Size = new PxSize(32, 32).ToSubpx();

			gameObject.State = (uint)State.INIT;
			gameObject.SubState = (uint)SubState.NONE;
			health = 5;
			animationTimer = 0;
			actionTimer = 0;

		}

		public void Update(GameObject gameObject)
		{
			ref uint actionTimer = ref gameObject.Timer;
			ref uint animationTimer = ref gameObject.Timer3;
			ref uint health = ref gameObject.Timer4;

			if(((FlagTypes)gameObject.InteractionFlags & FlagTypes.Hurt) == FlagTypes.Hurt)
			{
				if((SubState)gameObject.SubState == SubState.NONE)
				{
					gameObject.SubState = (uint)SubState.HURT;
					health--;

					GameObject.globalAssets!.HitSFXInstance!.Stop();
					GameObject.globalAssets!.HitSFXInstance!.Play();

					if (gameObject.linkedObject != null)
					{
						gameObject.linkedObject.Delete();
						gameObject.linkedObject = null;
					}
					if(health <= 0)
					{
						gameObject.drawOrder = GameObject.DrawOrderTypes.FRONT;

						GameObject.signalFlags!.EmitGameplayMessage(GameplayMessages.TriggerLevel1BossEnd);
						gameObject.State = (uint)State.DEFEATED;
						actionTimer = 0;
						gameObject.SavedSpeed = -240;
					}

				}
			}

			// movement
			switch((State)gameObject.State)
			{
				case State.INIT:
					{				
						gameObject.drawOrder = GameObject.DrawOrderTypes.BACK;
						gameObject.State = (uint)State.ATTACKING_FROM_LEFT;
						actionTimer = 0;
						GameObject.signalFlags!.GetChunkLimits(out uint limitUp, out uint _, out uint limitLeft, out uint _);
						gameObject.currentBoundingBox.Position = new SubpxPosition(limitLeft,limitUp);
					}
					break;
				case State.DEFEATED:
					if(actionTimer < 30)
					{
						// do nothing
					}
					else
					{
						gameObject.SavedSpeed += 10;
						gameObject.currentBoundingBox.Position.Y += (uint)gameObject.SavedSpeed;
					}
					actionTimer++;
					break;
				case State.ATTACKING_FROM_LEFT:
				case State.ATTACKING_FROM_RIGHT: // modify
					if((SubState)gameObject.SubState == SubState.HURT)
					{
						gameObject.currentBoundingBox.Position.Y -= (uint)2 << Configuration.Px.SubpxPower;
						GameObject.signalFlags!.GetChunkLimits(out uint limitUp, out uint _, out uint limitLeft, out uint limitRight);

						if (gameObject.currentBoundingBox.Position.Y <= limitUp)
						{
							gameObject.State = (State)gameObject.State switch
							{
								State.ATTACKING_FROM_RIGHT => (uint)State.ATTACKING_FROM_LEFT,
								_ => (uint)State.ATTACKING_FROM_RIGHT,
							};
							gameObject.SubState = (uint)SubState.NONE;
							actionTimer = 0;
							gameObject.currentBoundingBox.Position = (State)gameObject.State switch
							{
								State.ATTACKING_FROM_RIGHT => new SubpxPosition(limitRight - (uint)(32 << Configuration.Px.SubpxPower), limitUp),
								_ => new SubpxPosition(limitLeft, limitUp),
							};
						}
					}
					else
					{
						gameObject.linkedPosition = (State)gameObject.State switch
						{
							State.ATTACKING_FROM_LEFT => new PxPosition((uint)((20) & uint.MaxValue), 12).ToSubpx(),
							_ => new PxPosition((uint)((32 - 20 - 22) & uint.MaxValue), 12).ToSubpx(),

						};

						uint playerY =  GameObject.signalFlags!.GetPlayerPosition().Y;
						uint mermaidY = gameObject.currentBoundingBox.Center().Y;
						int diff = ((int)playerY - (int)mermaidY) >> 2;
						if(diff < -(4<<Configuration.Px.SubpxPower))
						{
							diff = -(4 << Configuration.Px.SubpxPower);
						}
						if (diff > (4 << Configuration.Px.SubpxPower))
						{
							diff = (4 << Configuration.Px.SubpxPower);
						}
						gameObject.currentBoundingBox.Position.Y += (uint)(diff);
						//Console.WriteLine(gameObject.currentBoundingBox.Position.Y);

						actionTimer++;
						Trident.SpawnValue spawnValue = (State)gameObject.State switch
						{ 
							State.ATTACKING_FROM_LEFT => Trident.SpawnValue.FACING_RIGHT,
							_ => Trident.SpawnValue.FACING_LEFT,

						};
						if(actionTimer == ATTACKING_CHARGING_END)
						{
							GameObject.signalFlags!.CreateAndAttachObject(
								Trident.Instance, 
								(byte)spawnValue,
								//gameObject.currentBoundingBox.Position
								gameObject
							);
						}
						if(actionTimer == ATTACKING_HOLDING_END)
						{
							gameObject.linkedObject = null;

						}
					
						if(actionTimer >= ATTACKING_THROWING_END)
						{
							actionTimer = 0;	
						}
					}

					break;
			}

			// animation
			animationTimer++;
			switch((State)gameObject.State)
			{
				case State.DEFEATED:
					gameObject.atlasReference.Start = new PxPosition(320, 576);
					if(gameObject.SavedSpeed > 0)
					{
						gameObject.atlasReference.Effects |= SpriteEffects.FlipVertically;
					}
					break;
				case State.ATTACKING_FROM_LEFT:
				case State.ATTACKING_FROM_RIGHT:
					if((State)gameObject.State == State.ATTACKING_FROM_LEFT)
					{
						gameObject.atlasReference.Effects = SpriteEffects.FlipHorizontally;
					}
					else
					{
						gameObject.atlasReference.Effects = SpriteEffects.None;
					}

					if ((SubState)gameObject.SubState == SubState.HURT)
					{
						gameObject.atlasReference.Start = ((animationTimer >> 1) & 0x01) switch
						{
							0 => new PxPosition(320, 576),
							_ => new PxPosition(320 + 32, 576),
						};
					}
					else
					{
						if(actionTimer < ATTACKING_COOLDOWN_END)
						{
							gameObject.atlasReference.Start = ((animationTimer >> 3) & 0x03) switch
							{
								0 => new PxPosition(256 + 32, 512),
								2 => new PxPosition(256 + 64, 512),
								_ => new PxPosition(256, 512),
							};
						}
						else if(actionTimer < ATTACKING_CHARGING_END)
						{
							gameObject.atlasReference.Start = ((animationTimer >> 1) & 0x01) switch
							{
								0 => new PxPosition(256, 544),
								_ => new PxPosition(256 + 32, 544),
							};
						}
						else if(actionTimer < ATTACKING_HOLDING_END)
						{
							gameObject.atlasReference.Start = new PxPosition(256, 544);
						}
						else
						{
							if(actionTimer < ATTACKING_HOLDING_END + 5)
							{
								gameObject.atlasReference.Start = new PxPosition(256, 544 + 32);
							}
							else
							{
								gameObject.atlasReference.Start = new PxPosition(256 + 32, 544 + 32);
							}
						}
					}

					break;
			}


		}

		public void Interact(GameObject own, GameObject other)
		{
			if(other.behaviour == Scythe.Instance)
			{
				own.InteractionFlags |= (uint)FlagTypes.Hurt;
			}
		}

	}
}
