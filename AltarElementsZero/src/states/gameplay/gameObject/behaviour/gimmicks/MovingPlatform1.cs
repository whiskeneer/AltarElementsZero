using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{
    class MovingPlatform1 : IBehaviour
    {
        public static readonly MovingPlatform1 Instance = new ();

        enum State: uint
        {
            GOING_LEFT,
            WAIT1,
            GOING_UP,
            WAIT2,
            GOING_RIGHT,
            WAIT3,
            GOING_DOWN,
            WAIT4,
        }

        private static void StartMovingLeft(GameObject gameObject)
        {
			gameObject.State = (uint)State.GOING_LEFT;
			gameObject.Velocity = new(-64, 0);
			gameObject.Timer = 16 * 4;
		}
        private static void StartMovingUp(GameObject gameObject)
        {
			gameObject.State = (uint)State.GOING_UP;
			gameObject.Velocity = new(0, -64);
			gameObject.Timer = 16 * 2;
		}
		private static void StartMovingRight(GameObject gameObject)
		{
			gameObject.State = (uint)State.GOING_RIGHT;
			gameObject.Velocity = new(64, 0);
			gameObject.Timer = 16 * 4;
		}
		private static void StartMovingDown(GameObject gameObject)
		{
			gameObject.State = (uint)State.GOING_DOWN;
			gameObject.Velocity = new(0, 64);
			gameObject.Timer = 16 * 2;
		}

		public void Init(GameObject gameObject)
        {
            gameObject.exists = true;
            gameObject.isSolid = true;
            gameObject.hurtsPlayer = false;
            gameObject.isFixed = false;
            gameObject.isAffectedByGravity = false;

            gameObject.isVisible = true;
            gameObject.spritesheetIndex = 8;
            gameObject.SpriteOffset = new(0, 16);
            gameObject.spriteEffects = SpriteEffects.None;

			gameObject.Size = new PxSize(32, 16).ToSubpx();
			gameObject.SpriteOffset = new PxSize(0, 16);

			gameObject.isSelfMoving = true;

            StartMovingLeft(gameObject);
        }

        public void Update(GameObject gameObject)
        {
            if(--gameObject.Timer == 0)
            {
                switch ((State)gameObject.State)
                {
                    case State.GOING_LEFT:
                        gameObject.Velocity = new(0, 0);
						gameObject.State = (uint)State.WAIT1;
                        gameObject.Timer = 60;
                        break;
                    case State.WAIT1:
                        StartMovingUp(gameObject);
                        break;
                    case State.GOING_UP:
						gameObject.Velocity = new(0, 0);
						gameObject.State = (uint)State.WAIT2;
						gameObject.Timer = 60;
						break;
					case State.WAIT2:
                        StartMovingRight(gameObject);
						break;
					case State.GOING_RIGHT:
                        gameObject.Velocity = new(0, 0);
						gameObject.State = (uint)State.WAIT3;
						gameObject.Timer = 60;
						break;
					case State.WAIT3:
                        StartMovingDown(gameObject);
						break;
					case State.GOING_DOWN:
                        gameObject.Velocity = new(0, 0);
						gameObject.State = (uint)State.WAIT4;
						gameObject.Timer = 60;
						break;
					case State.WAIT4:
                        StartMovingLeft(gameObject);
						break;

					default:
                        break;
                }
            }
        }
    }
}
