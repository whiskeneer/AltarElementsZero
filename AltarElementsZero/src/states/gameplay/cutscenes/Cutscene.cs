namespace AltarElementsZero.src.states.gameplay.cutscenes
{
	struct Cutscene(Dialogue[] dialogues)
	{
		public readonly Dialogue[] Dialogues = dialogues;
		public bool Played = false;
	}
}
