using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK.Graphics.OpenGL;

namespace Genus2D.Graphics
{
    public class Texture
    {

        private static int _currentBind = 0;

        private int _id;
        private int _width, _height;
        private bool _destroyed;

        public Texture(string filename)
        {
            LoadTexture(filename);
            _destroyed = false;
        }

        public Texture(Bitmap bitmap)
        {
            LoadTexture(bitmap);
            _destroyed = false;
        }

        private void LoadTexture(string filename)
        {
            if (File.Exists(filename))
            {
                Bitmap TextureBitmap = new Bitmap(filename);
                LoadTexture(TextureBitmap);
            }
            else
            {
                throw new Exception("Could not find file " + filename);
            }
        }

        private void LoadTexture(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                _width = bitmap.Width;
                _height = bitmap.Height;
                BitmapData TextureData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                _id = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, _id);
                _currentBind = _id;

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, TextureData.Scan0);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                bitmap.UnlockBits(TextureData);
                bitmap.Dispose();
            }
            else
            {
                throw new Exception("Error, a texture cannot be created from a null bitmap.");
            }
        }

        public int GetWidth()
        {
            return _width;
        }

        public int GetHeight()
        {
            return _height;
        }

        public void Bind()
        {
            if (!_destroyed)
            {
                BindTexture(_id);
            }
            else
            {
                BindNone();
            }
        }

        public static void BindTexture(int id)
        {
            if (_currentBind != id)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, id);
                int uTextureSamplerLocation = Renderer.GetShader().GetUniformLocation("uTextureSampler");
                GL.Uniform1(uTextureSamplerLocation, 0);
                int uTextureFlagLocation = Renderer.GetShader().GetUniformLocation("uTextureFlag");
                GL.Uniform1(uTextureFlagLocation, 1.0f);
                _currentBind = id;
            }
        }

        public static void BindNone()
        {
            if (_currentBind != 0)
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
                if (Renderer.GetShader() != null)
                {
                    int uTextureFlagLocation = Renderer.GetShader().GetUniformLocation("uTextureFlag");
                    GL.Uniform1(uTextureFlagLocation, 0.0f);
                }
                _currentBind = 0;
            }
        }

        public void Destroy()
        {
            if (!_destroyed)
            {
                GL.DeleteTexture(_id);
                _destroyed = true;
            }
        }

        public bool Destroyed()
        {
            return _destroyed;
        }
    }
}
