using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{
	class TurbineCurrentLeft : IBehaviour
	{
		public static readonly TurbineCurrentLeft Instance = new();

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.FLUID;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.NONE;

			gameObject.currentBoundingBox.Size = new TileSize(7, 2).ToPx().ToSubpx();

			gameObject.FluidVelocity = new SubpxVelocity(-64 * 10, 0);
			gameObject.FluidCoefficient = 10;
			
		}
		public void Update(GameObject gameObject)
		{ 
		}
		public void Interact(GameObject own, GameObject other)
		{
		}
	}
}
