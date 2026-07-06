using System.Text.Json;
using Microsoft.Xna.Framework.Input;

namespace AltarElementsZero.src
{
	public enum InputIndices
	{
		UP, 
		DOWN, 
		LEFT, 
		RIGHT,
		JUMP, 
		ATTACK, 
		DASH, 
		PAUSE,
		FINISHED
	}

	[Flags]
	public enum Input : Byte
	{
		None = 0,

		Up = 1 << InputIndices.UP,
		Down = 1 << InputIndices.DOWN,
		Left = 1 << InputIndices.LEFT,
		Right = 1 << InputIndices.RIGHT,

		Jump = 1 << InputIndices.JUMP,		// Accept
		Attack = 1 << InputIndices.ATTACK,	// Cancel
		Dash = 1 << InputIndices.DASH,
		Pause = 1 << InputIndices.PAUSE,
	}

	public struct InputActions
	{
		public Input IsDown;
		public Input IsPressed;
		public Input IsReleased;
	}

	sealed class InputHandler
	{
		private KeyboardState _previousKeyboardState;
		private KeyboardState _currentKeyboardState;

		private GamePadState _previousGamePadState;
		private GamePadState _currentGamePadState;

		private Keys[] KeyboardSettings = [
			Keys.Up, 
			Keys.Down, 
			Keys.Left, 
			Keys.Right,
			Keys.F,
			Keys.D,
			Keys.S,
			Keys.A
		];
		private Buttons[] GamepadSettings = [
			Buttons.DPadUp,
			Buttons.DPadDown,
			Buttons.DPadLeft,
			Buttons.DPadRight,
			Buttons.A,
			Buttons.X,
			Buttons.RightShoulder,
			Buttons.Start
		];

		public string GetKeyboardKeyFor(Input input)
		{			
			for(int i = 0; i < 8; i++)
			{
				if(((int)input & (1<<i)) == (1<<i))
				{
					return Enum.GetName(KeyboardSettings[i])!;
				}
			}

			return "NONE";
		}
		public string GetGamepadButtonFor(Input input)
		{
			for (int i = 0; i < 8; i++)
			{
				if (((int)input & (1 << i)) == (1 << i))
				{
					return Enum.GetName(GamepadSettings[i])!;
				}
			}

			return "NONE";
		}

		public void SaveKeyboardSettings()
		{
			var json = JsonSerializer.Serialize(KeyboardSettings);
			File.WriteAllText("assets/config/keyboard.json", json);
		}
		public void SaveGamepadSettings()
		{
			var json = JsonSerializer.Serialize(GamepadSettings);
			File.WriteAllText("assets/config/gamepad.json", json);
		}
		public void LoadKeyboardSettings()
		{
			var json = File.ReadAllText("assets/config/keyboard.json");
			KeyboardSettings = JsonSerializer.Deserialize<Keys[]>(json)!;
		}
		public void LoadGamepadSettings()
		{
			var json = File.ReadAllText("assets/config/gamepad.json");
			GamepadSettings = JsonSerializer.Deserialize<Buttons[]>(json)!;
		}


		private Input _previousInput;
		private Input _currentInput;

		// TODO: make sure that on Dispose all of these are disposed properly
		private FileStream? _stream;
		private BinaryWriter? _writer;
		private BinaryReader? _reader;

		private enum State
		{
			REGULAR,
			RECORDING,
			PLAYBACK,
			SETTING_KEYBOARD,
			SETTING_GAMEPAD
		}
		private State state = State.REGULAR;
		//private Input currentlySetting = Input.None;
		private InputIndices currentlySettingIndex = InputIndices.FINISHED;

		public InputActions Actions { get; private set; }

		public bool IsDown(Input input) => (Actions.IsDown & input) == input;
		public bool IsPressed(Input input) => (Actions.IsPressed & input) == input;
		public bool IsReleased(Input input) => (Actions.IsReleased & input) == input;

		public bool StartRecording()
		{
			if (state != State.REGULAR) return false;

			_stream = new FileStream("assets/playback/playback.bin", FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 8192, useAsync: false);
			_writer = new BinaryWriter(_stream);

			state = State.RECORDING;
			return true;
		}
		public bool StopRecording() 
		{ 
			if (state != State.RECORDING) return false;

			_writer!.Flush();
			_writer!.Dispose();
			_writer = null;
			_stream = null;

			state = State.REGULAR;
			return true;
		}
		public bool IsRecording()
		{
			return state == State.RECORDING;
		}
		public bool StartPlayback()
		{
			if (state != State.REGULAR) return false;

			_stream = new FileStream("assets/playback/playback.bin", FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 8192, useAsync: false);
			_reader = new BinaryReader(_stream);

			state = State.PLAYBACK;
			return true;
		}
		public bool StopPlayback()
		{
			if (state != State.PLAYBACK) return false;

			_reader!.Dispose();
			_reader = null;
			_stream = null;

			state = State.REGULAR;
			return true;
		}
		public bool IsPlayingPlayback()
		{
			return state == State.PLAYBACK;
		}

		public bool SetKeyboard()
		{
			if (state != State.REGULAR) return false;
			//currentlySetting = Input.Up;
			currentlySettingIndex = InputIndices.UP;
			state = State.SETTING_KEYBOARD;
			return true;
		}
		public bool SetGamepad()
		{
			if(state != State.REGULAR) return false;

			currentlySettingIndex = InputIndices.UP;
			state = State.SETTING_GAMEPAD;
			return true;
		}
		public bool IsSettingKeyboard()
		{
			return state == State.SETTING_KEYBOARD;
		}
		public bool IsSettingGamepad()
		{
			return state == State.SETTING_GAMEPAD;
		}
		public Input CurrentlySetting()
		{
			//return currentlySetting;
			return (Input)((1 << (int)currentlySettingIndex) & 0xff);
		}

