namespace AltarElementsZero.src.states.gameplay.cutscenes
{
	readonly struct Dialogue(
		Dialogue.Characters speaker,
		Dialogue.SpeakerPosition position,
		string speach
	)
	{
		public enum Characters
		{
			None,
			Ora,
			Mermaid,
			MermaidHurt,
			MermaidCrying
		}
		public enum SpeakerPosition
		{
			Left,
			Right
		}
		public readonly Characters Speaker = speaker;
		public readonly SpeakerPosition Position = position;
		public readonly string Speach = speach;
	}
}
