using AltarElementsZero.src;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class GameClass : Game
{

    private readonly InputHandler _inputHandler = new();

    private SpriteBatch _spriteBatch;

    private GlobalAssets? _globalAssets;

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
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _globalAssets = new GlobalAssets(GraphicsDevice, Services);
        _globalAssets.Load();

        base.LoadContent();
    }
    protected override void UnloadContent()
    {
        _spriteBatch!.Dispose();

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
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(
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

        base.Draw(gameTime);
    }
}