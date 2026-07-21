using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.bosses
{
	class Mermaid : IBehaviour
	{
		public static readonly Mermaid Instance = new();

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.REGION;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.BACK;
			gameObject.atlasReference.Start = new PxPosition(256, 512);
			gameObject.atlasReference.Size = new PxSize(32, 32);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);

			gameObject.currentBoundingBox.Size = new PxSize(32, 32).ToSubpx();

		}

		public void Update(GameObject gameObject)
		{

		}

		public void Interact(GameObject own, GameObject other)
		{

		}

	}
}
