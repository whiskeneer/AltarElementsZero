using AltarElementsZero.src.states.gameplay.gameObject.behaviour;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.enemies;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject
{

    sealed class GameObject
    {
        // Delete later, only for testing
        public static InputHandler? inputHandler = null;


        // For new physics implementation
        public SubpxVelocity previousVelocity = new();
        public SubpxVelocity currentVelocity = new();

        public BoundingBox previousBoundingBox = new();
        public BoundingBox currentBoundingBox = new();

        public bool PushedUp = false;
        public bool PushedDown = false;
        public bool PushedLeft = false;
        public bool PushedRight = false;

        public bool PushedPreviouslyUp = false;
        public bool PushedPreviouslyDown = false;
        public bool PushedPreviouslyLeft = false;
        public bool PushedPreviouslyRight = false;


        public void CleanHorizontalPushFlags()
        {
            PushedPreviouslyLeft = PushedLeft;
            PushedPreviouslyRight = PushedRight;

            PushedLeft = false;
            PushedRight = false;
        }

        public void CleanVerticalPushFlags()
        {
            PushedPreviouslyUp = PushedUp;
            PushedPreviouslyDown = PushedDown;

            PushedUp = false;
            PushedDown = false;
        }

		public void SavePreviousValues()
        {
            previousBoundingBox = currentBoundingBox;
            previousVelocity = currentVelocity;
        }
        public void CalculateDesiredOutcome()
        {
            behaviour.Update(this); // updates currentVelocity
        }

        public void ApplyHorizontalDesiredVelocity()
        {
            currentBoundingBox += currentVelocity.Horizontal();
        }

        public void ApplyVerticalDesiredVelocity()
        {
            currentBoundingBox += currentVelocity.Vertical();
        }

        public static void CheckHorizontalCollisions(GameObject go1, GameObject go2)
        {
            if (go1.currentBoundingBox & go2.currentBoundingBox)
            {
                switch (go1.Type)
                {
                    case Types.UNSTOPPABLE:
                        switch (go2.Type)
                        {
                            case Types.UNSTOPPABLE:
                                break;
                            case Types.PUSHABLE:
                                HorizontalPush(go1, go2);
                                break;
                            case Types.IMMOBILE:
                                break;
                            default:
                                break;
                        }
                        break;
                    case Types.PUSHABLE:
						switch (go2.Type)
						{
							case Types.UNSTOPPABLE:
								HorizontalPush(go2, go1);
								break;
							case Types.PUSHABLE:
                                HorizontalTie(go1, go2);
								break;
							case Types.IMMOBILE:
								HorizontalPush(go2, go1);
								break;
							default:
								break;
						}
						break;
                    case Types.IMMOBILE:
						switch (go2.Type)
						{
							case Types.UNSTOPPABLE:
								break;
							case Types.PUSHABLE:
								HorizontalPush(go1, go2);
								break;
							case Types.IMMOBILE:
								break;
							default:
								break;
						}
						break;
                    default:
                        break;
                }
            }
        }

		public static void CheckVerticalCollisions(GameObject go1, GameObject go2)
		{
			if (go1.currentBoundingBox & go2.currentBoundingBox)
			{
				switch (go1.Type)
				{
					case Types.UNSTOPPABLE:
						switch (go2.Type)
						{
							case Types.UNSTOPPABLE:
								break;
							case Types.PUSHABLE:
								VerticalPush(go1, go2);
								break;
							case Types.IMMOBILE:
								break;
							default:
								break;
						}
						break;
					case Types.PUSHABLE:
						switch (go2.Type)
						{
							case Types.UNSTOPPABLE:
								VerticalPush(go2, go1);
								break;
							case Types.PUSHABLE:
								VerticalTie(go1, go2);
								break;
							case Types.IMMOBILE:
								VerticalPush(go2, go1);
								break;
							default:
								break;
						}
						break;
					case Types.IMMOBILE:
						switch (go2.Type)
						{
							case Types.UNSTOPPABLE:
								break;
							case Types.PUSHABLE:
								VerticalPush(go1, go2);
								break;
							case Types.IMMOBILE:
								break;
							default:
								break;
						}
						break;
					default:
						break;
				}
			}
		}

		public static void HorizontalTie(GameObject go1, GameObject go2)
        {

            //Console.WriteLine("HORIZONTAL TIE");
            if(go1.currentVelocity.X > go2.currentVelocity.X)
            {// go1 at left of go2
                if ((go1.PushedPreviouslyRight || go1.PushedRight) && !(go2.PushedPreviouslyRight || go2.PushedRight))
                {
                    HorizontalPush(go1, go2);
                }
                else if ((go2.PushedPreviouslyLeft || go2.PushedLeft) && !(go1.PushedPreviouslyLeft || go1.PushedLeft))
                {
                    HorizontalPush(go2, go1);
                }
                else
                {
                    //HorizontalSeparation(go1, go2);
                }


                //else if (!(go2.PushedPreviouslyRight || go2.PushedRight))
                //{
                //    HorizontalPush(go1, go2);
                //}
                //else if (!(go1.PushedPreviouslyLeft || go1.PushedLeft))
                //{
                //    HorizontalPush(go2, go1);
                //}
            }
            else if(go1.currentVelocity.X < go2.currentVelocity.X)
            {// go2 at left of go1
                if ((go1.PushedPreviouslyLeft || go1.PushedLeft) && !(go2.PushedPreviouslyLeft || go2.PushedLeft))
                {
                    HorizontalPush(go1, go2);
                }
                else if ((go2.PushedPreviouslyRight || go2.PushedRight) && !(go1.PushedPreviouslyRight || go1.PushedRight))
                {
                    HorizontalPush(go2, go1);
                }
				else
				{
                    //HorizontalSeparation(go1, go2);
                }


                //else if (!(go2.PushedPreviouslyLeft || go2.PushedLeft))
                //{
                //    HorizontalPush(go1, go2);
                //}
                //else if (!(go1.PushedPreviouslyRight || go1.PushedRight))
                //{
                //    HorizontalPush(go2, go1);
                //}
            }
            else
            {
                //HorizontalSeparation(go1, go2);
            }

        }

        public static void VerticalTie(GameObject go1, GameObject go2)
        {
			//Console.WriteLine("VERTICAL TIE");
			if (go1.currentVelocity.Y > go2.currentVelocity.Y)
            { // go1 above go2
                if((go1.PushedPreviouslyDown || go1.PushedDown) && !(go2.PushedPreviouslyDown || go2.PushedDown))
                {
                    VerticalPush(go1, go2);
                }
                else if((go2.PushedPreviouslyUp || go2.PushedUp) && !(go1.PushedPreviouslyUp || go1.PushedUp))
                {
                    VerticalPush(go2, go1);
                }
				else
				{
                    //VerticalSeparation(go1, go2);
                }

				//else if (!(go2.PushedPreviouslyDown || go2.PushedDown))
				//{
				//    VerticalPush(go1, go2);
				//}
				//else if (!(go1.PushedPreviouslyUp || go1.PushedUp))
				//{
				//    VerticalPush(go2, go1);
				//}

			}
            else if(go1.currentVelocity.Y < go2.currentVelocity.Y)
            { // go2 above go1
                if((go1.PushedPreviouslyUp || go1.PushedUp) && !(go2.PushedPreviouslyUp || go2.PushedUp))
                {
                    VerticalPush(go1, go2);
                }
                else if((go2.PushedPreviouslyDown || go2.PushedDown) && !(go1.PushedPreviouslyDown || go1.PushedDown))
                {
                    VerticalPush(go2, go1);
                }
				else
				{
                    //VerticalSeparation(go1, go2);
                }
				//else if (!(go2.PushedPreviouslyUp || go2.PushedUp))
				//{
				//    VerticalPush(go1, go2);
				//}
				//else if (!(go1.PushedPreviouslyDown || go1.PushedDown))
				//{
				//    VerticalPush(go2, go1);
				//}
			}
            else {
                //VerticalSeparation(go1, go2);
            }

        }

        public static void HorizontalPush(GameObject pusher, GameObject pushee)
        {
            if(pusher.currentVelocity.X > pushee.currentVelocity.X)
            {
                pushee.currentBoundingBox.LeanAtRight(pusher.currentBoundingBox);
                pushee.PushedRight = true;
            }
            else
            {
				pushee.currentBoundingBox.LeanAtLeft(pusher.currentBoundingBox);
                pushee.PushedLeft = true;
            }
            pushee.FixHorizontalVelocity();
        }

        public static void VerticalPush(GameObject pusher, GameObject pushee)
        {
            if(pusher.currentVelocity.Y > pushee.currentVelocity.Y)
            {
                pushee.currentBoundingBox.LeanBelow(pusher.currentBoundingBox);
                pushee.PushedDown = true;
            }
            else
            {
                pushee.currentBoundingBox.LeanAbove(pusher.currentBoundingBox);
                pushee.PushedUp = true;
            }
            pushee.FixVerticalVelocity();
        }

        public static void HorizontalSeparation(GameObject go1, GameObject go2)
        {
            BoundingBox.SeparateHorizontally(ref go1.currentBoundingBox, ref go2.currentBoundingBox, (uint)Math.Abs(go1.currentVelocity.X - go2.currentVelocity.X) + 1 );
            go1.FixHorizontalVelocity();
            go2.FixHorizontalVelocity();

            //Console.WriteLine($"HS! GO1: {go1.currentBoundingBox.Position.X} - GO2: {go2.currentBoundingBox.Position.X}");

        }
        public static void VerticalSeparation(GameObject go1, GameObject go2)
        {
			BoundingBox.SeparateVertically(ref go1.currentBoundingBox, ref go2.currentBoundingBox, (uint)Math.Abs(go1.currentVelocity.Y - go2.currentVelocity.Y) + 1);
			go1.FixVerticalVelocity();
			go2.FixVerticalVelocity();

			//Console.WriteLine($"VS! GO1: {go1.currentBoundingBox.Position.Y} - GO2: {go2.currentBoundingBox.Position.Y}");

		}

        public static void Separation(GameObject go1, GameObject go2)
        {
            BoundingBox.Separate(ref go1.currentBoundingBox, ref go2.currentBoundingBox);
            go1.FixVelocity();
            go2.FixVelocity();
        }
        

		public void FixHorizontalVelocity()
        {
            currentVelocity.X = (int)currentBoundingBox.Position.X - (int)previousBoundingBox.Position.X;
        }

        public void FixVerticalVelocity()
        {
            currentVelocity.Y = (int)currentBoundingBox.Position.Y - (int)previousBoundingBox.Position.Y;
        }

        public void FixVelocity()
        {
            currentVelocity = currentBoundingBox.Position - previousBoundingBox.Position;
        }

        //

        //public bool PushingUp = false;
        //public bool PushingDown = false;
        //public bool PushingLeft = false;
        //public bool PushingRight = false;

        public enum Types : byte
        {
            NONEXISTENT,
            IMMOBILE,
            UNSTOPPABLE,
            PUSHABLE
        };

        public Types Type { get; set; }


        public IBehaviour behaviour = EmptyObject.Instance;
        public byte spawnValue = 0;
     

        public bool isVisible = false;
        public uint spritesheetIndex = 0;
        public SpriteEffects spriteEffects = SpriteEffects.None;
        public PxSize SpriteOffset;


        public void Init()
        {
            behaviour.Init(this);
        }

        public uint State = 0;
        public uint SubState = 0;
        public uint Timer = 0;

        public static GameObject GetToki()
        {
            return new GameObject()
            {
                currentBoundingBox = new BoundingBox(new SubpxPosition(), new PxSize(
					12,
					12
					).ToSubpx()),
				SpriteOffset = new PxSize(10,20),
				behaviour = Toki.Instance,
			};
        }


		public static GameObject GetTestObject()
		{
            GameObject testObject = new()
            {
                currentBoundingBox = new BoundingBox(new SubpxPosition(), new PxSize(
                    (uint)Configuration.Tile.Px.Width,
                    (uint)Configuration.Tile.Px.Height
                    ).ToSubpx()),
            };
            return testObject;
		}

        public static GameObject GetMovingPlatform1()
        {
            GameObject movingPlatform =  new()
            {
				currentBoundingBox = new BoundingBox(new SubpxPosition(), new PxSize(32, 16).ToSubpx()),
                SpriteOffset = new PxSize(0, 16),
                behaviour = MovingPlatform1.Instance,
            };
            movingPlatform.Init();
            return movingPlatform;
        }

    }
}
