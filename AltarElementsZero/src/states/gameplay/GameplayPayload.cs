namespace AltarElementsZero.src.states.gameplay
{
    sealed class GameplayPayload(GameplayPayload.GameplayConfiguration gameplayConfiguration) : Payload
    {
        public enum GameplayConfiguration
        {
            NORMAL_GAMEPLAY,
            RECORD_AUTOPLAY,
            PLAY_AUTOPLAY
        }

        public readonly GameplayConfiguration Configuration = gameplayConfiguration;
    }
}
