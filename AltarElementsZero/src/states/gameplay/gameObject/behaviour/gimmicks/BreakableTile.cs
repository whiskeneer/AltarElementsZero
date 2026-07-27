using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.effects;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.player;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

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

			gameObject.currentBoundingBox.Size = new PxSize(16, 16).ToSubpx();

			//gameObject.SpriteOffset = new();
			//gameObject.spritesheetIndex = 0x0e;
			//gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
			gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x0e);
			gameObject.atlasReference.Size = new PxSize(32, 32);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);

		}

		public void Update(GameObject gameObject){
			if(((FlagTypes)gameObject.InteractionFlags & FlagTypes.Broken) == FlagTypes.Broken)
			{
				SubpxPosition center = gameObject.currentBoundingBox.Center();
				gameObject.Delete();
				gameObject.behaviour = EnemyDefeated.Instance;
				gameObject.Init();
				gameObject.currentBoundingBox.Position = center;

				GameObject.globalAssets!.BreakingWallSFXInstance!.Stop();
				GameObject.globalAssets!.BreakingWallSFXInstance!.Play();
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
