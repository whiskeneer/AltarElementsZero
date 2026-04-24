using AltarElementsZero.src.states.gameplay.gameObject.behaviour;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.enemies;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks;
using AltarElementsZero.src.states.gameplay.level;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject
{

    sealed class GameObject
    {

        public GameObject? linkedObject = null;
        public SubpxPosition linkedPosition = new();

        public GameObject? secondLinkedObject = null;
        public SubpxPosition secondLinkedPosition = new();

        public void LinkWith(GameObject gameObject){
            linkedObject = gameObject; // ORA will use as a scythe
        }
        public void SecondLinkWith(GameObject gameObject){
            secondLinkedObject = gameObject; // ORA will use as a movable box
        }
        public enum DrawOrderTypes : Byte{
            NONE,
            BACK,
            MIDDLE,
            FRONT
        }
        public DrawOrderTypes drawOrder = DrawOrderTypes.NONE;


        public static ISignalFlags? signalFlags = null;

        // Delete later, only for testing
        public static InputHandler? inputHandler = null;

        // For new physics implementation
        public SubpxVelocity previousVelocity = new();
        public SubpxVelocity currentVelocity = new();

        public ObjectBoundingBox previousBoundingBox = new();
        public ObjectBoundingBox currentBoundingBox = new();

        public bool PushedUp = false;
        public bool PushedDown = false;
        public bool PushedLeft = false;
        public bool PushedRight = false;

        public bool PushedPreviouslyUp = false;
        public bool PushedPreviouslyDown = false;
        public bool PushedPreviouslyLeft = false;
        public bool PushedPreviouslyRight = false;

        // Gravity
        public Force Gravity = new(0, 12);
		// Friction with the ground
		public int VelocityBelow;
        public Tile.FrictionCoefficients FrictionCoefficientsBelow;
        // Friction with the medium
        public SubpxVelocity VelocityAround;
        public uint FrictionCoefficientAround;

        // Self impulse
        public int GroundImpulse;
        public SubpxVelocity AirImpulse;

        public Force AppliedForces;

        // For fluids
        public SubpxVelocity FluidVelocity;
        public uint FluidCoefficient;

        //////////////////////////////////////////////////

        public void SimulateRegularObjectPhysics()
        {
			currentVelocity = previousVelocity;

			ApplyAirImpulse();
			ApplyMediumFriction();
            AppliedForces += Gravity; // gravity

			Force ForcesBeforeGroundFriction = AppliedForces;

			TransformForcesIntoVelocity();

			if (PushedPreviouslyUp)
			{
				ApplyGroundImpulse(ForcesBeforeGroundFriction.Y);
				TransformForcesIntoVelocity();
			}

			CapDesiredVelocity();
		}

        
        public void ApplyGroundImpulse(int pushingForce)
        {
            if (pushingForce <= 0) return;

            int currentNetVelocity = previousVelocity.X - VelocityBelow - GroundImpulse;
            int targetNetVelocity = currentVelocity.X - VelocityBelow - GroundImpulse;

            if(currentNetVelocity == 0)
            {// STATIC FRICTION
                int staticFriction = Math.Min(
                    Math.Abs(targetNetVelocity),
                    (FrictionCoefficientsBelow.StaticMu * pushingForce) >> 8
                    );
                AppliedForces += new Force(
                    staticFriction * -Math.Sign(targetNetVelocity),
                    0
                    );
            }
            else
            {// KINEMATIC FRICTION
				int kinematicFriction = Math.Min(
	                Math.Abs(targetNetVelocity),
	                (FrictionCoefficientsBelow.KinematicMu * pushingForce) >> 8
	                );
				AppliedForces += new Force(
					kinematicFriction * -Math.Sign(targetNetVelocity),
					0
					);
			}
        }

        public void ApplyAirImpulse()
        {
            SubpxVelocity netTargetVelocity = AirImpulse - VelocityAround;
            SubpxVelocity remainingVelocity = netTargetVelocity - currentVelocity;
            AppliedForces += new Force(
                Math.Sign(remainingVelocity.X) * (Math.Abs(remainingVelocity.X) >> 5),
                Math.Sign(remainingVelocity.Y) * (Math.Abs(remainingVelocity.Y) >> 5)
                );
        }

        public void ApplyMediumFriction()
        {
            SubpxVelocity netVelocity = currentVelocity - VelocityAround;
            AppliedForces += new Force(
                -Math.Sign(netVelocity.X) * ((netVelocity.X * netVelocity.X * (int)FrictionCoefficientAround) >> 16),
                -Math.Sign(netVelocity.Y) * ((netVelocity.Y * netVelocity.Y * (int)FrictionCoefficientAround) >> 16)
				);
        }

        public void TransformForcesIntoVelocity()
        {
            currentVelocity += AppliedForces;
            AppliedForces = new();
        }

        public void CapDesiredVelocity()
        {
            if(currentVelocity.X > Configuration.Tile.Subpx.Width)
            {
                currentVelocity.X = Configuration.Tile.Subpx.Width;
            }
            if(currentVelocity.X < -Configuration.Tile.Subpx.Width)
            {
                currentVelocity.X = -Configuration.Tile.Subpx.Width;
            }

            if(currentVelocity.Y > Configuration.Tile.Subpx.Height)
            {
                currentVelocity.Y = Configuration.Tile.Subpx.Height;
            }
            if(currentVelocity.Y < -Configuration.Tile.Subpx.Height)
            {
                currentVelocity.Y = -Configuration.Tile.Subpx.Height;
            }

        }

        //////////////////////////////////////////////////

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

            InteractionFlags = 0; // flags were used on Update, now they are cleaned
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
				Interaction(go1, go2);

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
				Interaction(go1, go2);

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
                    go1.FrictionCoefficientsBelow = new(400, 200);
                    go1.VelocityBelow = go2.currentVelocity.X;
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
					go2.FrictionCoefficientsBelow = new(400, 200);
					go2.VelocityBelow = go1.currentVelocity.X;
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
                pushee.currentBoundingBox.LeanAtRight(pusher.currentBoundingBox, (uint)Math.Abs(pusher.currentVelocity.X - pushee.currentVelocity.X));
                pushee.PushedRight = true;
            }
            else
            {
				pushee.currentBoundingBox.LeanAtLeft(pusher.currentBoundingBox, (uint)Math.Abs(pusher.currentVelocity.X - pushee.currentVelocity.X));
                pushee.PushedLeft = true;
            }
            pushee.FixHorizontalVelocity();
        }

        public static ObjectBoundingBox.SeparationDirection VerticalPush(GameObject pusher, GameObject pushee)
        {
            if(pusher.currentVelocity.Y > pushee.currentVelocity.Y)
            {
                pushee.currentBoundingBox.LeanBelow(pusher.currentBoundingBox, (uint)Math.Abs(pusher.currentVelocity.Y - pushee.currentVelocity.Y));
                pushee.PushedDown = true;
				pushee.FixVerticalVelocity();
				return ObjectBoundingBox.SeparationDirection.DOWN;
            }
            else
            {
                pushee.currentBoundingBox.LeanAbove(pusher.currentBoundingBox, (uint)Math.Abs(pusher.currentVelocity.Y - pushee.currentVelocity.Y));
                pushee.PushedUp = true;
                pushee.FixVerticalVelocity();

                pushee.FrictionCoefficientsBelow = new(400, 200);
                pushee.VelocityBelow = pusher.currentVelocity.X;

                return ObjectBoundingBox.SeparationDirection.UP;
            }
        }

        public static void HorizontalSeparation(GameObject go1, GameObject go2)
        {
            ObjectBoundingBox.SeparateHorizontally(ref go1.currentBoundingBox, ref go2.currentBoundingBox, (uint)Math.Abs(go1.currentVelocity.X - go2.currentVelocity.X) + 1 );
            go1.FixHorizontalVelocity();
            go2.FixHorizontalVelocity();

            //Console.WriteLine($"HS! GO1: {go1.currentBoundingBox.Position.X} - GO2: {go2.currentBoundingBox.Position.X}");

        }
        public static void VerticalSeparation(GameObject go1, GameObject go2)
        {
			ObjectBoundingBox.SeparateVertically(ref go1.currentBoundingBox, ref go2.currentBoundingBox, (uint)Math.Abs(go1.currentVelocity.Y - go2.currentVelocity.Y) + 1);
			go1.FixVerticalVelocity();
			go2.FixVerticalVelocity();

			//Console.WriteLine($"VS! GO1: {go1.currentBoundingBox.Position.Y} - GO2: {go2.currentBoundingBox.Position.Y}");

		}

        public static void Separation(GameObject go1, GameObject go2)
        {
			ObjectBoundingBox.SeparationDirection direction = ObjectBoundingBox.Separate(ref go1.currentBoundingBox, ref go2.currentBoundingBox,
				(uint)Math.Abs(go1.currentVelocity.X - go2.currentVelocity.X) + 1,
				(uint)Math.Abs(go1.currentVelocity.Y - go2.currentVelocity.Y) + 1);
            switch (direction)
            {
                case ObjectBoundingBox.SeparationDirection.UP:
                    go1.FrictionCoefficientsBelow = new(400, 200);
                    go1.VelocityBelow = go2.currentVelocity.X;
                    go1.PushedUp = true;
					break;
                case ObjectBoundingBox.SeparationDirection.DOWN:
					go2.FrictionCoefficientsBelow = new(400, 200);
					go2.VelocityBelow = go1.currentVelocity.X;
					go2.PushedUp = true;
					break;
				default: break;
            }
            go1.FixVelocity();
            go2.FixVelocity();
        }
        public void SeparationFrom(ObjectBoundingBox other)
        {
            ObjectBoundingBox.SeparationDirection direction = currentBoundingBox.SeparateFrom(other);
            switch (direction)
            {
                case ObjectBoundingBox.SeparationDirection.UP:    PushedUp = true; break;
				case ObjectBoundingBox.SeparationDirection.DOWN: PushedDown = true; break;
				case ObjectBoundingBox.SeparationDirection.LEFT: PushedLeft = true; break;
				case ObjectBoundingBox.SeparationDirection.RIGHT: PushedRight = true; break;
                default: break;

			}
			FixVelocity();
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



        public enum Types : byte
        {
            NONEXISTENT, // empty slot
            IMMOBILE,    // similar to ground tiles
            UNSTOPPABLE, // self-moving platforms
            PUSHABLE,    // target of physical actions
            FLUID,      // region with friction and current
            REGION, // Like fluid, but without physical properties
            RESERVED,  // Like nonexistent, but not available
            PROJECTILE,  // Dissapears on contact with solid objects
            SPAWNING,   // reserved to be created on the next frame
        };

        public Types Type { get; set; }


        public IBehaviour behaviour = EmptyObject.Instance;
        public byte spawnValue = 0;
        public UInt32 InteractionFlags = 0;


        public static void Interaction(GameObject go1, GameObject go2)
        {
			go1.linkedObject?.currentBoundingBox.Position = go1.currentBoundingBox.Position + go1.linkedPosition;
			go1.secondLinkedObject?.currentBoundingBox.Position = go1.currentBoundingBox.Position + go1.secondLinkedPosition;

			go2.linkedObject?.currentBoundingBox.Position = go2.currentBoundingBox.Position + go2.linkedPosition;
			go2.secondLinkedObject?.currentBoundingBox.Position = go2.currentBoundingBox.Position + go2.secondLinkedPosition;

			if (go1.linkedObject != null && go1.linkedObject.currentBoundingBox & go2.currentBoundingBox)
            {
                if(go2.linkedObject != null && go2.linkedObject.currentBoundingBox & go1.linkedObject.currentBoundingBox)
                {
					go1.linkedObject.behaviour.Interact(go1.linkedObject, go2.linkedObject);
					go2.linkedObject.behaviour.Interact(go2.linkedObject, go1.linkedObject);

					go1.linkedObject.behaviour.Interact(go1.linkedObject, go2);
					go2.behaviour.Interact(go2, go1.linkedObject);

					go1.behaviour.Interact(go1, go2.linkedObject);
					go2.linkedObject.behaviour.Interact(go2.linkedObject, go1);

					go1.behaviour.Interact(go1, go2);
					go2.behaviour.Interact(go2, go1);

				}
                else
                {
					go1.linkedObject.behaviour.Interact(go1.linkedObject, go2);
					go2.behaviour.Interact(go2, go1.linkedObject);

					go1.behaviour.Interact(go1, go2);
					go2.behaviour.Interact(go2, go1);
				}
            }
            else
            {
				if (go2.linkedObject != null && go2.linkedObject.currentBoundingBox & go1.currentBoundingBox)
				{
					go1.behaviour.Interact(go1, go2.linkedObject);
					go2.linkedObject.behaviour.Interact(go2.linkedObject, go1);

					go1.behaviour.Interact(go1, go2);
					go2.behaviour.Interact(go2, go1);
				}
				else
				{
					go1.behaviour.Interact(go1, go2);
					go2.behaviour.Interact(go2, go1);
				}
			}

        }


        public bool isPersistentAcrossChunks = false;

        //public bool isVisible = false;
        public uint spritesheetIndex = 0;
        public SpriteEffects spriteEffects = SpriteEffects.None;
        public PxSize SpriteOffset;

        public void Delete()
        {
            behaviour = EmptyObject.Instance;
            behaviour.Init(this);
        }


        public void Init()
        {
            behaviour.Init(this);
        }

        public uint State = 0;
        public uint SubState = 0;
        public uint Timer = 0;
        public uint Timer2 = 0;

    }
}
