using AltarElementsZero.src.states.gameplay.gameObject.behaviour.debug;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.effects;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.enemies;
using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.player
{
    class Ora : IBehaviour
    {
        [Flags]
		public enum FlagTypes : UInt32
        {
            None = 0,
            Hurt = 1 << 0,
        }

        public static readonly Ora Instance = new();

        private enum State : uint
        {
            LOOKING_RIGHT,
            LOOKING_LEFT
        }

        private const int GROUND_IMPULSE = 64;
        private const int AIR_IMPULSE = 128;
        private const uint JUMP_TIME = 12;
        private const int JUMP_FORCE = 175;
        private const int DOWN_ATTACK_IMPULSE = 100;
        private const int BOUNCE_IMPULSE = 290;

        public void Init(GameObject gameObject)
        {
            gameObject.Type = GameObject.Types.PUSHABLE;
			gameObject.isPersistentAcrossChunks = true;


			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
            gameObject.spritesheetIndex = 0x00;
            gameObject.SpriteOffset = new(10, 6);
            gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

            gameObject.currentBoundingBox.Size = new PxSize(11, 24).ToSubpx();

            gameObject.State = (uint)State.LOOKING_RIGHT;


        }

        public void Update(GameObject gameObject)
        {
            InputHandler inputHandler = GameObject.inputHandler!;
            GameObject? scythe = gameObject.linkedObject;
            if (scythe == null) return;

            ref uint jumpTimer = ref gameObject.Timer;
            ref uint animationTimer = ref gameObject.Timer2;

            //

            if(inputHandler.IsPressed(Input.Pause)){
                GameObject.signalFlags!.EmitGameplayMessage(GameplayMessages.Exit);
            }

            if (((FlagTypes)gameObject.InteractionFlags & FlagTypes.Hurt) == FlagTypes.Hurt)
            {
                if (gameObject.secondLinkedObject != null)
                {
                    gameObject.secondLinkedObject.Type = GameObject.Types.PUSHABLE;
                    gameObject.secondLinkedObject.isPersistentAcrossChunks = false;
                    gameObject.secondLinkedObject = null;
                }

				//GameObject.signalFlags!.EmitGameplayMessage(GameplayMessages.RestartFromCheckpoint);
				SubpxPosition center = gameObject.currentBoundingBox.Center();
				gameObject.Delete();
				gameObject.behaviour = OraDefeated.Instance;
				gameObject.Init();
				gameObject.currentBoundingBox.Position = center;
                return;
			}
            


			// PHYSICS

			if (inputHandler.IsDown(Input.Left))
            {
                gameObject.AirImpulse = new(-AIR_IMPULSE, 0);
                gameObject.GroundImpulse = -GROUND_IMPULSE;

                gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
				gameObject.SpriteOffset = new(11, 6);
                gameObject.State = (uint)State.LOOKING_LEFT;




			}
			else if (inputHandler.IsDown(Input.Right))
            {
				gameObject.AirImpulse = new(AIR_IMPULSE, 0);
                gameObject.GroundImpulse = GROUND_IMPULSE;

                gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
				gameObject.SpriteOffset = new(10, 6);
				gameObject.State = (uint)State.LOOKING_RIGHT;




			}
			else
            {
				gameObject.AirImpulse = new(0, 0);
				gameObject.GroundImpulse = 0;
            }

            if ((gameObject.PushedUp || gameObject.PushedPreviouslyUp) && inputHandler.IsPressed(Input.Jump))
            {
                jumpTimer = JUMP_TIME;
				gameObject.AppliedForces += new Force(0, -12 -JUMP_FORCE);
			}
            else if(jumpTimer > 0)
            {
                if (inputHandler.IsReleased(Input.Jump))
                {
                    jumpTimer = 0;
                }
                else
                {
                    jumpTimer--;
                    gameObject.AppliedForces += new Force(0, -12);
                }

            }

			// SCYTHE SYNCHRONIZATION


			if (inputHandler.IsPressed(Input.Attack))
			{
				if (scythe.State == (uint)Scythe.State.INACTIVE && gameObject.secondLinkedObject == null)
				{
					if (inputHandler.IsDown(Input.Down) && !(gameObject.PushedUp || gameObject.PushedPreviouslyUp))
					{
						if (gameObject.previousVelocity.Y < 0)
						{
							gameObject.previousVelocity.Y = 0;
						}
						gameObject.AppliedForces += new Force(0, DOWN_ATTACK_IMPULSE);
						scythe.State = (uint)Scythe.State.ACTIVE_DOWN;
						jumpTimer = 0;
					}
					else
					{
						scythe.State = (uint)Scythe.State.ACTIVE;
					}

				}
			}

            if((scythe.InteractionFlags & (UInt32)Scythe.FlagTypes.Bounce) == (UInt32)Scythe.FlagTypes.Bounce)
            {
                if(gameObject.previousVelocity.Y > 0)
                {
                    gameObject.previousVelocity.Y = 0;
                }
                gameObject.AppliedForces += new Force(0, -BOUNCE_IMPULSE);
            }

			//scythe.SpriteOffset = gameObject.SpriteOffset;

			if (scythe.State == (uint)Scythe.State.ACTIVE)
			{
                if(gameObject.State == (uint)State.LOOKING_LEFT)
                {
				    gameObject.linkedPosition = new PxPosition((uint)((-11 - 16 + 10) & 0xffffffff), -6 + 13).ToSubpx();
				    scythe.SpriteOffset = new(10, 13);
				    scythe.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                }
                else
                {
					gameObject.linkedPosition = new PxPosition((uint)((-10 - 16 + 14) & 0xffffffff), -6 + 13).ToSubpx();
					scythe.SpriteOffset = new(14, 13);
					scythe.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
				}
			}
            else if(scythe.State == (uint)Scythe.State.ACTIVE_DOWN)
            {
				if (gameObject.State == (uint)State.LOOKING_LEFT)
				{
					gameObject.linkedPosition = new PxPosition((uint)((-11 - 16 + 25) & 0xffffffff), -6 + 19).ToSubpx();
					scythe.SpriteOffset = new(25, 19);
					scythe.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
				}
				else
				{
					gameObject.linkedPosition = new PxPosition((uint)((-10 - 16 + 23) & 0xffffffff), -6 + 19).ToSubpx();
					scythe.SpriteOffset = new(23, 19);
					scythe.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
				}
			}

            if(gameObject.secondLinkedObject != null){
				scythe.State = (uint)Scythe.State.COOLDOWN;
				scythe.Timer = 1; // cooldownTimer
			}

            // MOVING BOX SYNCRONIZATION

            if(gameObject.secondLinkedObject != null){

				if ((State)gameObject.State == State.LOOKING_LEFT)
				{
					gameObject.secondLinkedPosition = new PxPosition((uint)(-16 & 0xffffffff), (uint)(-7 & 0xffffffff)).ToSubpx();
				}
				else
				{
					gameObject.secondLinkedPosition = new PxPosition((uint)(11 & 0xffffffff), (uint)(-7 & 0xffffffff)).ToSubpx();
				}

				if (inputHandler.IsPressed(Input.Attack))
				{
					gameObject.secondLinkedObject.Type = GameObject.Types.PUSHABLE;
					gameObject.secondLinkedObject.isPersistentAcrossChunks = false;
                    gameObject.secondLinkedObject.currentVelocity = gameObject.currentVelocity;
					if ((State)gameObject.State == State.LOOKING_LEFT){
                        gameObject.secondLinkedObject.AppliedForces += new Force(-100,-100);
					}
					else{
						gameObject.secondLinkedObject.AppliedForces += new Force(100, -100);
					}
					gameObject.secondLinkedObject = null;
				}

			}


			// GRAPHICS

			if (gameObject.PushedUp || gameObject.PushedPreviouslyUp) // on ground
            {
                if(scythe.State == (uint)Scythe.State.ACTIVE)
                {
					ref uint attackTimer = ref scythe.Timer2;
					if (attackTimer > 6){
                        gameObject.spritesheetIndex = 0x18;
					}
					else
					{
						gameObject.spritesheetIndex = 0x19;
					}
				}
                else if(scythe.State == (uint)Scythe.State.ACTIVE_DOWN)
                {
                    gameObject.spritesheetIndex = 0x13;
                }
                else
                {
                    if (inputHandler.IsDown(Input.Left))
                    {
                        gameObject.spritesheetIndex = 0x08 + ((animationTimer>>3) & 0x3);
                        animationTimer++;
                    }
                    else if (inputHandler.IsDown(Input.Right))
                    {
					    gameObject.spritesheetIndex = 0x08 + ((animationTimer >> 3) & 0x3);
					    animationTimer++;
                    }
                    else
                    {
                        animationTimer = 0;
                        gameObject.spritesheetIndex = 0x2;
                    }

                    if (gameObject.secondLinkedObject != null) gameObject.spritesheetIndex += 0x4;
                }


            }
            else // on air
            {
                if (scythe.State == (uint)Scythe.State.ACTIVE)
                {
					ref uint attackTimer = ref scythe.Timer2;
					if (attackTimer > 6)
					{
						gameObject.spritesheetIndex = 0x1A;
					}
					else
					{
						gameObject.spritesheetIndex = 0x1B;
					}
				}
                else if (scythe.State == (uint)Scythe.State.ACTIVE_DOWN)
                {
                    gameObject.spritesheetIndex = 0x13;
                }
				else
                { 
				    if (jumpTimer > 0)
                    {
                        animationTimer = 0;
                        gameObject.spritesheetIndex = 0x10;
                    }
                    else
                    {
                        if(animationTimer < 15)
                        {
						    gameObject.spritesheetIndex = 0x11;
						    animationTimer++;
                        }
                        else
                        {
						    gameObject.spritesheetIndex = 0x12;
					    }
                    }

					if (gameObject.secondLinkedObject != null) gameObject.spritesheetIndex += 0x4;

				}
			}


            gameObject.SimulateRegularObjectPhysics();

            //


        }
		public void Interact(GameObject own, GameObject other)
		{
            IBehaviour otherBehaviour = other.behaviour;
            // Can this be optimized?
            // (I could use extra GameObject bools but I'm starting
            //  to worry about the potential size of the objectPool)
            if(otherBehaviour == Arrow.Instance)
            {   
                own.InteractionFlags |= (UInt32) FlagTypes.Hurt;
            }

            if (otherBehaviour == Toki.Instance && 
                ((Toki.FlagTypes)other.InteractionFlags & Toki.FlagTypes.Hurt) == 0)
            {
                own.InteractionFlags |= (UInt32) FlagTypes.Hurt;
			}
			if (otherBehaviour == Ufo.Instance &&
			    ((Ufo.FlagTypes)other.InteractionFlags & Ufo.FlagTypes.Hurt) == 0)
            {
                own.InteractionFlags |= (UInt32) FlagTypes.Hurt;
			}

			if (own.secondLinkedObject == null && 
                otherBehaviour == DebugBox.Instance &&
                GameObject.inputHandler!.IsPressed(Input.Attack))
            {
                ObjectBoundingBox checkingBoundingBox = own.currentBoundingBox;

                if((State)own.State == State.LOOKING_LEFT){
                    checkingBoundingBox.Size.Y -= 8 * 64;
                    checkingBoundingBox.Position.Y += 4 * 64;
                    checkingBoundingBox.Position.X -= 4 * 64;
                }else{
					checkingBoundingBox.Size.Y -= 8 * 64;
					checkingBoundingBox.Position.Y += 4 * 64;
					checkingBoundingBox.Position.X += 4 * 64;
				}

                if(checkingBoundingBox & other.currentBoundingBox){
                    other.Type = GameObject.Types.RESERVED;
                    own.secondLinkedObject = other;
                    other.isPersistentAcrossChunks = true;

                }

            }

		}

	}
}
