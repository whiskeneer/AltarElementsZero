using AltarElementsZero.src.states.gameplay.gameObject.behaviour.effects;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.player;
using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{
	class BreakableTile : IBehaviour
	{
		[Flags]
		public enum FlagTypes : UInt32
		{
			None = 0,
			Broken = 1 << 0,
		}

		public static readonly BreakableTile Instance = new ();

		public void Init(GameObject gameObject){
			gameObject.Type = GameObject.Types.IMMOBILE;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
			gameObject.spritesheetIndex = 0x0e;
			gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

			gameObject.currentBoundingBox.Size = new PxSize(16, 16).ToSubpx();
			gameObject.SpriteOffset = new();

		}

		public void Update(GameObject gameObject){
			if(((FlagTypes)gameObject.InteractionFlags & FlagTypes.Broken) == FlagTypes.Broken)
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
			if(otherBehaviour == Scythe.Instance)
			{
				own.InteractionFlags |= (UInt32)FlagTypes.Broken;
			}
		}

	}
}
