using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.player
{
    class Ora : IBehaviour
    {
        public static readonly Ora Instance = new();

        private const int GROUND_IMPULSE = 64;
        private const int AIR_IMPULSE = 128;
        private const uint JUMP_TIME = 12;
        private const int JUMP_FORCE = 175;

        public void Init(GameObject gameObject)
        {
            gameObject.Type = GameObject.Types.PUSHABLE;
			gameObject.isPersistentAcrossChunks = true;


			gameObject.isVisible = true;
            gameObject.spritesheetIndex = 0x00;
            gameObject.SpriteOffset = new(10, 6);
            gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

            gameObject.currentBoundingBox.Size = new PxSize(11, 24).ToSubpx();


        }

        public void Update(GameObject gameObject)
        {
            InputHandler inputHandler = GameObject.inputHandler!;

            ref uint jumpTimer = ref gameObject.Timer;
            ref uint animationTimer = ref gameObject.Timer2;
            
            if (inputHandler.IsDown(Input.Left))
            {
                gameObject.AirImpulse = new(-AIR_IMPULSE, 0);
                gameObject.GroundImpulse = -GROUND_IMPULSE;

                gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
				gameObject.SpriteOffset = new(11, 6);
			}
            else if (inputHandler.IsDown(Input.Right))
            {
				gameObject.AirImpulse = new(AIR_IMPULSE, 0);
                gameObject.GroundImpulse = GROUND_IMPULSE;

                gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
				gameObject.SpriteOffset = new(10, 6);
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

            if (gameObject.PushedUp || gameObject.PushedPreviouslyUp) // on ground
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


            }
            else
            {
                if(jumpTimer > 0)
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
            }


            gameObject.SimulateRegularObjectPhysics();

        }

    }
}
