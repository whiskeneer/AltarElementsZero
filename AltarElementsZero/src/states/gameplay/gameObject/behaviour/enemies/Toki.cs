using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.enemies
{
    class Toki : IBehaviour
    {
        public static readonly Toki Instance = new();

        enum State : uint
        {
            GOING_LEFT,
            GOING_RIGHT,
            ATTACKING_LEFT,
            ATTACKING_RIGHT,
            SHIFTING_TO_LEFT,
            SHIFTING_TO_RIGHT
        }

        public void Init(GameObject gameObject)
        {
            gameObject.exists = true;
            gameObject.isSolid = true;
            gameObject.hurtsPlayer = true;
            gameObject.isFixed = false;
            gameObject.isAffectedByGravity = true;

            gameObject.isVisible = true;
            gameObject.spritesheetIndex = 0;
            gameObject.spriteEffects = SpriteEffects.None;


			gameObject.Size = new PxSize(12, 12).ToSubpx();
			gameObject.SpriteOffset = new PxSize(10, 20);

			if(gameObject.spawnValue == 0)
			{
				gameObject.State = (uint)State.GOING_LEFT;
				gameObject.Timer = 60 * 4;
			}
			else
			{
				gameObject.State = (uint)State.GOING_RIGHT;
				gameObject.Timer = 60 * 4;
			}
		}

        public void Update(GameObject gameObject)
        {
            if (--gameObject.Timer == 0)
            {
                switch ((State)gameObject.State)
                {
                    case State.GOING_LEFT:
                        gameObject.State = (uint)State.SHIFTING_TO_RIGHT;
                        gameObject.Timer = 24;
                        break;
                    case State.GOING_RIGHT:
						gameObject.State = (uint)State.SHIFTING_TO_LEFT;
						gameObject.Timer = 24;
						break;
                    case State.SHIFTING_TO_LEFT:
						gameObject.State = (uint)State.GOING_LEFT;
						gameObject.Timer = 60*4;
						break;
                    case State.SHIFTING_TO_RIGHT:
						gameObject.State = (uint)State.GOING_RIGHT;
						gameObject.Timer = 60 * 4;
						break;
				}
            }

			switch ((State)gameObject.State)
			{
				case State.GOING_LEFT:
					gameObject.FeetVelocity = new(-16, 0);
                    gameObject.spritesheetIndex = (gameObject.Timer>>4)&3;
                    gameObject.spriteEffects = SpriteEffects.None;
					break;
				case State.GOING_RIGHT:
					gameObject.FeetVelocity = new(16, 0);
					gameObject.spritesheetIndex = (gameObject.Timer >> 4) & 3;
					gameObject.spriteEffects = SpriteEffects.FlipHorizontally;
					break;
                case State.SHIFTING_TO_RIGHT:
					gameObject.FeetVelocity = new(0, 0);
                    if(gameObject.Timer > 16)
                    {
						gameObject.spritesheetIndex = 4;
						gameObject.spriteEffects = SpriteEffects.None;
					}
                    else if(gameObject.Timer > 8)
                    {
						gameObject.spritesheetIndex = 5;
						gameObject.spriteEffects = SpriteEffects.None;
					}
                    else
                    {
						gameObject.spritesheetIndex = 4;
						gameObject.spriteEffects = SpriteEffects.FlipHorizontally;
					}

					break;
                case State.SHIFTING_TO_LEFT:
					gameObject.FeetVelocity = new(0, 0);
					if (gameObject.Timer > 16)
					{
						gameObject.spritesheetIndex = 4;
						gameObject.spriteEffects = SpriteEffects.FlipHorizontally;
					}
					else if (gameObject.Timer > 8)
					{
						gameObject.spritesheetIndex = 5;
						gameObject.spriteEffects = SpriteEffects.None;
					}
					else
					{
						gameObject.spritesheetIndex = 4;
						gameObject.spriteEffects = SpriteEffects.None;
					}
					break;
				default:
					gameObject.FeetVelocity = new(0, 0);
					break;
			}
		}

	}
}
