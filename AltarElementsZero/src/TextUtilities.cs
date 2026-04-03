using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src
{
    static class TextUtilities
    {
        static public void DrawChar(
            SpriteBatch spriteBatch,
            Texture2D font,
            int fontWidth,
            int fontHeight,
            char c,
            int outputX,
            int outputY
            )
        {
            int index = c;
            int sourceX = (index & 0xf) * fontWidth;
            int sourceY = (index >> 4) * fontHeight;

            Rectangle source = new(sourceX, sourceY, fontWidth, fontHeight);
            Rectangle output = new(outputX, outputY, fontWidth, fontHeight);

            spriteBatch.Draw(
                font,
                output,
                source,
                Color.White
                );
        }

        static public void DrawText(
            SpriteBatch spriteBatch,
            Texture2D font,
            int fontWidth,
            int fontHeight,
            string text,
            int outputX,
            int outputY
            )
        {
            int charX = outputX;
            int charY = outputY;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if(c == '\n')
                {
                    charX = outputX;
                    charY += fontHeight;
                }
                else
                {
                    DrawChar(
                        spriteBatch,
                        font,
                        fontWidth,
                        fontHeight,
                        c,
                        charX,
                        charY
                        );
                    charX += fontWidth;
                }
            }
        }
    }
}
