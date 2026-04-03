namespace AltarElementsZero.src.states.intro
{
    sealed class IntroPayload(string debugText) : Payload
    {
        public readonly string DebugText = debugText;
    }
}
