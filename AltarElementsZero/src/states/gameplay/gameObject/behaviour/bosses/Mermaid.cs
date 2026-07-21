using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.bosses
{
	class TridentRight : IBehaviour
	{
		public static readonly TridentRight Instance = new();
		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.REGION;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.FRONT;
			gameObject.atlasReference.Start = new PxPosition(320, 544);
			gameObject.atlasReference.Size = new PxSize(24, 9);
			gameObject.atlasReference.Effects = SpriteEffects.FlipHorizontally;
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
				gameObject.currentBoundingBox.Position.X += (uint)(2 << Configuration.Px.SubpxPower);
				GameObject.signalFlags!.GetChunkLimits(out uint _, out uint _, out uint _, out uint limitRight);
				if(gameObject.currentBoundingBox.Position.X > limitRight)
				{
					//Console.WriteLine("I DONT EXIST ANYMORE");
					gameObject.Delete();
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
		}
		private enum SubState
		{
			NONE,
			HURT
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

			// movement
			switch((State)gameObject.State)
			{
				case State.INIT:
					gameObject.drawOrder = GameObject.DrawOrderTypes.BACK;
					gameObject.State = (uint)State.ATTACKING_FROM_LEFT;
					actionTimer = 0;
					GameObject.signalFlags!.GetChunkLimits(out uint limitUp, out uint _, out uint limitLeft, out uint _);
					gameObject.currentBoundingBox.Position = new SubpxPosition(limitLeft,limitUp);
					break;

				case State.ATTACKING_FROM_LEFT:
					gameObject.linkedPosition = new PxPosition((uint)((-8) & uint.MaxValue),12).ToSubpx();

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
					if(actionTimer == ATTACKING_CHARGING_END)
					{
						GameObject.signalFlags!.CreateAndAttachObject(
							TridentRight.Instance, 
							0,
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

					break;
			}

			// animation
			animationTimer++;
			switch((State)gameObject.State)
			{
				case State.ATTACKING_FROM_LEFT:
					gameObject.atlasReference.Effects = SpriteEffects.FlipHorizontally;
					if(actionTimer < ATTACKING_COOLDOWN_TIME)
					{
						gameObject.atlasReference.Start = ((animationTimer >> 3) & 0x03) switch
						{
							0 => new PxPosition(256 + 32, 512),
							2 => new PxPosition(256 + 64, 512),
							_ => new PxPosition(256, 512),
						};
					}
					else
					{
						gameObject.atlasReference.Start = new PxPosition(256, 544);
					}
					break;
			}


		}

		public void Interact(GameObject own, GameObject other)
		{

		}

	}
}
