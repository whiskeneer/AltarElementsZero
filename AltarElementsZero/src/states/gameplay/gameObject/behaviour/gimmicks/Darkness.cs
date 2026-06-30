namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{
	class Darkness : IBehaviour
	{
		public static readonly Darkness Instance = new();
		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.REGION;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.NONE;

			gameObject.currentBoundingBox.Size = new();
		}
		public void Update(GameObject gameObject)
		{

		}
		public void Interact(GameObject own, GameObject other)
		{

		}

		public static void GetSpan(GameObject gameObject, out uint from, out uint to)
		{
			from = 0;
			to = (uint)(-1 & uint.MaxValue);
			
			if(gameObject.spawnValue != 0)
			{
				uint objectPosition = gameObject.currentBoundingBox.Position.X;
				uint objectTilePosition = objectPosition >> Configuration.Tile.SubpxPower;
				from = objectTilePosition - (uint)((gameObject.spawnValue >> 4)&0xf);
				to = objectTilePosition - 1 + (uint)(gameObject.spawnValue & 0xf);
			}
		}

	}
}
