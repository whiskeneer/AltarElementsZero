using AltarElementsZero.src.states.gameplay.gameObject.behaviour.effects;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.player;
using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.enemies
{
	class Ufo : IBehaviour
	{
		public static readonly Ufo Instance = new ();

		[Flags]
		public enum FlagTypes : UInt32
		{
			None = 0,
			Hurt = 1<<0
		}

		enum State : uint
		{
			FLOATING_LEFT,
			FLOATING_RIGHT,
			CHASING_LEFT,
			CHASING_RIGHT
		}

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.PUSHABLE;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
			gameObject.spritesheetIndex = 0x30;
			gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

			gameObject.currentBoundingBox.Size = new PxSize(12, 12).ToSubpx();
			gameObject.SpriteOffset = new PxSize(10, 10);

			gameObject.State = (uint)State.FLOATING_LEFT;
		}

		public void Update(GameObject gameObject)
		{
			ref uint animationTimer = ref gameObject.Timer;

			// PHYSICS
			
			gameObject.AppliedForces += new Force(0, -12);

			SubpxPosition playerPosition = GameObject.signalFlags!.GetPlayerPosition();
			SubpxPosition ownPosition = gameObject.currentBoundingBox.Position;

			if (SubpxPosition.DistanceSquared(ownPosition, playerPosition) > 5 * 64 * 5 * 64 * 16 * 16)
			{
				// not chasing
				gameObject.AppliedForces += new Force(
					gameObject.currentVelocity.X >> 1,
					gameObject.currentVelocity.Y >> 1
				);
				if ((State)gameObject.State == State.CHASING_RIGHT) gameObject.State = (uint)State.FLOATING_RIGHT;
				if ((State)gameObject.State == State.CHASING_LEFT) gameObject.State = (uint)State.FLOATING_LEFT;  
			}
			else
			{
				// chasing

				if(ownPosition.X > playerPosition.X){
					// going left
					gameObject.State = (uint)State.CHASING_LEFT;
					uint targetForce = (ownPosition.X - playerPosition.X) >> 9;
					gameObject.AppliedForces += new Force(
						-(int)Math.Min(targetForce, 50),
						0
					);

				}else{
					// going right
					gameObject.State = (uint)State.CHASING_RIGHT;
					uint targetForce = (playerPosition.X - ownPosition.X) >> 9;
					gameObject.AppliedForces += new Force(
						(int)Math.Min(targetForce, 50),
						0
					);
				}

				if (ownPosition.Y > playerPosition.Y)
				{
					// going up
					uint targetForce = (ownPosition.Y - playerPosition.Y) >> 9;
					gameObject.AppliedForces += new Force(
						0,
						-(int)Math.Min(targetForce, 50)
					);

				}
				else
				{
					// going down
					uint targetForce = (playerPosition.Y - ownPosition.Y) >> 9;
					gameObject.AppliedForces += new Force(
						0,
						(int)Math.Min(targetForce, 50)
					);
				}

			}
			


			// GRAPHICS

			animationTimer++;
			if((animationTimer&4) == 4){
				gameObject.spritesheetIndex = 0x30;
			}else{
				gameObject.spritesheetIndex = 0x31;
			}

			if ((State)gameObject.State == State.FLOATING_LEFT || 
				(State)gameObject.State == State.CHASING_LEFT)
			{
				gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
			}
			else 
			{
				gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
			}

			//

			gameObject.SimulateRegularObjectPhysics();

			if(((FlagTypes)gameObject.InteractionFlags & FlagTypes.Hurt) == FlagTypes.Hurt)
			{
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
			if(otherBehaviour == Scythe.Instance){
				own.InteractionFlags |= (UInt32)FlagTypes.Hurt;
			}
		}

	}

}
