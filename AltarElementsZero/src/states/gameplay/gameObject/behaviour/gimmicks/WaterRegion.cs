using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{
	class WaterRegion : IBehaviour
	{
		public static readonly WaterRegion Instance = new();

		public void Init(GameObject gameObject)
		{

			uint height = 1 + (((uint)gameObject.spawnValue >> 4) & 0xf);
			uint width = 1 + ((uint)gameObject.spawnValue & 0xf);

			gameObject.Type = GameObject.Types.FLUID;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.FRONT;
			gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x3d);
			gameObject.atlasReference.Size = new PxSize(16, 16);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);
			gameObject.atlasReference.RepeatX = (byte)(width - 1);
			gameObject.atlasReference.RepeatY = (byte)(height - 1);



			gameObject.currentBoundingBox.Size = new TileSize(width,height).ToPx().ToSubpx();

			gameObject.FluidVelocity = new SubpxVelocity();
			gameObject.FluidCoefficient = Configuration.WaterFriction;
			gameObject.FluidGravity = Configuration.WaterGravity;
		}

		public void Update(GameObject gameObject)
		{
			ref uint animationTimer = ref gameObject.Timer;

			animationTimer++;
			if ((animationTimer & 0x1) == 0x1)
			{
				gameObject.atlasReference.Effects = SpriteEffects.None;
			}
			else
			{
				gameObject.atlasReference.Effects = SpriteEffects.FlipHorizontally;
			}
		}

		public void Interact(GameObject own, GameObject other)
		{

		}

	}
}
