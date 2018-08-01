using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Genus2D.Graphics
{
    public class TrueTypeFont
    {
        private static SharpFont.Library _library = new SharpFont.Library();

        private SharpFont.Face _fontFace;

        private int _size;
        private int _lineHeight;

        private int _textureID = 0;
        private int _textureSize;

        public class FontGlyph
        {
            public uint GlpyhID;
            public Shape GlyphRect;
            public float SourceX, SourceY;
            public float SourceWidth, SourceHeight;
            public int RealWidth, RealHeight;
            public int BearingX, BearingY;
            public int AdvanceX;
            public int TextureID;
            public bool Loaded;

            public FontGlyph()
            {
                Loaded = false;
            }
        }

        private FontGlyph[] _fontGlyphs = null;

        public TrueTypeFont(string filename, int fontSize)
        {
            _fontFace = new SharpFont.Face(_library, filename);
            fontSize = Math.Max(fontSize, 10);
            fontSize = Math.Min(1024 / 16, fontSize);
            _lineHeight = 0;
            SetFontSize(fontSize);

        }

        private void SetFontSize(int size)
        {
            _size = size;
            //_fontFace.SetCharSize(size, size, 0, 0);
            _fontFace.SetPixelSizes((uint)size, (uint)size);
            GenerateTextures();
        }

        private int Next_P2(int val)
        {
            int rVal = 1;
            while (rVal < val) 
                rVal <<= 1;
            return rVal;
                
        }

        private void GenerateTextures()
        {
            if (_fontGlyphs == null)
            {
                _fontGlyphs = new FontGlyph[256];
            }

            if (_textureID == 0)
                _textureID = GL.GenTexture();

            _textureSize = Next_P2(16 * _size);
            int spacing = _textureSize / 16;
            byte[] textureData = new byte[4 * _textureSize * _textureSize];
            //byte[] outlineData = new byte[4 * _textureSize * _textureSize];

            for (int i = 0; i < 256; i++)
            {
                char c = (char)i;
                uint glyphIndex = _fontFace.GetCharIndex(Convert.ToUInt32(c));
                _fontFace.LoadGlyph(glyphIndex, SharpFont.LoadFlags.Default, SharpFont.LoadTarget.Normal);

                _fontFace.Glyph.RenderGlyph(SharpFont.RenderMode.Normal);

                SharpFont.FTBitmap bitmap = _fontFace.Glyph.Bitmap;


                if (_fontGlyphs[i] == null)
                {
                    _fontGlyphs[i] = new FontGlyph();
                }

                try
                {
                    int blitX = (i % 16 * spacing);// +(spacing / 2) - (bitmap.Width / 2); //trying to center glyph on on tile region
                    int blitY = (i / 16 * spacing);// +(spacing / 2) - (bitmap.Rows / 2);

                    byte[] bitmapData = bitmap.BufferData;
                    for (int y = 0; y < spacing; y++)
                    {
                        for (int x = 0; x < spacing; x++)
                        {
                            byte bltVal = (x >= bitmap.Width || y >= bitmap.Rows) ? (byte)0 : bitmapData[x + bitmap.Width * y];

                            int index = 4 * ((x + blitX) + (y + blitY) * _textureSize);

                            textureData[index] = 255;
                            textureData[index + 1] = 255;
                            textureData[index + 2] = 255; 
                            textureData[index + 3] = bltVal;

                            /* Experimental Outlining of text
                            int[] xOffsets = new int[]{
                                -1, -1, -1, 0, 1, 1, 1, 0
                            };

                            int[] yOffsets = new int[]{
                                1, 0, -1, -1, -1, 0, 1, 1
                            };

                            if (bltVal != 0)
                            {
                                int outlineThickness = 1;
                                bool done = false;
                                for (int dir = 0; dir < 8; dir++)
                                {
                                    int xOffsetBase = xOffsets[dir];
                                    int yOffsetBase = yOffsets[dir];
                                    for (int j = 1; j <= outlineThickness; j++)
                                    {
                                        int xOffset = xOffsetBase * j;
                                        int yOffset = yOffsetBase * j;

                                        byte nextVal = (x + xOffset < 0 || x + xOffset >= bitmap.Width || y + yOffset < 0 || y + yOffset >= bitmap.Rows) ? (byte)0
                                                                                                        : bitmapData[(x + xOffset) + bitmap.Width * (y + yOffset)];
                                        if (nextVal == 0)
                                        {
                                            outlineData[index] = 255;
                                            outlineData[index + 1] = 255;
                                            outlineData[index + 2] = 255;
                                            outlineData[index + 3] = 255;
                                            done = true;
                                            break;
                                        }
                                    }
                                    if (done)
                                        break;

                                    
                                }
                            }
                            */
                        }
                    }

                    _fontGlyphs[i].GlpyhID = glyphIndex;

                    Shape glyphRect = ShapeFactory.GenerateRectangle();
                    glyphRect.SetTextureCoordOffset(blitX / (float)_textureSize, blitY / (float)_textureSize);
                    glyphRect.SetTextureCoordScale(bitmap.Width / (float)_textureSize, bitmap.Rows / (float)_textureSize);
                    _fontGlyphs[i].GlyphRect = glyphRect;

                    _fontGlyphs[i].SourceX = blitX / (float)_textureSize;
                    _fontGlyphs[i].SourceY = blitY / (float)_textureSize;
                    _fontGlyphs[i].SourceWidth = bitmap.Width / (float)_textureSize;
                    _fontGlyphs[i].SourceHeight = bitmap.Rows / (float)_textureSize;
                    _fontGlyphs[i].RealWidth = bitmap.Width;
                    _fontGlyphs[i].RealHeight = bitmap.Rows;
                    if (_lineHeight < bitmap.Rows)
                        _lineHeight = bitmap.Rows;
                    _fontGlyphs[i].BearingX = (int)_fontFace.Glyph.Metrics.HorizontalBearingX;
                    _fontGlyphs[i].BearingY = (int)_fontFace.Glyph.Metrics.HorizontalBearingY;
                    _fontGlyphs[i].AdvanceX = (int)_fontFace.Glyph.Advance.X;
                    _fontGlyphs[i].TextureID = _textureID;
                    _fontGlyphs[i].Loaded = true;
                }
                catch
                {
                    _fontGlyphs[i].Loaded = false;
                }

            }

            Texture.BindTexture(_textureID);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _textureSize, _textureSize, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, textureData);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
        }

        public int GetSize()
        {
            return _size;
        }

        public int GetLineHeight()
        {
            return _lineHeight;
        }

        public FontGlyph GetFontGlyph(int character)
        {
            if (character >= 0 && character < 256)
                return _fontGlyphs[character];
            return null;
        }

        public int GetKerning(uint prevGlyph, uint currentGlyph)
        {
            int kerning = (int)_fontFace.GetKerning(prevGlyph, currentGlyph, SharpFont.KerningMode.Default).X;
            return kerning;
        }

        public int GetTextWidth(string text)
        {
            int width = 0;
            int maxWidth = 0;

            uint prevC = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    width += _size / 3;
                }
                else if (text[i] == '\n')
                {
                    maxWidth = width;
                    width = 0;
                }
                else
                {
                    int c = (int)text[i];
                    width += _fontGlyphs[c].AdvanceX + 1;
                    if (i > 0)
                    {
                        width += GetKerning(prevC, _fontGlyphs[c].GlpyhID);
                    }
                    prevC = _fontGlyphs[c].GlpyhID;
                }
            }
            if (maxWidth < width)
                maxWidth = width;

            return maxWidth;
        }

        public int GetTextHeight(string text)
        {
            int height = 0;
            int lineHeight = 0;

            for (int i = 0; i < text.Length; i++)
            {
                int c = text[i];
                if (c == '\n')
                {
                    height += lineHeight;
                    lineHeight = 0;
                }
                else if (_fontGlyphs[c].RealHeight > 0)
                {
                    lineHeight = _lineHeight;
                }
            }
            height += lineHeight;

            return height;
        }

        public void Destroy()
        {
            for (int i = 0; i < _fontGlyphs.Length; i++)
            {
                if (_fontGlyphs[i].Loaded)
                {
                    _fontGlyphs[i].GlyphRect.Destroy();
                }
            }

            GL.DeleteTexture(_textureID);
        }

    }
}
