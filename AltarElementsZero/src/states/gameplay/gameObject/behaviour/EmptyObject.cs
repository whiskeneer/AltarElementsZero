namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour
{
    class EmptyObject : IBehaviour
    {
        public static readonly EmptyObject Instance = new();

        public void Init(GameObject gameObject)
        {
            //gameObject.exists = false;
            gameObject.isVisible = false;
            return;
        }
        public void Update(GameObject gameObject)
        {
            return;
        }


	}
}
