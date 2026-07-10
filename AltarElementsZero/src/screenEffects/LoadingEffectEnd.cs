using System;
using System.Collections.Generic;
using System.Text;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.screenEffects
{
	class LoadingEffectEnd : IScreenEffect
	{
		public static readonly LoadingEffectEnd Instance = new();

		private int frame = 0;
		private PxPosition LoadingScreenPosition = new(192, 384);

		public void Start()
		{
			frame = 0;
		}
		public void Update()
		{
			if (!IsFinished())
			{
				frame++;
			}
		}

		public bool IsFinished()
		{
			return frame >= 40;
		}

		public void Draw(SpriteBatch spriteBatch, Texture2D atlas)
		{
			int diagonalOffset = 18 - (frame >> 1);
			for (int j = 0; j < 8; j++)
			{
				for (int i = 0; i < 12; i++)
				{
					int currentDiagonal = i + j;
					if (currentDiagonal < diagonalOffset)
					{
						Vector2 tilePosition = new(i * 16, j * 16);
						Rectangle sourceRectangle = new(
							i * 16 + (int)LoadingScreenPosition.X,
							j * 16 + (int)LoadingScreenPosition.Y,
							16,
							16
							);
						spriteBatch.Draw(
							atlas,
							tilePosition,
							sourceRectangle,
							Color.White
						);
					}
				}

			}

		}

	}
}
