using System;
using System.Collections.Generic;
using System.Text;
using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.enemies;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.player;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.triggers
{
	class Level1EndTrigger : IBehaviour
	{
		public static readonly Level1EndTrigger Instance = new ();

		private enum State
		{
			WAITING_FOR_PLAYER,
			STARTED
		}

		[Flags]
		public enum FlagTypes : UInt32
		{
			None = 0,
			Start = 1 << 0,
		}


		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.REGION;
			gameObject.isPersistentAcrossChunks = false;
			gameObject.drawOrder = GameObject.DrawOrderTypes.NONE;
			gameObject.currentBoundingBox.Size = new TileSize(10, 1).ToPx().ToSubpx();

			gameObject.State = (uint)State.WAITING_FOR_PLAYER;
		}

		public void Update(GameObject gameObject)
		{
			if(gameObject.State == (uint)State.WAITING_FOR_PLAYER && 
				(gameObject.InteractionFlags & (uint)FlagTypes.Start) == (uint)FlagTypes.Start)
			{
				gameObject.State = (uint)State.STARTED;

				GameObject.signalFlags!.CreateGameObject(
					MermaidLevel1EndCutscene.Instance,
					0,
					gameObject.currentBoundingBox.Position + new TilePosition(7,(uint)((-8)&uint.MaxValue)).ToPx().ToSubpx()
				);
			}
		}

		public void Interact(GameObject own, GameObject other)
		{
			if(other.behaviour == Ora.Instance)
			{
				own.InteractionFlags |= (uint)FlagTypes.Start;
			}
		}
	}

	class FloorLevel1EndCutscene : IBehaviour
	{
		public static readonly FloorLevel1EndCutscene Instance = new();

		public enum State
		{
			SOLID,
			BREAKING,
			BROKEN
		}
		public enum FlagTypes : UInt32
		{
			None = 0,
			Break = 1 << 0,
		}
		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.IMMOBILE;
			gameObject.isPersistentAcrossChunks = false;
			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
			gameObject.currentBoundingBox.Size = new PxSize(160, 16).ToSubpx();
			gameObject.atlasReference.Start = new PxPosition(512, 832);
			gameObject.atlasReference.Size = new PxSize(32, 32);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 8);
			gameObject.atlasReference.RepeatX = 4;

			gameObject.State = (uint)State.SOLID;
		}
		public void Update(GameObject gameObject)
		{
			switch((State)gameObject.State)
			{
				case State.SOLID:
					if(((FlagTypes)gameObject.InteractionFlags & FlagTypes.Break) == FlagTypes.Break)
					{
						gameObject.State = (uint)State.BREAKING;
						gameObject.Timer = 0;
					}
					break;
				case State.BREAKING:
					gameObject.Timer++;

					gameObject.atlasReference.Start = ((gameObject.Timer >> 2) & 1) switch
					{
						1 => new PxPosition(512 + 32, 832),
						_ => new PxPosition(512 + 64, 832),
					};

					if(gameObject.Timer >= 90)
					{
						gameObject.State = (uint)State.BROKEN;
						gameObject.Timer = 0;

						gameObject.Type = GameObject.Types.PUSHABLE;
						//gameObject.currentBoundingBox.Size = new();
					}

					break;
				case State.BROKEN:
					gameObject.Timer++;
					gameObject.atlasReference.Start = new PxPosition(512 + 96, 832);

					if(gameObject.Timer >= 90)
					{
						GameObject.signalFlags!.SetTeleportDestiny(1, 0);
						GameObject.signalFlags!.EmitGameplayMessage(GameplayMessages.TriggerLevel1BossTransition);
					}
					break;
			}

		}

		public void Interact(GameObject own, GameObject other)
		{
			if(other.behaviour == TridentLevel1EndCutscene.Instance)
			{
				own.InteractionFlags |= (uint)FlagTypes.Break;
			}
		}

	}

	class TridentLevel1EndCutscene : IBehaviour
	{

		public static readonly TridentLevel1EndCutscene Instance = new();

		public enum State
		{
			HELD,
			THROWN
		}

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.REGION;
			gameObject.isPersistentAcrossChunks = false;
			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
			gameObject.currentBoundingBox.Size = new PxSize(9, 24).ToSubpx();
			gameObject.atlasReference.Start = new PxPosition(352, 552);
			gameObject.atlasReference.Size = new PxSize(9, 24);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);

			gameObject.State = (uint)State.HELD;
			gameObject.Timer = 0;
		}
		public void Update(GameObject gameObject)
		{
			if (gameObject.State == (uint)State.HELD)
			{
				gameObject.Timer++;
				if(gameObject.Timer == 60)
				{
					gameObject.State = (uint)State.THROWN;
					gameObject.Timer = 0;
				}
			}
			if(gameObject.State == (uint)State.THROWN)
			{
				gameObject.currentBoundingBox.Position += new PxPosition(0, 2).ToSubpx();
			}
		}
		public void Interact(GameObject own, GameObject other)
		{

		}
	}

	class MermaidLevel1EndCutscene : IBehaviour
	{
		public static readonly MermaidLevel1EndCutscene Instance = new();

		private enum State
		{
			GOING_DOWN,
			STOPPED,
			CHARGING,
			THROWING
		}

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.REGION;
			gameObject.isPersistentAcrossChunks = false;
			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
			gameObject.currentBoundingBox.Size = new TileSize(2, 2).ToPx().ToSubpx();
			gameObject.atlasReference.Start = new PxPosition(256, 512);
			gameObject.atlasReference.Size = new PxSize(32, 32);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);

			ref uint animationTimer = ref gameObject.Timer;
			ref uint actionTimer = ref gameObject.Timer2;
			animationTimer = 0;
			actionTimer = 0;

			gameObject.State = (uint)State.GOING_DOWN;
		}

		public void Update(GameObject gameObject)
		{
			ref uint animationTimer = ref gameObject.Timer;
			ref uint actionTimer = ref gameObject.Timer2;

			switch((State)gameObject.State)
			{
				case State.GOING_DOWN:
					animationTimer++;
					gameObject.atlasReference.Start = ((animationTimer >> 3) & 0x03) switch
					{
						0 => new PxPosition(256 + 32, 512),
						2 => new PxPosition(256 + 64, 512),
						_ => new PxPosition(256, 512),
					};

					actionTimer++;
					if(actionTimer >= 30*5)
					{
						actionTimer = 0;
						gameObject.State = (uint)State.STOPPED;
						GameObject.signalFlags!.EmitGameplayMessage(GameplayMessages.TriggerLevel1EndCutscene);
					}
					else
					{
						gameObject.currentBoundingBox.Position.Y += (uint)Configuration.Px.Subpx.Height / 2;
					}
					break;
				case State.STOPPED:
					animationTimer++;
					gameObject.atlasReference.Start = ((animationTimer >> 3) & 0x03) switch
					{
						0 => new PxPosition(256 + 32, 512),
						2 => new PxPosition(256 + 64, 512),
						_ => new PxPosition(256, 512),
					};
					actionTimer++;
					if(actionTimer >= 60)
					{
						actionTimer = 0;
						gameObject.State = (uint)State.CHARGING;
					}
					break;
				case State.CHARGING:
					actionTimer++;
					if(actionTimer < 30 || actionTimer > 90)
					{
						gameObject.atlasReference.Start = new PxPosition(256, 544);
					}
					else if((actionTimer & 0x02) == 0x02)
					{
						gameObject.atlasReference.Start = new PxPosition(256, 544);
					}
					else
					{
						gameObject.atlasReference.Start = new PxPosition(256 + 32, 544);
					}
					if(actionTimer == 60)
					{
						// create trident
						GameObject.signalFlags!.CreateGameObject(
							TridentLevel1EndCutscene.Instance,
							0,
							gameObject.currentBoundingBox.Position + new PxPosition(1, 3).ToSubpx()
						);
					}


					if(actionTimer >= 120)
					{
						actionTimer = 0;
						gameObject.State = (uint)State.THROWING;
					}
					break;
				case State.THROWING:
					actionTimer++;
					if(actionTimer < 6)
					{
						gameObject.atlasReference.Start = new PxPosition(256, 576);
					}
					else
					{
						gameObject.atlasReference.Start = new PxPosition(256 + 32, 576);
					}
					break;

			}
			
		}

		public void Interact(GameObject own, GameObject other)
		{

		}
	}

}
