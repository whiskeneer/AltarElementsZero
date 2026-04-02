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

		Jump = 1 << 4,
		Attack = 1 << 5,
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

		public InputActions Actions { get; private set; }

		public bool IsDown(Input input) => (Actions.IsDown & input) == input;
		public bool IsPressed(Input input) => (Actions.IsPressed & input) == input;
		public bool IsReleased(Input input) => (Actions.IsReleased & input) == input;

		public void Update()
		{
			_previousKeyboardState = _currentKeyboardState;
			_currentKeyboardState = Keyboard.GetState();

			InputActions actions = new();

			MapKey(Keys.Up, ref actions, Input.Up);
			MapKey(Keys.Down, ref actions, Input.Down);
			MapKey(Keys.Left, ref actions, Input.Left);
			MapKey(Keys.Right, ref actions, Input.Right);

			MapKey(Keys.F, ref actions, Input.Jump);
			MapKey(Keys.D, ref actions, Input.Attack);
			MapKey(Keys.S, ref actions, Input.Dash);
			MapKey(Keys.A, ref actions, Input.Pause);

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
	}


}
