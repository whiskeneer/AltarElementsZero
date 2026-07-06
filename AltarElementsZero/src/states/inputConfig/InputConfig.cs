using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using AltarElementsZero.src.states.menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.inputConfig
{
	internal class InputConfig(
		GraphicsDevice graphicsDevice,
		GameServiceContainer gameServiceContainer,
		IManager manager,
		InputConfigPayload payload,
		GlobalAssets globalAssets,
		InputHandler inputHandler
		) : State<InputConfigAssets, InputConfigPayload>(
			manager: manager,
			payload: payload,
			assets: new InputConfigAssets(graphicsDevice, gameServiceContainer),
			inputHandler: inputHandler,
			globalAssets: globalAssets)
	{

		private int selectionIndex = 0;
		private enum State
		{
			NOT_SETTING,
			SETTING_KEYBOARD,
			SETTING_GAMEPAD
		}
		private State state = State.NOT_SETTING;

		public override void Enter()
		{
			base.Enter();
			state = State.NOT_SETTING;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (state == State.NOT_SETTING)
			{
				if (_inputHandler.IsPressed(Input.Up))
				{
					selectionIndex--;
					if (selectionIndex < 0) selectionIndex = 2;
				}
				if (_inputHandler.IsPressed(Input.Down))
				{
					selectionIndex++;
					if (selectionIndex > 2) selectionIndex = 0;
				}

				if (_inputHandler.IsPressed(Input.Jump))
				{
					switch (selectionIndex)
					{
						case 0:
							state = State.SETTING_KEYBOARD;
							_inputHandler.SetKeyboard();
							break;
						case 1:
							state = State.SETTING_GAMEPAD;
							_inputHandler.SetGamepad();
							break;
						default:
							_manager.RequestTransition(new MenuPayload());
							break;
					}
				}

			}
			else if (state == State.SETTING_KEYBOARD)
			{
				if (_inputHandler.IsSettingKeyboard() == false)
				{
					state = State.NOT_SETTING;
				}
			}
			else if (state == State.SETTING_GAMEPAD)
			{
				if(_inputHandler.IsSettingGamepad() == false)
				{
					state = State.NOT_SETTING;
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			
			for(int j = 0; j < 8; j++)
			{
				for (int i = 0; i < 12; i++)
				{
					spriteBatch.Draw(
						_assets.Atlas,
						new Vector2(i*16, j*16),
						new Rectangle(768,768,16,16),
						Color.White
					);
				}
			}

			if(state == State.NOT_SETTING)
			{
				Renderer.RenderText(
					spriteBatch,
					new PxPosition(16, 0),
					_assets.Atlas!,
					"SET KEYB.");
				Renderer.RenderText(
					spriteBatch,
					new PxPosition(16, 16),
					_assets.Atlas!,
					"SET GAMEPAD");
				Renderer.RenderText(
					spriteBatch,
					new PxPosition(16, 32),
					_assets.Atlas!,
					"BACK");
				Renderer.RenderText(
					spriteBatch,
					new PxPosition(0, 16 * (uint)selectionIndex),
					_assets.Atlas!,
					">");

				if(selectionIndex != 2)
				{
					spriteBatch.Draw(
						_assets.Atlas,
						new Vector2(0 * 16, 4 * 16),
						new Rectangle(784, 768, 16, 16),
						Color.White
					);
					spriteBatch.Draw(
						_assets.Atlas,
						new Vector2(0 * 16, 5 * 16),
						new Rectangle(800, 768, 16, 16),
						Color.White
					);
					spriteBatch.Draw(
						_assets.Atlas,
						new Vector2(0 * 16, 6 * 16),
						new Rectangle(816, 768, 16, 16),
						Color.White
					);
					spriteBatch.Draw(
						_assets.Atlas,
						new Vector2(0 * 16, 7 * 16),
						new Rectangle(832, 768, 16, 16),
						Color.White
					);
					spriteBatch.Draw(
						_assets.Atlas,
						new Vector2(6 * 16, 4 * 16),
						new Rectangle(784 + 64, 768, 16, 16),
						Color.White
					);
					spriteBatch.Draw(
						_assets.Atlas,
						new Vector2(6 * 16, 5 * 16),
						new Rectangle(800 + 64, 768, 16, 16),
						Color.White
					);
					spriteBatch.Draw(
						_assets.Atlas,
						new Vector2(6 * 16, 6 * 16),
						new Rectangle(816 + 64, 768, 16, 16),
						Color.White
					);
					spriteBatch.Draw(
						_assets.Atlas,
						new Vector2(6 * 16, 7 * 16),
						new Rectangle(832 + 64, 768, 16, 16),
						Color.White
					);
				}
				if(selectionIndex == 0)
				{
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(1 * 16, 4 * 16),
						_assets.Atlas!,
						_inputHandler.GetKeyboardKeyFor(Input.Up),
						5
					);
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(1 * 16, 5 * 16),
						_assets.Atlas!,
						_inputHandler.GetKeyboardKeyFor(Input.Down),
						5
					);
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(1 * 16, 6 * 16),
						_assets.Atlas!,
						_inputHandler.GetKeyboardKeyFor(Input.Left),
						5
					);
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(1 * 16, 7 * 16),
						_assets.Atlas!,
						_inputHandler.GetKeyboardKeyFor(Input.Right),
						5
					);
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(7 * 16, 4 * 16),
						_assets.Atlas!,
						_inputHandler.GetKeyboardKeyFor(Input.Jump),
						5
					);
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(7 * 16, 5 * 16),
						_assets.Atlas!,
						_inputHandler.GetKeyboardKeyFor(Input.Attack),
						5
					);
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(7 * 16, 6 * 16),
						_assets.Atlas!,
						_inputHandler.GetKeyboardKeyFor(Input.Dash),
						5
					);
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(7 * 16, 7 * 16),
						_assets.Atlas!,
						_inputHandler.GetKeyboardKeyFor(Input.Pause),
						5
					);
				} 
				else if (selectionIndex == 1)
				{
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(1 * 16, 4 * 16),
						_assets.Atlas!,
						_inputHandler.GetGamepadButtonFor(Input.Up),
						5
						);
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(1 * 16, 5 * 16),
						_assets.Atlas!,
						_inputHandler.GetGamepadButtonFor(Input.Down),
						5
					);
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(1 * 16, 6 * 16),
						_assets.Atlas!,
						_inputHandler.GetGamepadButtonFor(Input.Left),
						5
					);
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(1 * 16, 7 * 16),
						_assets.Atlas!,
						_inputHandler.GetGamepadButtonFor(Input.Right),
						5
					);
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(7 * 16, 4 * 16),
						_assets.Atlas!,
						_inputHandler.GetGamepadButtonFor(Input.Jump),
						5
					);
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(7 * 16, 5 * 16),
						_assets.Atlas!,
						_inputHandler.GetGamepadButtonFor(Input.Attack),
						5
					);
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(7 * 16, 6 * 16),
						_assets.Atlas!,
						_inputHandler.GetGamepadButtonFor(Input.Dash),
						5
					);
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(7 * 16, 7 * 16),
						_assets.Atlas!,
						_inputHandler.GetGamepadButtonFor(Input.Pause),
						5
					);
				}
			} 
			else if(state == State.SETTING_KEYBOARD || state == State.SETTING_GAMEPAD)
			{
				Renderer.RenderText(
					spriteBatch,
					new PxPosition(0, 0),
					_assets.Atlas!,
					"SETTING");
				if(state == State.SETTING_KEYBOARD)
				{
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(0, 16),
						_assets.Atlas!,
						"KEYBOARD.");
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(0, 32),
						_assets.Atlas!,
						"PRESS KEY");
				}
				else
				{
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(0, 16),
						_assets.Atlas!,
						"GAMEPAD.");
					Renderer.RenderText(
						spriteBatch,
						new PxPosition(0, 32),
						_assets.Atlas!,
						"PRESS BUTTON");
				}


				Renderer.RenderText(
					spriteBatch,
					new PxPosition(0, 48),
					_assets.Atlas!,
					"FOR...");

				switch(_inputHandler.CurrentlySetting())
				{
					case Input.Up:
						Renderer.RenderText(
							spriteBatch,
							new PxPosition(16, 80),
							_assets.Atlas!,
							"GOING UP");
						break;
					case Input.Down:
						Renderer.RenderText(
							spriteBatch,
							new PxPosition(16, 80),
							_assets.Atlas!,
							"GOING DOWN");
						break;
					case Input.Left:
						Renderer.RenderText(
							spriteBatch,
							new PxPosition(16, 80),
							_assets.Atlas!,
							"GOING LEFT");
						break;
					case Input.Right:
						Renderer.RenderText(
							spriteBatch,
							new PxPosition(16, 80),
							_assets.Atlas!,
							"GOING RIGHT");
						break;
					case Input.Jump:
						Renderer.RenderText(
							spriteBatch,
							new PxPosition(16, 80),
							_assets.Atlas!,
							"JUMPING");
						break;
					case Input.Attack:
						Renderer.RenderText(
							spriteBatch,
							new PxPosition(16, 80),
							_assets.Atlas!,
							"ATTACKING");
						break;
					case Input.Dash:
						Renderer.RenderText(
							spriteBatch,
							new PxPosition(16, 80),
							_assets.Atlas!,
							"DASHING");
						break;
					case Input.Pause:
						Renderer.RenderText(
							spriteBatch,
							new PxPosition(16, 80),
							_assets.Atlas!,
							"PAUSING");
						break;
				}

			}


		}

		public override void Exit()
		{
			// if allocating on Enter, dispose here
			base.Exit();
		}
	}
}
