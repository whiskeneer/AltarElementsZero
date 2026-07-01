using Microsoft.Xna.Framework.Input;

namespace AltarElementsZero.src
{
	[Flags]
	public enum Input : Byte
	{
		None = 0,

		Up = 1 << 0,
		Down = 1 << 1,
		Left = 1 << 2,
		Right = 1 << 3,

		Jump = 1 << 4,	// Accept
		Attack = 1 << 5,// Cancel
		Dash = 1 << 6,
		Pause = 1 << 7,
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

		// TODO: make sure that on Dispose all of these are disposed properly
		private FileStream? _stream;
		private BinaryWriter? _writer;
		private BinaryReader? _reader;

		private enum State
		{
			REGULAR,
			RECORDING,
			PLAYBACK
		}
		private State state = State.REGULAR;

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

		public void Update()
		{
			_previousKeyboardState = _currentKeyboardState;
			_currentKeyboardState = Keyboard.GetState();

			_previousGamePadState = _currentGamePadState;
			_currentGamePadState = GamePad.GetState(0);
			
			if(state == State.RECORDING)
			{
				Input recordingInput = Input.None;
				if (_currentKeyboardState.IsKeyDown(Keys.Up) || _currentGamePadState.IsButtonDown(Buttons.DPadUp))
				{
					recordingInput |= Input.Up;
				}
				if (_currentKeyboardState.IsKeyDown(Keys.Down) || _currentGamePadState.IsButtonDown(Buttons.DPadDown))
				{
					recordingInput |= Input.Down;
				}
				if (_currentKeyboardState.IsKeyDown(Keys.Left) || _currentGamePadState.IsButtonDown(Buttons.DPadLeft))
				{
					recordingInput |= Input.Left;
				}
				if (_currentKeyboardState.IsKeyDown(Keys.Right) || _currentGamePadState.IsButtonDown(Buttons.DPadRight))
				{
					recordingInput |= Input.Right;
				}
				if (_currentKeyboardState.IsKeyDown(Keys.F) || _currentGamePadState.IsButtonDown(Buttons.A))
				{
					recordingInput |= Input.Jump;
				}
				if (_currentKeyboardState.IsKeyDown(Keys.D) || _currentGamePadState.IsButtonDown(Buttons.X))
				{
					recordingInput |= Input.Attack;
				}
				if (_currentKeyboardState.IsKeyDown(Keys.S) || _currentGamePadState.IsButtonDown(Buttons.RightShoulder))
				{
					recordingInput |= Input.Dash;
				}
				if (_currentKeyboardState.IsKeyDown(Keys.A) || _currentGamePadState.IsButtonDown(Buttons.Start))
				{
					recordingInput |= Input.Pause;
				}
				_writer!.Write((byte)recordingInput);
			}

			InputActions actions = new();

			MapKey(Keys.Up, ref actions, Input.Up);
			MapKey(Keys.Down, ref actions, Input.Down);
			MapKey(Keys.Left, ref actions, Input.Left);
			MapKey(Keys.Right, ref actions, Input.Right);

			MapKey(Keys.F, ref actions, Input.Jump);
			MapKey(Keys.D, ref actions, Input.Attack);
			MapKey(Keys.S, ref actions, Input.Dash);
			MapKey(Keys.A, ref actions, Input.Pause);

			//
		
			MapButton(Buttons.DPadUp, ref actions, Input.Up);
			MapButton(Buttons.DPadDown, ref actions, Input.Down);
			MapButton(Buttons.DPadLeft, ref actions, Input.Left);
			MapButton(Buttons.DPadRight, ref actions, Input.Right);

			MapButton(Buttons.A, ref actions, Input.Jump);
			MapButton(Buttons.X, ref actions, Input.Attack);
			MapButton(Buttons.RightShoulder, ref actions, Input.Dash);
			MapButton(Buttons.Start, ref actions, Input.Pause);

			Actions = actions;
		}
		private void MapKey(Keys key, ref InputActions actions, Input input)
		{
			bool previousDown = _previousKeyboardState.IsKeyDown(key);
			bool currentDown = _currentKeyboardState.IsKeyDown(key);

			if (currentDown) actions.IsDown |= input;
			if (currentDown && !previousDown) actions.IsPressed |= input;
			if (!currentDown && previousDown) actions.IsReleased |= input; 
		}

		private void MapButton(Buttons button, ref InputActions actions, Input input)
		{
			bool previousDown = _previousGamePadState.IsButtonDown(button);
			bool currentDown = _currentGamePadState.IsButtonDown(button);

			if (currentDown) actions.IsDown |= input;
			if (currentDown && !previousDown) actions.IsPressed |= input;
			if (!currentDown && previousDown) actions.IsReleased |= input;
		}

	}


}
