using System;
using System.Drawing;

using Genus2D.Utililities;

using OpenTK;
using OpenTK.Graphics;

namespace Genus2D.Graphics
{
    public class BitmapFont
    {
        public struct BitmapGlyph
        {
            public int X, Y, Width, Height, xOffset, yOffset, xAdvance;
        }

        public Texture texture;
        private BitmapGlyph[] glyphs;
        private int lineHeight;

        public BitmapFont(String filename)
        {
            glyphs = new BitmapGlyph[256];
            ParseFont(filename);
            //lineHeight = 24;
        }

        private void ParseFont(String filename)
        {
            String[] lines = System.IO.File.ReadAllLines(filename);
            for (int i = 0; i < lines.Length; i++)
            {
                String[] parameters = lines[i].Split(new char[] { ' ' });
                switch (parameters[0])
                {
                    case "img":
                        this.texture = Assets.GetTexture(parameters[1]);
                        break;
                    case "char":
                        int id = int.Parse(parameters[1].Split(new char[] { '=' })[1]);
                        glyphs[id] = new BitmapGlyph();
                        for (int j = 2; j < parameters.Length; j++)
                        {
                            String[] parameter = parameters[j].Split(new char[] { '=' });
                            switch (parameter[0])
                            {
                                case "x":
                                    glyphs[id].X = int.Parse(parameter[1]);
                                    break;
                                case "y":
                                    glyphs[id].Y = int.Parse(parameter[1]);
                                    break;
                                case "width":
                                    glyphs[id].Width = int.Parse(parameter[1]);
                                    break;
                                case "height":
                                    glyphs[id].Height = int.Parse(parameter[1]);
                                    if (lineHeight < glyphs[id].Height)
                                        lineHeight = glyphs[id].Height;
                                    break;
                                case "xoffset":
                                    glyphs[id].xOffset = int.Parse(parameter[1]);
                                    break;
                                case "yoffset":
                                    glyphs[id].yOffset = int.Parse(parameter[1]);
                                    break;
                                case "xadvance":
                                    glyphs[id].xAdvance = int.Parse(parameter[1]);
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        public int GetLineHeight()
        {
            return lineHeight;
        }

        public void RenderText(String text, float x, float y, float z, float scale, Color4 colour)
        {
            Renderer.SetGrayScaleAlpha(true);
            float rX = x;
            float rY = y;
            int height = this.GetHeight(text);
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                int sx = glyphs[(int)c].X;
                int sy = glyphs[(int)c].Y;
                int sw = glyphs[(int)c].Width;
                int sh = glyphs[(int)c].Height;
                int ox = (int)(glyphs[(int)c].xOffset * scale);
                int oy = (int)(glyphs[(int)c].yOffset * scale);

                Vector3 pos = new Vector3(rX + ox, rY + oy, z);
                Vector3 dim = new Vector3(sw * scale, sh * scale, 1);
                Rectangle src = new Rectangle(sx, sy, sw, sh);
                Vector3 zeroVec = Vector3.Zero;
                Renderer.FillTexture(this.texture, ShapeFactory.Rectangle, ref pos, ref dim, ref zeroVec, ref zeroVec, ref src, ref colour);
                rX += glyphs[(int)c].xAdvance * scale;
            }
            Renderer.SetGrayScaleAlpha(false);
        }

        public int GetWidth(String text)
        {
            int width = 0;
            for (int i = 0; i < text.Length; i++)
                width += glyphs[(int)text[i]].Width;
            return width;
        }

        public int GetHeight(String text)
        {
            int height = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (glyphs[(int)text[i]].Height > height)
                    height = glyphs[(int)text[i]].Height;
            }
            return height;
        }
    }
}
