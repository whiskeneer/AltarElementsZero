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
			LEVEL1END,

			CUTSCENES_LENGTH,
			NONE = CUTSCENES_LENGTH
		}
		private CutsceneID CurrentPlayingCutscene = CutsceneID.NONE;
		private int CutsceneDialogIndex = -1;
		
		private readonly Cutscene[] Cutscenes = new Cutscene[(int)CutsceneID.CUTSCENES_LENGTH]{
			new Cutscene(
				new Dialogue[]{
					//new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "alguien\naprovechó\nmientras\ndormía para"),
					//new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "robar mis\nPODERES DEL\nTIEMPO."),
					//new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "ya se las\nverá conmigo"),
					new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "somebody\nstole my\nTIME POWERS\nwhile I was\nsleeping"),
					new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "they'll pay\nfor this")

				}
			),
			new Cutscene(
				new Dialogue[]{
					//new Dialogue(Dialogue.Characters.Mermaid, Dialogue.SpeakerPosition.Right, " EY!!!"),
					//new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "holaquetal"),
					//new Dialogue(Dialogue.Characters.Mermaid, Dialogue.SpeakerPosition.Right, " ESTOY MUY\n ENOJADA\n CONTIGO!!!"),
					//new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "no serías\nla primera"),
					//new Dialogue(Dialogue.Characters.Mermaid, Dialogue.SpeakerPosition.Right, " PAGARÁS POR\n LO QUE\n HICISTE"),
					//new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "bueno"),
					new Dialogue(Dialogue.Characters.Mermaid, Dialogue.SpeakerPosition.Right, " HEY!!!"),
					new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "what's up"),
					new Dialogue(Dialogue.Characters.Mermaid, Dialogue.SpeakerPosition.Right, " I AM VERY\n ANGRY WITH\n YOU!!!"),
					new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "wouldn't be\nthe first\none"),
					new Dialogue(Dialogue.Characters.Mermaid, Dialogue.SpeakerPosition.Right, " YOU'LL PAY\n FOR WHAT\n YOU DID"),
					new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "k"),
				}
			),
			new Cutscene(
				new Dialogue[]{
					//new Dialogue(Dialogue.Characters.MermaidHurt, Dialogue.SpeakerPosition.Right, " POR QUÉ ME\n HACES ESTO\n ?!?!?!"),
					//new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "vos\nempezaste"),
					//new Dialogue(Dialogue.Characters.MermaidHurt, Dialogue.SpeakerPosition.Right, " NO ES CIERTO\n !!!"),
					//new Dialogue(Dialogue.Characters.MermaidHurt, Dialogue.SpeakerPosition.Right, " VOS ROBASTE\n MI MICRÓFONO\n !!!"),
					//new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "..."),
					//new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "no"),
					//new Dialogue(Dialogue.Characters.MermaidCrying, Dialogue.SpeakerPosition.Right, " CLARO QUE\n SI!"),
					//new Dialogue(Dialogue.Characters.MermaidCrying, Dialogue.SpeakerPosition.Right, " Y si no\n fuiste vos,\n entonces fue\n una de tus\n secuaces."),
					//new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "Mis Tokis?\n(los \n\"cerditos\")"),
					//new Dialogue(Dialogue.Characters.MermaidCrying, Dialogue.SpeakerPosition.Right, " Eh? No!\n Fue una\n chica con\n traje de\n idol"),
					//new Dialogue(Dialogue.Characters.MermaidCrying, Dialogue.SpeakerPosition.Right, " Se escapó\n usando tus\n PODERES DEL\n TIEMPO"),
					//new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "interesante\n..."),

					//new Dialogue(Dialogue.Characters.MermaidCrying, Dialogue.SpeakerPosition.Right, " ..."),
					//new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "..."),
					//new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "buenochau"),
					//new Dialogue(Dialogue.Characters.MermaidHurt, Dialogue.SpeakerPosition.Right, " EY!!!\n ESPERÁ!!!"),
					new Dialogue(Dialogue.Characters.MermaidHurt, Dialogue.SpeakerPosition.Right, " WHY ARE YOU\n DOING THIS\n TO ME?!?!?!"),
					new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "you started\nit"),
					new Dialogue(Dialogue.Characters.MermaidHurt, Dialogue.SpeakerPosition.Right, " THAT'S NOT\n TRUE!!!"),
					new Dialogue(Dialogue.Characters.MermaidHurt, Dialogue.SpeakerPosition.Right, " YOU STOLE MY\n MICROPHONE\n !!!"),
					new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "..."),
					new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "no"),
					new Dialogue(Dialogue.Characters.MermaidCrying, Dialogue.SpeakerPosition.Right, " OF COURSE\n YOU DID!"),
					new Dialogue(Dialogue.Characters.MermaidCrying, Dialogue.SpeakerPosition.Right, " And if it\n wasn't you,\n then it was\n one of your\n minions."),
					new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "My Tokis?\n(the \"little\npigs\")"),
					new Dialogue(Dialogue.Characters.MermaidCrying, Dialogue.SpeakerPosition.Right, " Huh? No!\n It was a\n girl in an\n idol outfit"),
					new Dialogue(Dialogue.Characters.MermaidCrying, Dialogue.SpeakerPosition.Right, " She got\n away using\n your TIME\n POWERS"),
					new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "interesting\n..."),

					new Dialogue(Dialogue.Characters.MermaidCrying, Dialogue.SpeakerPosition.Right, " ..."),
					new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "..."),
					new Dialogue(Dialogue.Characters.Ora, Dialogue.SpeakerPosition.Left, "kthxbye"),
					new Dialogue(Dialogue.Characters.MermaidHurt, Dialogue.SpeakerPosition.Right, " HEY!!!\n WAIT!!!"),
				}
			),
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
				var speakerRectangle = dialogue.Speaker switch
				{
					Dialogue.Characters.Ora => new Rectangle(0, 640, 64, 128),
					Dialogue.Characters.Mermaid => new Rectangle(64, 640, 96, 128),
					Dialogue.Characters.MermaidHurt => new Rectangle(160, 640, 96, 128),
					Dialogue.Characters.MermaidCrying => new Rectangle(256, 640, 96, 128),
					_ => new Rectangle(0, 0, 0, 0),
				};
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
