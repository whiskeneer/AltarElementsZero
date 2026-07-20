using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.cutscenes
{
	class CutsceneManager(
		InputHandler inputHandler
	)
	{
		InputHandler _inputHandler = inputHandler;
		public enum CutsceneID
		{
			LEVEL1START,
			LEVEL1BOSS,

			CUTSCENES_LENGTH,
			NONE = CUTSCENES_LENGTH
		}
		private CutsceneID CurrentPlayingCutscene = CutsceneID.NONE;
		private int CutsceneDialogIndex = -1;
		
		private readonly Cutscene[] Cutscenes = new Cutscene[(int)CutsceneID.CUTSCENES_LENGTH]{
			new Cutscene(
				new Dialogue[]{
					new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "hola"),
					new Dialogue(Dialogue.Characters.Mermaid, Dialogue.SpeakerPosition.Right, "hola"),
					new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "adios"),
					new Dialogue(Dialogue.Characters.Mermaid, Dialogue.SpeakerPosition.Right, "adios")
				}
			),
			new Cutscene()
		};
		public void ResetCutscenes()
		{
			for (int i = 0; i < Cutscenes.Length; i++)
			{
				ref Cutscene cutscene = ref Cutscenes[i];
				cutscene.Played = false;
			}
		}
		public bool IsPlayingACutscene()
		{
			return CurrentPlayingCutscene != CutsceneID.NONE;
		}
		public void StartCutscene(CutsceneID id)
		{
			if (IsPlayingACutscene()) return;

			if(id < CutsceneID.CUTSCENES_LENGTH && Cutscenes[(int)id].Played == false)
			{
				CurrentPlayingCutscene = id;
				Cutscenes[(int)id].Played = true;
				CutsceneDialogIndex = 0;
			}
		}
		public void Update()
		{
			if (!IsPlayingACutscene()) return;

			if(_inputHandler.IsPressed(Input.Jump))
			{
				CutsceneDialogIndex++;
				if(CutsceneDialogIndex >= Cutscenes[(int)CurrentPlayingCutscene].Dialogues.Length)
				{
					CurrentPlayingCutscene = CutsceneID.NONE;
					CutsceneDialogIndex = -1;
				}
			}
		}
		public void Draw(SpriteBatch spriteBatch, Texture2D atlas)
		{
			if (!IsPlayingACutscene()) return;
			Dialogue dialogue = Cutscenes[(int)CurrentPlayingCutscene].Dialogues[CutsceneDialogIndex];

			if (dialogue.Speaker != Dialogue.Characters.None)
			{
				Rectangle speakerRectangle;
				switch (dialogue.Speaker)
				{
					case Dialogue.Characters.Ora:
						speakerRectangle = new Rectangle(0, 640, 64, 128);
						break;
					case Dialogue.Characters.Mermaid:
						speakerRectangle = new Rectangle(64, 640, 96, 128);
						break;
					default:
						speakerRectangle = new Rectangle(0, 0, 0, 0);
						break;
				}

				Vector2 speakerPosition;
				SpriteEffects mirrorSpeaker;
				PxPosition textPosition;
				if(dialogue.Position == Dialogue.SpeakerPosition.Right)
				{
					mirrorSpeaker = SpriteEffects.FlipHorizontally;
					speakerPosition = new(192 - speakerRectangle.Width, 0);
					textPosition = new(0, 16*5);
				}
				else
				{
					mirrorSpeaker = SpriteEffects.None;
					speakerPosition = new(0, 0);
					textPosition = new(16*6, 16 * 5);
				}

				for(int j = 5; j < 8; j++)
				{
					for(int i = 0; i < 12; i++)
					{
						spriteBatch.Draw(
							texture:atlas,
							position:new Vector2(16*i, 16*j),
							sourceRectangle: new Rectangle(0,0,16,16),
							color: Color.White
						);
					}
				}

				spriteBatch.Draw(
					texture: atlas,
					position: speakerPosition,
					sourceRectangle: speakerRectangle,
					color: Color.White,
					0f, Vector2.Zero, 1f,
					mirrorSpeaker,
					0f
				);


				Renderer.RenderText(
					spriteBatch,
					textPosition,
					atlas,
					dialogue.Speach,
					Renderer.Fonts.FONT8X8
					);
			}
		}

	}
}
