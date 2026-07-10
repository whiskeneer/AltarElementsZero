using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.screenEffects
{
	interface IScreenEffect
	{
		void Start();
		void Update();
		void Draw(SpriteBatch spriteBatch, Texture2D atlas);
		bool IsFinished();
	}
}
