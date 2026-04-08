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
			gameObject.State = (uint)State.GOING_LEFT;
			gameObject.Timer = 60 * 4;
		}

        public void Update(GameObject gameObject)
        {
            switch ((State)gameObject.State)
            {
                case State.GOING_LEFT:
                    gameObject.FeetVelocity = new(-16,0);
                    break;
                case State.GOING_RIGHT:
					gameObject.FeetVelocity = new(16, 0);
					break;
                default:
                    gameObject.FeetVelocity = new(0, 0);
					break;
			}
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
        }

        public bool Exists(GameObject gameObject)
        {
            return true; 
        }
        public bool IsSolid(GameObject gameObject)
        {
            return true;
        }
        public bool HurtsPlayer(GameObject gameObject)
        {
            return true;
        }
        public bool IsFixed(GameObject gameObject)
        {
            return false;
        }
        public bool IsAffectedByGravity(GameObject gameObject)
        {
            return true;
        }
        public bool IsVisible(GameObject gameObject)
        {
            return true;
        }
		public uint GetSpritesheetIndex(GameObject gameObject)
        {
            switch ((State)gameObject.State)
            {
                case State.GOING_LEFT:
                    return 0;
                case State.GOING_RIGHT:
                    return 0;
                case State.ATTACKING_LEFT:
                    return 6;
                case State.ATTACKING_RIGHT:
                    return 6;
				case State.SHIFTING_TO_LEFT:
                    return 5;
                case State.SHIFTING_TO_RIGHT:
					return 5;
				default:
                    return 0;
            }

        }
        public SpriteEffects GetSpriteEffects(GameObject gameObject)
        {
            switch ((State)gameObject.State)
            {
                case State.GOING_LEFT:
                    return SpriteEffects.None;
                case State.GOING_RIGHT:
                    return SpriteEffects.FlipHorizontally;
                case State.ATTACKING_LEFT:
                    return SpriteEffects.None;
                case State.ATTACKING_RIGHT:
                    return SpriteEffects.FlipHorizontally;
                case State.SHIFTING_TO_RIGHT:
                    return SpriteEffects.None;
                case State.SHIFTING_TO_LEFT:
                    return SpriteEffects.FlipHorizontally;
                default:
                    return SpriteEffects.None;
            }

        }


	}
}