		public void Update()
		{
			_previousInput = _currentInput;

			_previousKeyboardState = _currentKeyboardState;
			_previousGamePadState = _currentGamePadState;

			_currentKeyboardState = Keyboard.GetState();
			_currentGamePadState = GamePad.GetState(0);

			if (state == State.PLAYBACK)
			{
				_currentInput = Input.None;
				if (_currentKeyboardState.IsKeyDown(KeyboardSettings[(int)InputIndices.JUMP]) 
					|| _currentGamePadState.IsButtonDown(GamepadSettings[(int)InputIndices.JUMP])
					|| _currentKeyboardState.IsKeyDown(KeyboardSettings[(int)InputIndices.PAUSE]) 
					|| _currentGamePadState.IsButtonDown(GamepadSettings[(int)InputIndices.PAUSE])
					)
				{
					_currentInput |= Input.Pause;
				}
				if (_stream!.Position >= _stream.Length)
				{
					_currentInput |= Input.Pause;
				}
				else
				{
					_currentInput |= (Input)_reader!.ReadByte();
				}
			}
			else if (state == State.REGULAR || state == State.RECORDING) // REGULAR or RECORDING
			{
				_currentInput = Input.None;
				if (_currentKeyboardState.IsKeyDown(KeyboardSettings[(int)InputIndices.UP]) || 
					_currentGamePadState.IsButtonDown(GamepadSettings[(int)InputIndices.UP]))
				{
					_currentInput |= Input.Up;
				}
				if (_currentKeyboardState.IsKeyDown(KeyboardSettings[(int)InputIndices.DOWN]) ||
					_currentGamePadState.IsButtonDown(GamepadSettings[(int)InputIndices.DOWN]))
				{
					_currentInput |= Input.Down;
				}
				if (_currentKeyboardState.IsKeyDown(KeyboardSettings[(int)InputIndices.LEFT]) ||
					_currentGamePadState.IsButtonDown(GamepadSettings[(int)InputIndices.LEFT]))
				{
					_currentInput |= Input.Left;
				}
				if (_currentKeyboardState.IsKeyDown(KeyboardSettings[(int)InputIndices.RIGHT]) ||
					_currentGamePadState.IsButtonDown(GamepadSettings[(int)InputIndices.RIGHT]))
				{
					_currentInput |= Input.Right;
				}
				if (_currentKeyboardState.IsKeyDown(KeyboardSettings[(int)InputIndices.JUMP]) ||
					_currentGamePadState.IsButtonDown(GamepadSettings[(int)InputIndices.JUMP]))
				{
					_currentInput |= Input.Jump;
				}
				if (_currentKeyboardState.IsKeyDown(KeyboardSettings[(int)InputIndices.ATTACK]) ||
					_currentGamePadState.IsButtonDown(GamepadSettings[(int)InputIndices.ATTACK]))
				{
					_currentInput |= Input.Attack;
				}
				if (_currentKeyboardState.IsKeyDown(KeyboardSettings[(int)InputIndices.DASH]) ||
					_currentGamePadState.IsButtonDown(GamepadSettings[(int)InputIndices.DASH]))
				{
					_currentInput |= Input.Dash;
				}
				if (_currentKeyboardState.IsKeyDown(KeyboardSettings[(int)InputIndices.PAUSE]) ||
					_currentGamePadState.IsButtonDown(GamepadSettings[(int)InputIndices.PAUSE]))
				{
					_currentInput |= Input.Pause;
				}
			}
			else if(state == State.SETTING_KEYBOARD)
			{
				if(currentlySettingIndex == InputIndices.FINISHED)
				{
					SaveKeyboardSettings();
					state = State.REGULAR;	
				}
				else
				{
					//bool foundKeystroke = false;
					for(int b = 0; b < 256; b++)
					{
						if(_currentKeyboardState.IsKeyDown((Keys)b) && _previousKeyboardState.IsKeyUp((Keys)b))
						{
							//foundKeystroke = true;
							KeyboardSettings[(int)currentlySettingIndex] = (Keys)b;
							currentlySettingIndex++;
							break;
						}
					}

				}


			}
			else if (state == State.SETTING_GAMEPAD)
			{
				if (currentlySettingIndex == InputIndices.FINISHED)
				{
					//SaveKeyboardSettings();
					SaveGamepadSettings();
					state = State.REGULAR;
				}
				else
				{
					for (int b = 0; b < 32; b++)
					{
						if (_currentGamePadState.IsButtonDown((Buttons)(1<<b)) && _previousGamePadState.IsButtonUp((Buttons)(1 << b)))
						{
							GamepadSettings[(int)currentlySettingIndex] = (Buttons)(1 << b);
							currentlySettingIndex++;
							break;
						}
					}

				}


			}

			if (state == State.RECORDING)
			{
				_writer!.Write((byte)_currentInput);
			}

			InputActions actions;

			actions.IsDown = _currentInput;
			actions.IsPressed = _currentInput & ~_previousInput;
			actions.IsReleased = ~_currentInput & _previousInput;

			Actions = actions;
		}

	}


}
