namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour
{
    class EmptyObject : IBehaviour
    {
        public static readonly EmptyObject Instance = new();

        public bool Exists(GameObject gameObject) 
        {
            return false;
        }
        public bool IsSolid(GameObject gameObject)
        {
            return false;
        }
        public bool HurtsPlayer(GameObject gameObject)
        {
            return false;
        }
        public bool IsFixed(GameObject gameObject)
        {
            // should this be true? It is not going to be checked anyway (since Exists() = false)
            return false;
        }
        public bool IsAffectedByGravity(GameObject gameObject)
        {
            return false;
        }
        public bool IsVisible(GameObject gameObject)
        {
            return false;
        }


    }
}
