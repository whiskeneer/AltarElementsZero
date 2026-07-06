using AltarElementsZero.src.states.editor;
using AltarElementsZero.src.states.gameplay;
using AltarElementsZero.src.states.gameplay.vectors;
using AltarElementsZero.src.states.inputConfig;
using AltarElementsZero.src.states.intro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.menu
{
	class Menu(
		GraphicsDevice graphicsDevice,
		GameServiceContainer gameServiceContainer,
		IManager manager,
		MenuPayload payload,
		GlobalAssets globalAssets,
		InputHandler inputHandler
		) : State <MenuAssets, MenuPayload>(
			manager: manager,
			payload: payload,
			assets: new MenuAssets(graphicsDevice, gameServiceContainer),
			inputHandler: inputHandler,
			globalAssets: globalAssets
			)
	{

		public override void Enter()
		{
			base.Enter();
		}

		private readonly int BackgroundSpeedX = 1;
		private readonly int BackgroundSpeedY = -1;
		private int BackgroundPositionX = 0;
		private int BackgroundPositionY = 0;
		private int CursorTimer = 0;

		private enum MainList : int
		{
			PLAY,
			OPTIONS,
			EXIT,
			LENGTH
		}
		private enum OptionsList : int
		{
			INPUT,
			EDITOR,
			RECORD,
			BACK,
			LENGTH
		}
		private enum Tab : int
		{
			MAIN,
			OPTIONS
		}

		private int CurrentTab = (int)Tab.MAIN;
		private int CurrentItem = (int)MainList.PLAY;

		private int CurrentPositionX = -192;
		private int CurrentPositionY = 0;
		private void TargetPosition(out int targetPositionX, out int targetPositionY)
		{
			targetPositionX = CurrentItem * 16 + (CurrentTab == (int)Tab.OPTIONS ? 192 + 16: 0);
			targetPositionY = (CurrentItem + (CurrentTab == (int)Tab.OPTIONS ? 1 : 0)) * 32;
			//return new((uint)CurrentItem * 16, (uint)CurrentItem * 32);
		}

		private static void Lerp(int fromX, int fromY, int toX, int toY, out int outX, out int outY)
		{
			outX = ((toX - fromX)>>2) + fromX;
			outY = ((toY - fromY)>>2) + fromY;
		}

		

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			CursorTimer++;

			// update animation
			BackgroundPositionX += BackgroundSpeedX;
			BackgroundPositionY += BackgroundSpeedY;

			if (BackgroundPositionX > 0) BackgroundPositionX -= 192;
			if (BackgroundPositionX <= -192) BackgroundPositionX += 192;

			if (BackgroundPositionY > 0) BackgroundPositionY -= 128;
			if (BackgroundPositionY <= -128) BackgroundPositionY += 128;

			int listLength = 1;
			if (CurrentTab == (int)Tab.MAIN) listLength = (int)MainList.LENGTH;
			else if (CurrentTab == (int)Tab.OPTIONS) listLength = (int)OptionsList.LENGTH;

			if(_inputHandler.IsPressed(Input.Up))
			{
				CurrentItem--;
				if(CurrentItem < 0) CurrentItem += listLength;
			}
			if(_inputHandler.IsPressed(Input.Down))
			{
				CurrentItem++;
				if(CurrentItem >= listLength) CurrentItem -= listLength; 
			}
			if(_inputHandler.IsPressed(Input.Jump) || _inputHandler.IsPressed(Input.Pause))
			{
				if(CurrentTab == (int)Tab.MAIN)
				{
					if(CurrentItem == (int)MainList.OPTIONS)
					{
						CurrentTab = (int)Tab.OPTIONS;
						CurrentItem = (int)OptionsList.INPUT;
					}
					else if(CurrentItem == (int)MainList.EXIT)
					{
						_manager.RequestTransition(new IntroPayload("HELLO"));
					}
					else if(CurrentItem == (int)MainList.PLAY)
					{
						_manager.RequestTransition(new GameplayPayload(GameplayPayload.GameplayConfiguration.NORMAL_GAMEPLAY));
					}
				}
				else if(CurrentTab == (int)Tab.OPTIONS)
				{
					if(CurrentItem == (int)OptionsList.BACK)
					{
						CurrentTab = (int)Tab.MAIN;
						CurrentItem = (int)MainList.PLAY;
					}
					else if(CurrentItem == (int)OptionsList.EDITOR)
					{
						_manager.RequestTransition(new EditorPayload());
					}
					else if(CurrentItem == (int)OptionsList.RECORD)
					{
						_manager.RequestTransition(new GameplayPayload(GameplayPayload.GameplayConfiguration.RECORD_AUTOPLAY));
					}
					else if (CurrentItem == (int)OptionsList.INPUT)
					{
						//_inputHandler.SaveKeyboardSettings();	
						_manager.RequestTransition(new InputConfigPayload());

					}
				}
			}

			TargetPosition(out int targetPositionX, out int targetPositionY);
			Lerp(CurrentPositionX, CurrentPositionY, targetPositionX, targetPositionY, out int lerpPositionX, out int lerpPositionY);
			CurrentPositionX = lerpPositionX;
			CurrentPositionY = lerpPositionY;


		}




		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			Rectangle backgroundSourceRectangle = new(384, 256, 192, 128);

			spriteBatch.Draw(
				_assets.Atlas,
				new Vector2(BackgroundPositionX, BackgroundPositionY),
				backgroundSourceRectangle,
				Color.White
				);
			spriteBatch.Draw(
				_assets.Atlas,
				new Vector2(BackgroundPositionX + 192, BackgroundPositionY),
				backgroundSourceRectangle,
				Color.White
				);
			spriteBatch.Draw(
				_assets.Atlas,
				new Vector2(BackgroundPositionX, BackgroundPositionY + 128),
				backgroundSourceRectangle,
				Color.White
				);
			spriteBatch.Draw(
				_assets.Atlas,
				new Vector2(BackgroundPositionX + 192, BackgroundPositionY + 128),
				backgroundSourceRectangle,
				Color.White
				);
			//

			//if(CurrentTab == (int)Tab.MAIN)
			//{
			Rectangle itemPlayRectangle = new(768, 256, 64, 32);
			Rectangle itemOptionsRectangle = new(896, 256, 128, 32);
			Rectangle itemExitRectangle = new(832, 256, 64, 32);
			Rectangle cursorRectangle = new(656, 352, 16, 32);

			Rectangle itemInputRectangle = new(767, 288, 81, 32);
			Rectangle itemEditorRectangle = new(768, 320, 112, 32);
			Rectangle itemRecordRectangle = new(880, 320, 116, 32);
			Rectangle itemBackRectangle = new(848, 288, 80, 32);

			int referenceX = 32;
			int referenceY = 16+32;

			spriteBatch.Draw(
				_assets.Atlas,
				new Vector2(16*1 - CurrentPositionX + referenceX, 32*0 - CurrentPositionY + referenceY),
				itemPlayRectangle,
				Color.White
				);
			spriteBatch.Draw(
				_assets.Atlas,
				new Vector2(16*2 - CurrentPositionX + referenceX, 32*1 - CurrentPositionY + referenceY),
				itemOptionsRectangle,
				Color.White
				);
			spriteBatch.Draw(
				_assets.Atlas,
				new Vector2(16*3 - CurrentPositionX + referenceX, 32*2 - CurrentPositionY + referenceY),
				itemExitRectangle,
				Color.White
				);

			spriteBatch.Draw(
				_assets.Atlas,
				new Vector2(16 * 2 + 192 - CurrentPositionX + referenceX, 32 * 1 - CurrentPositionY + referenceY),
				itemInputRectangle,
				Color.White
				);
			spriteBatch.Draw(
				_assets.Atlas,
				new Vector2(16 * 3 + 192 - CurrentPositionX + referenceX, 32 * 2 - CurrentPositionY + referenceY),
				itemEditorRectangle,
				Color.White
				);
			spriteBatch.Draw(
				_assets.Atlas,
				new Vector2(16 * 4 + 192 - CurrentPositionX + referenceX, 32 * 3 - CurrentPositionY + referenceY),
				itemRecordRectangle,
				Color.White
				);
			spriteBatch.Draw(
				_assets.Atlas,
				new Vector2(16 * 5 + 192 - CurrentPositionX + referenceX, 32 * 4 - CurrentPositionY + referenceY),
				itemBackRectangle,
				Color.White
				);

			if ((CursorTimer & 0x10) == 0x10)
			{
				spriteBatch.Draw(
					_assets.Atlas,
					new Vector2(referenceX, referenceY),
					cursorRectangle,
					Color.White
					);
			}
			//}
			//else if(CurrentTab == (int)Tab.OPTIONS)
			//{

			//}

		}
		public override void Exit()
		{
			// if allocating on Enter, dispose here
			base.Exit();
		}
	}
}
