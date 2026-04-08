namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.enemies
{
    class Toki : IBehaviour
    {
        public static readonly Toki Instance = new();

        public bool Exists(GameObject gameObject)
        {
            return true; 
        }
        public bool IsSolid(GameObject gameObject)
        {
            return true;
        }
        public bool HurtsPlayer(GameObject gameObject)
        {
            return true;
        }
        public bool IsFixed(GameObject gameObject)
        {
            return false;
        }
        public bool IsAffectedByGravity(GameObject gameObject)
        {
            return true;
        }
        public bool IsVisible(GameObject gameObject)
        {
            return true;
        }

    }
}
