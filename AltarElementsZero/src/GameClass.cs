using AltarElementsZero.src;
using Microsoft.Xna.Framework;

class GameClass : Game
{
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
        base.LoadContent();
    }
    protected override void UnloadContent()
    {
        base.UnloadContent();
    }
    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}