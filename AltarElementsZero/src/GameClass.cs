using AltarElementsZero.src;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class GameClass : Game
{

    private readonly InputHandler _inputHandler = new();

    private SpriteBatch? _spriteBatch;

    private GlobalAssets? _globalAssets;

    private Rectangle _outputRectangle;
    private bool _resizing = false;
    private RenderTarget2D? _downscaled;

	public GameClass()
	{
		GraphicsDeviceManager graphicDeviceManager = new(this);
		graphicDeviceManager.PreferredBackBufferWidth = Configuration.VisibleScreen.Px.Width * Configuration.VisibleScreen.Scale;
		graphicDeviceManager.PreferredBackBufferHeight = Configuration.VisibleScreen.Px.Height * Configuration.VisibleScreen.Scale;
		graphicDeviceManager.IsFullScreen = Configuration.VisibleScreen.IsFullScreen;
		graphicDeviceManager.SynchronizeWithVerticalRetrace = Configuration.VisibleScreen.SynchronizeWithVerticalRetrace;

		Content.RootDirectory = "assets";
	}

    protected override void Initialize()
    {
        base.Initialize();

        RecalculateOutputRectangle();

        Window.ClientSizeChanged += OnResize;
        Window.AllowUserResizing = true;
    }
    protected override void Dispose(bool disposing)
    {
        Window.ClientSizeChanged -= OnResize;

        base.Dispose(disposing);
    }
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _downscaled = new RenderTarget2D(
            GraphicsDevice,
            Configuration.VisibleScreen.Px.Width,
            Configuration.VisibleScreen.Px.Height
            );

        _globalAssets = new GlobalAssets(GraphicsDevice, Services);
        _globalAssets.Load();

        base.LoadContent();
    }
    protected override void UnloadContent()
    {
        _spriteBatch!.Dispose();
        _downscaled!.Dispose();

        _globalAssets!.Unload();
        _globalAssets = null;

        base.UnloadContent();
    }
    protected override void Update(GameTime gameTime)
    {
        _inputHandler.Update();
        // Console.WriteLine(_inputHandler.Actions.IsDown); // debugging InputHandler

        base.Update(gameTime);
    }
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(_downscaled);
        GraphicsDevice.Clear(Color.Black);
		_spriteBatch!.Begin(
			SpriteSortMode.Deferred,
			BlendState.AlphaBlend,
			SamplerState.PointClamp,
			DepthStencilState.None,
			RasterizerState.CullNone
			);

		_spriteBatch.Draw(
			_globalAssets!.Placeholder,
			Vector2.Zero,
			Color.White
			);

		_spriteBatch.End();

		GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch!.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone
            );

        _spriteBatch.Draw(
            _downscaled, 
            _outputRectangle, 
            Color.White
            );

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void OnResize(object? sender, EventArgs e)
    {
        if (_resizing) return;

        RecalculateOutputRectangle();
    }

    private void RecalculateOutputRectangle()
    {
        _resizing = true;

        Rectangle bounds = GraphicsDevice.PresentationParameters.Bounds;
        float scaleX = (float)bounds.Width / Configuration.VisibleScreen.Px.Width;
        float scaleY = (float)bounds.Height / Configuration.VisibleScreen.Px.Height;
        float scale = Math.Min(scaleX, scaleY);

        int newWidth = (int)(Configuration.VisibleScreen.Px.Width * scale);
        int newHeight = (int)(Configuration.VisibleScreen.Px.Height * scale);

        int posX = (bounds.Width - newWidth) / 2;
        int posY = (bounds.Height - newHeight) / 2;

        _outputRectangle = new Rectangle(posX, posY, newWidth, newHeight);

        _resizing = false;
    }

}