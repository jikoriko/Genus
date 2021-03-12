using System;
using System.Collections;
using System.Drawing;

using Genus2D.Core;
using Genus2D.Utililities;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Genus2D.Graphics
{
    public class Renderer
    {

        public enum ResolutionMode { Dynamic, Fixed }
        private static ResolutionMode _resolutionMode = ResolutionMode.Dynamic;

        private static Rectangle _screenBounds;
        private static Vector2 _resolution;
        private static Rectangle _viewport;
        private static bool _fullscreen = false;

        private static Shader _currentShader;

        private static bool _calculateGradientRect = true;

        private static Vector3 _zeroVector = Vector3.Zero;

        private static Matrix4 _projectionMatrix = Matrix4.Identity;
        private static Matrix4 _worldMatrix = Matrix4.Identity;
        private static bool _worldMatrixChanged = true;
        private static Stack _worldMatrixStack = new Stack();

        private static Stack _clipStack = new Stack();
        private static bool _screenClip = false;

        private static bool[] _flipUV = { false, false };

        private static int _stencilDepth = -1;
        private class StencilDepthParamaters
        {
            public StencilOp op;
            public StencilFunction function;
        }
        private static Stack _stencilStack = new Stack();

        private static bool _checkCull = true;

        public enum GradientMode
        {
            None, Horizontal, Vertical, Diagonal, 
            HorizontalMidBand, VerticalMidBand, DiagonalMidBand,
            RadialFull, RadialFit, DiamondFull, DiamondFit
        }

        private static GradientMode _gradientMode = GradientMode.None;

        public enum ProjectionMode { Ortho, Perspective }
        private static ProjectionMode _projectionMode = ProjectionMode.Ortho;

        private static Color4 _startColour = Color4.Transparent, _endColour = Color4.Transparent;
        private static Color4 _defaultColour = Color4.White;
        private static Color4 _clearColour = Color4.Black;

        private static TrueTypeFont _currentFont;

        public static void Initialize()
        {
            Shader shader = Assets.GetShader("vPassThrough.vert", "fColour.frag");
            SetShader(shader);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            SetColour(ref _defaultColour, ref _defaultColour);
            SetFlipUV(false, false);
            _currentFont = Assets.GetFont("arial.ttf", 20);
            _resolution = Vector2.Zero;
            SetScreenBounds(new Rectangle(0, 0, StateWindow.Instance.Width, StateWindow.Instance.Height));
            Assets.PreLoadTextures();
        }

        public static void DisableDepthWrite()
        {
            GL.DepthMask(false);
        }

        public static void EnableDepthWrite()
        {
            GL.DepthMask(true);
        }

        public static void DisableColourWrite()
        {
            GL.ColorMask(false, false, false, false);
        }

        public static void EnableColourWrite()
        {
            GL.ColorMask(true, true, true, true);
        }

        public static void DisableDepthTest()
        {
            GL.Disable(EnableCap.DepthTest);
        }

        public static void EnableDepthTest()
        {
            GL.Enable(EnableCap.DepthTest);
        }

        public static void DisableBlend()
        {
            GL.Disable(EnableCap.Blend);
        }

        public static void EnableBlend()
        {
            GL.Enable(EnableCap.Blend);
        }

        public static void DisableAlphaTest()
        {
            GL.Disable(EnableCap.AlphaTest);
        }

        public static void EnableAlphaTest()
        {
            GL.Enable(EnableCap.AlphaTest);
        }

        public static void SetAlphaFunc(AlphaFunction func, float value)
        {
            GL.AlphaFunc(func, value);
        }

        public static void SetDepthFunc(DepthFunction func)
        {
            GL.DepthFunc(func);
        }

        public static void SetBlendFunc(BlendingFactorSrc src, BlendingFactorDest dest)
        {
            GL.BlendFunc(src, dest);
        }

        public static void SetShader(Shader shader)
        {
            _currentShader = shader;
            _currentShader.Bind();
        }

        public static Shader GetShader()
        {
            return _currentShader;
        }

        public static void SetFont(TrueTypeFont font)
        {
            _currentFont = font;
        }

        public static TrueTypeFont GetFont()
        {
            return _currentFont;
        }

        public static void SetClearColour(Color4 colour)
        {
            if (!_clearColour.Equals(colour))
            {
                GL.ClearColor(colour);
                _clearColour = colour;
            }
        }

        public static void SetColour(ref Color4 startColour, ref Color4 endColour)
        {
            int uColorLocation;
            if (!_startColour.Equals(startColour))
            {
                uColorLocation = _currentShader.GetUniformLocation("uStartColour");
                GL.Uniform4(uColorLocation, startColour);
                _startColour = startColour;
            }
            if (!_endColour.Equals(endColour))
            {
                uColorLocation = _currentShader.GetUniformLocation("uEndColour");
                GL.Uniform4(uColorLocation, endColour);
                _endColour = endColour;
            }
        }

        public static void SetFlipUV(bool flipX, bool flipY)
        {
            int uxLocation = _currentShader.GetUniformLocation("uFlipUV");
            GL.Uniform2(uxLocation, flipX == true ? 1.0f : 0.0f, flipY == true ? 1.0f : 0.0f);
        }

        public static void SetTextureOffset(float x, float y)
        {
            int uxLocation = _currentShader.GetUniformLocation("uTexOffset");
            GL.Uniform2(uxLocation, x, y);
        }

        public static void Clear()
        {
            _worldMatrixStack.Clear();
            _worldMatrix = Matrix4.Identity;
            _worldMatrixChanged = true;
            _clipStack.Clear();
            ClearScreenClip();
            _stencilDepth = -1;
            _stencilStack.Clear();
            GL.Disable(EnableCap.StencilTest);
            ClearColourBits();
            ClearDepthBits();
            ClearStencilBits();
        }

        public static void ClearColourBits()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public static void ClearDepthBits()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }

        public static void ClearStencilBits()
        {
            GL.Clear(ClearBufferMask.StencilBufferBit);
        }

        public static void PushWorldMatrix()
        {
            if (_worldMatrixStack.Count != 0)
                _worldMatrixStack.Push(_worldMatrixStack.Peek());
            else
                _worldMatrixStack.Push(_worldMatrix);
            _worldMatrixChanged = true;
        }

        public static void PopWorldMatrix()
        {
            if (_worldMatrixStack.Count != 0)
            {
                _worldMatrixChanged = true;
                _worldMatrixStack.Pop();
            }
        }

        public static void SetWorldMatrix(Matrix4 matrix)
        {
            if (_worldMatrixStack.Count != 0)
            {
                _worldMatrixStack.Pop();
                _worldMatrixStack.Push(matrix);
            }
            else
            {
                _worldMatrix = matrix;
            }
            _worldMatrixChanged = true;
        }

        public static Matrix4 GetWorldMatrix()
        {
            if (_worldMatrixStack.Count != 0)
            {
                Matrix4 currentMatrix = (Matrix4)_worldMatrixStack.Peek();
                return currentMatrix;
            }
            else
            {
                return _worldMatrix;
            }
        }

        public static void TranslateWorld(Vector3 translation)
        {
            TranslateWorld(translation.X, translation.Y, translation.Z);
        }

        public static void TranslateWorld(float x, float y, float z)
        {
            if (_worldMatrixStack.Count != 0)
            {
                Matrix4 currentMatrix = (Matrix4)_worldMatrixStack.Peek();
                _worldMatrixStack.Pop();
                currentMatrix *= Matrix4.CreateTranslation(x, y, z);
                _worldMatrixStack.Push(currentMatrix);
            }
            else
            {
                _worldMatrix *= Matrix4.CreateTranslation(x, y, z);
            }
            _worldMatrixChanged = true;
        }

        public static void SetWorldTranslation(Vector3 translation)
        {
            if (_worldMatrixStack.Count != 0)
            {
                _worldMatrixStack.Pop();
                _worldMatrixStack.Push(Matrix4.CreateTranslation(translation));
                _worldMatrixChanged = true;
            }
            else
            {
                _worldMatrix = Matrix4.Identity;
                TranslateWorld(translation);
            }
        }

        public static Vector3 GetWorldTranslation()
        {
            if (_worldMatrixStack.Count != 0)
            {
                Matrix4 currentMatrix = (Matrix4)_worldMatrixStack.Peek();
                return currentMatrix.ExtractTranslation();
            }
            else
            {
                return _worldMatrix.ExtractTranslation();
            }
        }

        private static void ApplyWorldMatrix()
        {
            if (_worldMatrixChanged)
            {
                int uWorldLocation = _currentShader.GetUniformLocation("uWorld");
                Matrix4 worldMatrix = _worldMatrixStack.Count > 0 ? (Matrix4)_worldMatrixStack.Peek() : _worldMatrix;
                GL.UniformMatrix4(uWorldLocation, true, ref worldMatrix);

                _worldMatrixChanged = false;
            }
        }

        private static void SetScreenClip(Rectangle clip)
        {
            if (!_screenClip)
            {
                GL.Enable(EnableCap.ScissorTest);
                _screenClip = true;
            }

            float aspectRatio = GetAspectRatioX();

            int scissorX = _viewport.X + (int)(clip.X * aspectRatio);
            int scissorWidth = (int)Math.Ceiling(clip.Width * aspectRatio);
            int scissorHeight = (int)Math.Ceiling(clip.Height * aspectRatio);
            int scissorY = (int)(_viewport.Bottom - (clip.Y * aspectRatio) - (clip.Height * aspectRatio));

            GL.Scissor(scissorX, scissorY, scissorWidth, scissorHeight);
        }

        public static void PushScreenClip(Rectangle clip, bool scissorClip = true)
        {
            if (_clipStack.Count > 0)
            {
                Rectangle currentClip = (Rectangle)_clipStack.Peek();
                if (currentClip.Contains(clip) || scissorClip == false)
                {
                    SetScreenClip(clip);
                    _clipStack.Push(clip);
                }
                else if (currentClip.IntersectsWith(clip))
                {
                    currentClip.Intersect(clip);
                    SetScreenClip(currentClip);
                    _clipStack.Push(currentClip);
                }
                else
                {
                    SetScreenClip(Rectangle.Empty);
                    _clipStack.Push(Rectangle.Empty);
                }
            }
            else
            {
                SetScreenClip(clip);
                _clipStack.Push(clip);
            }
        }

        public static void PopScreenClip()
        {
            if (_clipStack.Count == 0)
                return;
            else
            {

                _clipStack.Pop();
                if (_clipStack.Count > 0)
                {
                    SetScreenClip((Rectangle)_clipStack.Peek());
                }
                else
                {
                    ClearScreenClip();
                }
            }
        }

        private static void ClearScreenClip()
        {
            GL.Disable(EnableCap.ScissorTest);
            _screenClip = false;
        }

        public static Rectangle GetScreenClip()
        {
            if (_screenClip)
            {
                return (Rectangle)_clipStack.Peek();
            }
            else
            {
                return _viewport;
            }
        }

        public static bool InsideScreenClip(ref Vector2 minBounds, ref Vector2 maxBounds, ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Vector3 offset)
        {
            //return true;
            if (_projectionMode == ProjectionMode.Perspective)
                return true;
            Rectangle screen = GetScreenClip();

            if (screen == Rectangle.Empty)
                return false;

            Matrix4 rotX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X));
            Matrix4 rotY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation.Y));
            Matrix4 rotZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z));
            Matrix4 rotationMatrix = Matrix4.CreateTranslation(offset) * rotX * rotY * rotZ;

            Vector3 worldTranslation = GetWorldTranslation();

            Vector3 TL = new Vector3(minBounds.X, minBounds.Y, 0) * scale;
            Vector3 TR = new Vector3(maxBounds.X, minBounds.Y, 0) * scale;
            Vector3 BR = new Vector3(maxBounds.X, maxBounds.Y, 0) * scale;
            Vector3 BL = new Vector3(minBounds.X, maxBounds.Y, 0) * scale;

            Vector3[] positions = new Vector3[] {
                Vector3.Transform(TL, rotationMatrix) + position + worldTranslation,
                Vector3.Transform(TR, rotationMatrix) + position + worldTranslation,
                Vector3.Transform(BR, rotationMatrix) + position + worldTranslation,
                Vector3.Transform(BL, rotationMatrix) + position + worldTranslation
            };

            float minX = positions[0].X;
            float maxX = positions[0].X;
            float minY = positions[0].Y;
            float maxY = positions[0].Y;

            for (int i = 1; i < positions.Length; i++)
            {
                if (minX > positions[i].X)
                    minX = positions[i].X;
                if (maxX < positions[i].X)
                    maxX = positions[i].X;
                if (minY > positions[i].Y)
                    minY = positions[i].Y;
                if (maxY < positions[i].Y)
                    maxY = positions[i].Y;
            }

            if (minX < screen.Right && maxX > screen.X && minY < screen.Bottom && maxY > screen.Y)
                return true;
            return false;
        }

        public static void PushStencilDepth(StencilOp op, StencilFunction function)
        {
            if (_stencilDepth == -1)
            {
                GL.Enable(EnableCap.StencilTest);
                GL.StencilMask(0xFF);
                //GL.Clear(ClearBufferMask.StencilBufferBit);
            }
            _stencilDepth++;

            StencilDepthParamaters stencilParamaters = new StencilDepthParamaters();
            _stencilStack.Push(stencilParamaters);
            SetStencilOp(op);
            SetStencilFunction(function);
        }

        public static void SetStencilOp(StencilOp op)
        {
            StencilDepthParamaters stencilParamaters = (StencilDepthParamaters)_stencilStack.Peek();
            stencilParamaters.op = op;
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, op);
        }

        public static void SetStencilFunction(StencilFunction function)
        {
            if (_stencilStack.Count > 0)
            {
                StencilDepthParamaters stencilParamaters = (StencilDepthParamaters)_stencilStack.Peek();
                stencilParamaters.function = function;
            }
            GL.StencilFunc(function, _stencilDepth, 0xFF);
        }

        public static void PopStencilDepth()
        {
            if (_stencilDepth > -1)
            {
                _stencilDepth--;
                _stencilStack.Pop();
                if (_stencilDepth == -1)
                {
                    GL.Disable(EnableCap.StencilTest);
                }
                else
                {
                    StencilDepthParamaters stencilParameters = (StencilDepthParamaters)_stencilStack.Peek();
                    SetStencilOp(stencilParameters.op);
                    SetStencilFunction(stencilParameters.function);
                }
            }
        }

        public static bool GetFulscreen()
        {
            return _fullscreen;
        }

        public static void SetFulscreen(bool fullscreen)
        {
            _fullscreen = fullscreen;
            if (fullscreen)
            {
                StateWindow.Instance.WindowState = WindowState.Fullscreen;
            }
            else
            {
                StateWindow.Instance.WindowState = WindowState.Normal;
            }
        }

        public static void SetResolutionMode(ResolutionMode resolutionMode)
        {
            if (_resolutionMode != resolutionMode)
            {
                _resolutionMode = resolutionMode;
                SetResolution(_resolution);
            }
        }

        public static void SetResolution(Vector2 resolution)
        {
            SetResolution((int)resolution.X, (int)resolution.Y);
        }

        public static void SetResolution(int width, int height)
        {
            bool changed = false;
            if (_resolution.X != width || _resolution.Y != height)
            {
                _resolution.X = width;
                _resolution.Y = height;

                int uResolutionLocation = _currentShader.GetUniformLocation("uResolution");
                GL.Uniform2(uResolutionLocation, ref _resolution);

                changed = true;
            }

            SetGradientRectangle(0, 0, _resolution.X, _resolution.Y);
            CalculateViewport();
            SetProjection();

            if (changed)
            {
                if (OnResolutionChange != null)
                    OnResolutionChange();
            }
        }

        public static Vector2 GetResoultion()
        {
            return _resolution;
        }

        public delegate void OnResolutionChangeEvent();
        public static OnResolutionChangeEvent OnResolutionChange;

        public static float GetAspectRatioX()
        {
            return _viewport.Width / _resolution.X;
        }

        public static float GetAspectRatioY()
        {
            return _viewport.Height / _resolution.Y;
        }

        public static void SetScreenBounds(Rectangle bounds)
        {
            _screenBounds = bounds;

            int uScreenLocation = _currentShader.GetUniformLocation("uScreen");
            GL.Uniform2(uScreenLocation, new Vector2(bounds.Width, bounds.Height));

            if (_resolutionMode == ResolutionMode.Dynamic)
            {
                SetResolution(new Vector2(bounds.Width, bounds.Height));
            }
            else
            {
                SetResolution(_resolution);
            }
        }

        private static void CalculateViewport()
        {
            if (_resolutionMode == ResolutionMode.Fixed)
            {
                float targetAspectRatio = _resolution.X / _resolution.Y;
                int vpWidth = _screenBounds.Width;
                int vpHeight = (int)(vpWidth / targetAspectRatio + 0.5f);

                if (vpHeight > _screenBounds.Height)
                {
                    vpHeight = _screenBounds.Height;
                    vpWidth = (int)(vpHeight * targetAspectRatio + 0.5f);
                }

                int vpX = (_screenBounds.Width / 2) - (vpWidth / 2);
                int vpY = (_screenBounds.Height / 2) - (vpHeight / 2);

                SetViewport(new Rectangle(vpX, vpY, vpWidth, vpHeight));
            }
            else
            {
                SetViewport(_screenBounds);
            }
        }

        private static void SetViewport(Rectangle viewport)
        {
            _viewport = viewport;
            GL.Viewport(_viewport);

            int uViewportLocation = _currentShader.GetUniformLocation("uViewport");
            Vector4 viewportVector = new Vector4(_viewport.X, _viewport.Y, _viewport.Width, _viewport.Height);
            GL.Uniform4(uViewportLocation, ref viewportVector);
        }

        public static Rectangle GetViewport()
        {
            return _viewport;
        }

        public static Vector2 GetViewportPosition()
        {
            return new Vector2(_viewport.X, _viewport.Y);
        }

        public static Vector2 GetViewportSize()
        {
            return new Vector2(_viewport.Width, _viewport.Height);
        }

        public static Vector2 GetViewportScale()
        {
            Vector2 viewportSize = GetViewportSize();
            Vector2 scale = new Vector2(viewportSize.X / _screenBounds.Width, viewportSize.Y / _screenBounds.Height);
            return scale;
        }

        private static void SetProjection()
        {
            Matrix4 projection = Matrix4.Identity;

            if (_projectionMode == ProjectionMode.Ortho)
            {
                projection = Matrix4.CreateOrthographicOffCenter(0, _resolution.X, _resolution.Y, 0, 10000, -10000);
            }
            else
            {
                projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, _resolution.X / _resolution.Y, 0.01f, 100000f);
            }

            if (_projectionMatrix != projection)
            {
                _projectionMatrix = projection;
                int uProjectionLocation = _currentShader.GetUniformLocation("uProjection");
                GL.UniformMatrix4(uProjectionLocation, true, ref _projectionMatrix);
            }
        }

        public static void SetGradientMode(GradientMode mode)
        {
            _gradientMode = mode;
            int uGradientModeLocation = _currentShader.GetUniformLocation("uGradientMode");
            GL.Uniform1(uGradientModeLocation, (int)mode);
        }

        public static GradientMode GetGradientMode()
        {
            return _gradientMode;
        }

        public static void SetProjectionMode(ProjectionMode mode)
        {
            if (_projectionMode != mode)
            {
                _projectionMode = mode;
                SetScreenBounds(_screenBounds);
            }
        }

        public static ProjectionMode GetProjectionMode()
        {
            return _projectionMode;
        }

        private static void SetGradientRectangle(float x, float y, float width, float height)
        {
            Vector3 worldPosition = GetWorldTranslation();
            Vector2 rectStart = new Vector2(worldPosition.X + x, worldPosition.Y + y);
            Vector2 rectDimension = new Vector2(width, height);
            int uRectStartLocation = _currentShader.GetUniformLocation("uGradientStart");
            GL.Uniform2(uRectStartLocation, ref rectStart);
            int uRectDimensionLocation = _currentShader.GetUniformLocation("uGradientDimension");
            GL.Uniform2(uRectDimensionLocation, ref rectDimension);
        }

        private static void SetGradientRotation(Matrix4 rotation)
        {
            int uRotationLocation = _currentShader.GetUniformLocation("uGradientRotation");
            rotation.Invert();
            GL.UniformMatrix4(uRotationLocation, false, ref rotation);
        }

        private static void SetGradientOffset(ref Vector2 offset)
        {
            int uGradientOffsetLocation = _currentShader.GetUniformLocation("uGradientOffset");
            GL.Uniform2(uGradientOffsetLocation, ref offset);
        }

        public static void SetGrayScaleAlpha(bool enabled)
        {
            int uGrayScaleAlphaLocation = _currentShader.GetUniformLocation("uGrayScaleAlpha");
            GL.Uniform1(uGrayScaleAlphaLocation, enabled ? 1 : 0);
        }

        public static Color4 GetDarkerColour(Color4 source)
        {
            float r = source.R * 0.8f;
            float g = source.G * 0.8f;
            float b = source.B * 0.8f;
            float a = source.A;
            Color4 colour = new Color4(r, g, b, a);
            return colour;
        }

        public static Color4 GetLighterColour(Color4 source, float multiply = 1.0f)
        {
            float r = source.R * (1.2f * multiply);
            float g = source.G * (1.2f * multiply);
            float b = source.B * (1.2f * multiply);
            float a = source.A;
            Color4 colour = new Color4(r, g, b, a);
            return colour;
        }


        public static void FillTexture(Texture texture, Shape shape, ref Vector3 position, ref Color4 colour)
        {
            Vector3 scale = new Vector3(texture.GetWidth(), texture.GetHeight(), 1);
            FillTexture(texture, shape, ref position, ref scale, ref colour);
        }

        public static void FillTexture(Texture texture, Shape shape, ref Vector3 position, ref Vector3 scale, ref Color4 colour)
        {
            FillTexture(texture, shape, ref position, ref scale, ref _zeroVector, ref colour);
        }

        public static void FillTexture(Texture texture, Shape shape, ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Color4 colour)
        {
            FillTexture(texture, shape, ref position, ref scale, ref rotation, ref _zeroVector, ref colour);
        }

        public static void FillTexture(Texture texture, Shape shape, ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Vector3 centerOffset,
                                            ref Color4 colour)
        {
            Rectangle source = new Rectangle(0, 0, texture.GetWidth(), texture.GetHeight());
            FillTexture(texture, shape, ref position, ref scale, ref rotation, ref centerOffset, ref source, ref colour);
        }

        public static void FillTexture(Texture texture, Shape shape, ref Vector3 position, ref Vector3 scale, ref Rectangle source, ref Color4 colour)
        {
            FillTexture(texture, shape, ref position, ref scale, ref _zeroVector, ref _zeroVector, ref source, ref colour);
        }

        public static void FillTexture(Texture texture, Shape shape, ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Vector3 centerOffset,
                                            ref Rectangle source, ref Color4 colour)
        {
            FillTexture(texture, shape, ref position, ref scale, ref rotation, ref centerOffset, ref source, ref colour, ref colour);
        }

        public static void FillTexture(Texture texture, Shape shape, ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Vector3 centerOffset,
                                            ref Rectangle source, ref Color4 startColour, ref Color4 endColour)
        {
            if (_checkCull)
            {
                Vector2 minBounds = shape.GetMinBounds();
                Vector2 maxBounds = shape.GetMaxBounds();
                if (!InsideScreenClip(ref minBounds, ref maxBounds, ref position, ref scale, ref rotation, ref centerOffset))
                    return;
            }
            texture.Bind();
            float textureX = ((float)source.X + 0.5f) / texture.GetWidth();
            float textureY = ((float)source.Y + 0.5f) / texture.GetHeight();
            float textureW = ((float)source.Width - 1f) / texture.GetWidth();
            float textureH = ((float)source.Height - 1f) / texture.GetHeight();


            shape.SetTextureCoordOffset(textureX, textureY);
            shape.SetTextureCoordScale(textureW, textureH);

            shape.Bind(_currentShader);

            SetColour(ref startColour, ref endColour);
            if (_calculateGradientRect)
            {
                SetGradientRectangle(position.X + centerOffset.X, position.Y + centerOffset.Y, scale.X, scale.Y);
                Vector2 offset2d = centerOffset.Xy;
                SetGradientOffset(ref offset2d);
            }

            ApplyWorldMatrix();

            int uModelLocation = _currentShader.GetUniformLocation("uModel");
            Matrix4 rotX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X));
            Matrix4 rotY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation.Y));
            Matrix4 rotZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z));
            Matrix4 rotationMatrix = rotX * rotY * rotZ;
            SetGradientRotation(rotationMatrix);

            Matrix4 matrix = Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(centerOffset) * rotationMatrix * Matrix4.CreateTranslation(position);
            GL.UniformMatrix4(uModelLocation, true, ref matrix);

            GL.DrawElements(shape.GetPrimitiveType(), shape.IndicesLength(), DrawElementsType.UnsignedInt, 0);
        }

        public static void DrawShape(Shape shape, ref Vector3 position, ref Vector3 scale, float lineWidth, ref Color4 colour)
        {
            DrawShape(shape, ref position, ref scale, ref _zeroVector, lineWidth, ref colour);
        }

        public static void DrawShape(Shape shape, ref Vector3 position, ref Vector3 scale, float lineWidth, ref Color4 startColour, ref Color4 endColour)
        {
            DrawShape(shape, ref position, ref scale, ref _zeroVector, ref _zeroVector, lineWidth, ref startColour, ref endColour);
        }

        public static void DrawShape(Shape shape, ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, float lineWidth, ref Color4 colour)
        {
            DrawShape(shape, ref position, ref scale, ref rotation, ref _zeroVector, lineWidth, ref colour);
        }

        public static void DrawShape(Shape shape, ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Vector3 centerOffset, float lineWidth,
                                        ref Color4 colour)
        {
            DrawShape(shape, ref position, ref scale, ref rotation, ref centerOffset, lineWidth, ref colour, ref colour);
        }

        public static void DrawShape(Shape shape, ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Vector3 centerOffset, float lineWidth,
                                        ref Color4 startColour, ref Color4 endColour)
        {
            if (_checkCull)
            {
                Vector2 minBounds = shape.GetMinBounds();
                Vector2 maxBounds = shape.GetMaxBounds();
                if (!InsideScreenClip(ref minBounds, ref maxBounds, ref position, ref scale, ref rotation, ref centerOffset))
                    return;
            }

            _checkCull = false;

            PushStencilDepth(StencilOp.Incr, StencilFunction.Lequal);

            Vector3 stencilOffset = centerOffset;
            Vector3 stencilScale = scale;

            stencilOffset.X += lineWidth;
            stencilOffset.Y += lineWidth;
            stencilScale.X -= lineWidth * 2;
            stencilScale.Y -= lineWidth * 2;

            GL.ColorMask(false, false, false, false);
            GL.DepthMask(false);

            FillShape(shape, ref position, ref stencilScale, ref rotation, ref stencilOffset, ref startColour, ref endColour);

            SetStencilOp(StencilOp.Keep);
            SetStencilFunction(StencilFunction.Equal);

            GL.ColorMask(true, true, true, true);
            GL.DepthMask(true);

            FillShape(shape, ref position, ref scale, ref rotation, ref centerOffset, ref startColour, ref endColour);

            SetStencilOp(StencilOp.Decr);
            SetStencilFunction(StencilFunction.Lequal);

            GL.ColorMask(false, false, false, false);
            GL.DepthMask(false);

            FillShape(shape, ref position, ref stencilScale, ref rotation, ref stencilOffset, ref startColour, ref endColour);

            GL.ColorMask(true, true, true, true);
            GL.DepthMask(true);

            PopStencilDepth();

            _checkCull = true;
        }

        public static void FillShape(Shape shape, ref Vector3 position, ref Vector3 scale, ref Color4 colour)
        {
            FillShape(shape, ref position, ref scale, ref _zeroVector, ref colour);
        }

        public static void FillShape(Shape shape, ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Color4 colour)
        {
            FillShape(shape, ref position, ref scale, ref rotation, ref _zeroVector, ref colour);
        }

        public static void FillShape(Shape shape, ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Vector3 centerOffset, ref Color4 colour)
        {
            FillShape(shape, ref position, ref scale, ref rotation, ref centerOffset, ref colour, ref colour);
        }

        public static void FillShape(Shape shape, ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Vector3 centerOffset,
                                        ref Color4 startColour, ref Color4 endColour)
        {
            if (_checkCull)
            {
                Vector2 minBounds = shape.GetMinBounds();
                Vector2 maxBounds = shape.GetMaxBounds();
                if (!InsideScreenClip(ref minBounds, ref maxBounds, ref position, ref scale, ref rotation, ref centerOffset))
                    return;
            }

            Texture.BindNone();

            shape.Bind(_currentShader);

            SetColour(ref startColour, ref endColour);
            if (_calculateGradientRect)
            {
                SetGradientRectangle(position.X + centerOffset.X, position.Y + centerOffset.Y, scale.X, scale.Y);
                Vector2 offset2d = centerOffset.Xy;
                SetGradientOffset(ref offset2d);
            }

            ApplyWorldMatrix();

            int uModelLocation = _currentShader.GetUniformLocation("uModel");
            Matrix4 rotX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X));
            Matrix4 rotY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation.Y));
            Matrix4 rotZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z));
            Matrix4 rotationEuler = rotX * rotY * rotZ;
            SetGradientRotation(rotationEuler);

            Matrix4 matrix = Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(centerOffset) * rotationEuler * Matrix4.CreateTranslation(position);
            GL.UniformMatrix4(uModelLocation, true, ref matrix);

            GL.DrawElements(shape.GetPrimitiveType(), shape.IndicesLength(), DrawElementsType.UnsignedInt, 0);
        }

        public static void DrawRoundedRectangle(ref Vector3 position, ref Vector3 scale, float cornerRadius, float lineWidth, ref Color4 colour)
        {
            DrawRoundedRectangle(ref position, ref scale, ref _zeroVector, cornerRadius, lineWidth, ref colour);
        }

        public static void DrawRoundedRectangle(ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, float cornerRadius, float lineWidth, ref Color4 colour)
        {
            DrawRoundedRectangle(ref position, ref scale, ref rotation, ref _zeroVector, cornerRadius, lineWidth, ref colour);
        }

        public static void DrawRoundedRectangle(ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Vector3 centerOffset, float cornerRadius,
                                                    float lineWidth, ref Color4 colour)
        {
            DrawRoundedRectangle(ref position, ref scale, ref rotation, ref centerOffset, cornerRadius, lineWidth, ref colour, ref colour);
        }

        public static void DrawRoundedRectangle(ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Vector3 centerOffset, float cornerRadius,
                                                    float lineWidth, ref Color4 startColour, ref Color4 endColour)
        {
            if (_checkCull)
            {
                Vector2 minBounds = Vector2.Zero;
                Vector2 maxBounds = Vector2.One;
                if (!InsideScreenClip(ref minBounds, ref maxBounds, ref position, ref scale, ref rotation, ref centerOffset))
                    return;
            }

            _checkCull = false;

            GL.PushAttrib(AttribMask.StencilBufferBit);
            GL.ColorMask(false, false, false, false);
            GL.DepthMask(false);

            PushStencilDepth(StencilOp.Incr, StencilFunction.Lequal);

            Vector3 stencilOffset = centerOffset;
            Vector3 stencilScale = scale;

            stencilOffset.X += lineWidth;
            stencilOffset.Y += lineWidth;
            stencilScale.X -= lineWidth * 2;
            stencilScale.Y -= lineWidth * 2;

            FillRoundedRectangle(ref position, ref stencilScale, ref rotation, ref stencilOffset, cornerRadius, ref startColour, ref endColour);

            GL.ColorMask(true, true, true, true);
            GL.DepthMask(true);

            SetStencilOp(StencilOp.Keep);
            SetStencilFunction(StencilFunction.Equal);

            FillRoundedRectangle(ref position, ref scale, ref rotation, ref centerOffset, cornerRadius, ref startColour, ref endColour);

            SetStencilOp(StencilOp.Decr);
            SetStencilFunction(StencilFunction.Lequal);

            GL.ColorMask(false, false, false, false);
            GL.DepthMask(false);

            FillRoundedRectangle(ref position, ref stencilScale, ref rotation, ref stencilOffset, cornerRadius, ref startColour, ref endColour);

            GL.ColorMask(true, true, true, true);
            GL.DepthMask(true);

            PopStencilDepth();

            _checkCull = true;
        }

        public static void FillRoundedRectangle(ref Vector3 position, ref Vector3 scale, float cornerRadius, ref Color4 colour)
        {
            FillRoundedRectangle(ref position, ref scale, ref _zeroVector, cornerRadius, ref colour);
        }

        public static void FillRoundedRectangle(ref Vector3 position, ref Vector3 scale, float cornerRadius, ref Color4 startColour, ref Color4 endColour)
        {
            FillRoundedRectangle(ref position, ref scale, ref _zeroVector, ref _zeroVector, cornerRadius, ref startColour, ref endColour);
        }

        public static void FillRoundedRectangle(ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, float cornerRadius, ref Color4 colour)
        {
            FillRoundedRectangle(ref position, ref scale, ref rotation, ref _zeroVector, cornerRadius, ref colour);
        }

        public static void FillRoundedRectangle(ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Vector3 centerOffset, float cornerRadius,
                                                    ref Color4 colour)
        {
            FillRoundedRectangle(ref position, ref scale, ref rotation, ref centerOffset, cornerRadius, ref colour, ref colour);
        }

        public static void FillRoundedRectangle(ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Vector3 centerOffset, float cornerRadius,
                                                    ref Color4 startColour, ref Color4 endColour)
        {
            if (_checkCull)
            {
                Vector2 minBounds = Vector2.Zero;
                Vector2 maxBounds = Vector2.One;
                if (!InsideScreenClip(ref minBounds, ref maxBounds, ref position, ref scale, ref rotation, ref centerOffset))
                    return;
            }

            _checkCull = false;

            Shape rectangle = ShapeFactory.Rectangle;

            if (cornerRadius <= 0)
            {
                FillShape(rectangle, ref position, ref scale, ref rotation, ref centerOffset, ref startColour, ref endColour);
                return;
            }

            Shape cornerTL = ShapeFactory.RoundedCornerTL;
            Shape cornerTR = ShapeFactory.RoundedCornerTR;
            Shape cornerBL = ShapeFactory.RoundedCornerBL;
            Shape cornerBR = ShapeFactory.RoundedCornerBR;

            cornerRadius = Math.Min(cornerRadius, scale.X / 2);
            _calculateGradientRect = false;
            SetGradientRectangle(position.X + centerOffset.X, position.Y + centerOffset.Y, scale.X, scale.Y);
            Vector2 offset2d = centerOffset.Xy;
            SetGradientOffset(ref offset2d);

            Vector3 cornerPosition = position;
            Vector3 cornerScale = new Vector3(cornerRadius, cornerRadius, scale.Z);
            Vector3 offset = centerOffset;

            FillShape(cornerTL, ref position, ref cornerScale, ref rotation, ref offset, ref startColour, ref endColour);
            cornerPosition.X += scale.X - cornerRadius;
            offset.X += (cornerPosition.X - position.X);
            FillShape(cornerTR, ref position, ref cornerScale, ref rotation, ref offset, ref startColour, ref endColour);
            cornerPosition.Y += scale.Y - cornerRadius;
            offset.Y += (cornerPosition.Y - position.Y);
            FillShape(cornerBR, ref position, ref cornerScale, ref rotation, ref offset, ref startColour, ref endColour);
            offset.X -= (cornerPosition.X - position.X);
            FillShape(cornerBL, ref position, ref cornerScale, ref rotation, ref offset, ref startColour, ref endColour);

            Vector3 rectPosition;
            Vector3 rectScale;
            if (scale.X > cornerRadius * 2)
            {
                rectPosition = new Vector3(position.X + cornerRadius, position.Y, position.Z);
                rectScale = new Vector3(scale.X - (cornerRadius * 2), scale.Y, scale.Z);
                offset = centerOffset + (rectPosition - position);
                FillShape(rectangle, ref position, ref rectScale, ref rotation, ref offset, ref startColour, ref endColour);
            }
            if (scale.Y > cornerRadius * 2)
            {
                rectPosition = new Vector3(position.X, position.Y + cornerRadius, position.Z);
                rectScale = new Vector3(cornerRadius, scale.Y - (cornerRadius * 2), scale.Z);
                offset = centerOffset + (rectPosition - position);
                FillShape(rectangle, ref position, ref rectScale, ref rotation, ref offset, ref startColour, ref endColour);
                rectPosition = new Vector3(position.X + scale.X - cornerRadius, position.Y + cornerRadius, position.Z);
                offset = centerOffset + (rectPosition - position);
                FillShape(rectangle, ref position, ref rectScale, ref rotation, ref offset, ref startColour, ref endColour);
            }

            _calculateGradientRect = true;
            _checkCull = true;
        }

        public static void FillRoundedTexture(Texture texture, ref Vector3 position, ref Vector3 scale, ref Vector3 rotation, ref Vector3 centerOffset,
                                                ref Rectangle source, float cornerRadius, ref Color4 startColour, ref Color4 endColour)
        {
            if (_checkCull)
            {
                Vector2 minBounds = Vector2.Zero;
                Vector2 maxBounds = Vector2.One;
                if (!InsideScreenClip(ref minBounds, ref maxBounds, ref position, ref scale, ref rotation, ref centerOffset))
                    return;
            }

            _checkCull = false;

            Shape rectangle = ShapeFactory.Rectangle;
            if (cornerRadius <= 0)
            {
                FillTexture(texture, rectangle, ref position, ref scale, ref rotation, ref centerOffset, ref source, ref startColour, ref endColour);
                return;
            }

            Shape cornerTL = ShapeFactory.RoundedCornerTL;
            Shape cornerTR = ShapeFactory.RoundedCornerTR;
            Shape cornerBL = ShapeFactory.RoundedCornerBL;
            Shape cornerBR = ShapeFactory.RoundedCornerBR;

            cornerRadius = Math.Min(cornerRadius, scale.X / 2);
            _calculateGradientRect = false;
            SetGradientRectangle(position.X + centerOffset.X, position.Y + centerOffset.Y, scale.X, scale.Y);
            Vector2 offset2d = centerOffset.Xy;
            SetGradientOffset(ref offset2d);

            float xRatio = cornerRadius / scale.X;
            float yRatio = cornerRadius / scale.Y;
            int texWidth = (int)(source.Width * xRatio);
            int texHeight = (int)(source.Height * yRatio);

            Vector3 cornerPosition = position;
            Vector3 cornerScale = new Vector3(cornerRadius, cornerRadius, scale.Z);
            Vector3 offset = centerOffset;
            Rectangle cornerSource = new Rectangle(source.X, source.Y, texWidth, texHeight);

            FillTexture(texture, cornerTL, ref position, ref cornerScale, ref rotation, ref offset, ref cornerSource, ref startColour, ref endColour);
            cornerPosition.X += scale.X - cornerRadius;
            offset.X += (cornerPosition.X - position.X);
            cornerSource.X = source.X + source.Width - texWidth;
            FillTexture(texture, cornerTR, ref position, ref cornerScale, ref rotation, ref offset, ref cornerSource, ref startColour, ref endColour);
            cornerPosition.Y += scale.Y - cornerRadius;
            offset.Y += (cornerPosition.Y - position.Y);
            cornerSource.Y = source.Y + source.Height - texHeight;
            FillTexture(texture, cornerBR, ref position, ref cornerScale, ref rotation, ref offset, ref cornerSource, ref startColour, ref endColour);
            offset.X -= (cornerPosition.X - position.X);
            cornerSource.X = source.X;
            FillTexture(texture, cornerBL, ref position, ref cornerScale, ref rotation, ref offset, ref cornerSource, ref startColour, ref endColour);

            Vector3 rectPosition;
            Vector3 rectScale;
            Rectangle rectSource;
            if (scale.X > cornerRadius * 2)
            {
                rectPosition = new Vector3(position.X + cornerRadius, position.Y, position.Z);
                rectScale = new Vector3(scale.X - (cornerRadius * 2), scale.Y, scale.Z);
                offset = centerOffset + (rectPosition - position);
                rectSource = new Rectangle(source.X + texWidth, source.Y, source.Width - (texWidth * 2), source.Height);
                FillTexture(texture, rectangle, ref position, ref rectScale, ref rotation, ref offset, ref rectSource, ref startColour, ref endColour);
            }
            if (scale.Y > cornerRadius * 2)
            {
                rectPosition = new Vector3(position.X, position.Y + cornerRadius, position.Z);
                rectScale = new Vector3(cornerRadius, scale.Y - (cornerRadius * 2), scale.Z);
                offset = centerOffset + (rectPosition - position);
                rectSource = new Rectangle(source.X, source.Y + texHeight, texWidth, source.Height - (texHeight * 2));
                FillTexture(texture, rectangle, ref position, ref rectScale, ref rotation, ref offset, ref rectSource, ref startColour, ref endColour);
                rectPosition = new Vector3(position.X + scale.X - cornerRadius, position.Y + cornerRadius, position.Z);
                offset = centerOffset + (rectPosition - position);
                rectSource.X = source.X + source.Width - texWidth;
                FillTexture(texture, rectangle, ref position, ref rectScale, ref rotation, ref offset, ref rectSource, ref startColour, ref endColour);
            }

            _checkCull = true;
            _calculateGradientRect = true;
        }

        public static void PrintText(string text, ref Vector3 position, ref Color4 colour)
        {
            PrintText(text, ref position, ref colour, ref colour);
        }

        public static void PrintText(string text, ref Vector3 position, ref Vector3 rotation, ref Color4 colour)
        {
            PrintText(text, ref position, ref rotation, ref colour, ref colour);
        }

        public static void PrintText(string text, ref Vector3 position, ref Color4 startColour, ref Color4 endColour)
        {
            PrintText(text, ref position, ref _zeroVector, ref _zeroVector, ref startColour, ref endColour);
        }

        public static void PrintText(string text, ref Vector3 position, ref Vector3 rotation, ref Color4 startColour, ref Color4 endColour)
        {
            PrintText(text, ref position, ref rotation, ref _zeroVector, ref startColour, ref endColour);
        }

        public static void PrintText(string text, ref Vector3 position, ref Vector3 rotation, ref Vector3 offset, ref Color4 startColour, ref Color4 endColour)
        {
            float offsetX = 0;
            float offsetY = 0;
            int c, prevC;
            TrueTypeFont.FontGlyph glyph = null, prevGlyph = null;
            int size = _currentFont.GetSize();

            string[] lines = text.Split(new char[] { '\n' });

            foreach (string line in lines)
            {
                int lineHeight = _currentFont.GetTextHeight(line);

                for (int i = 0; i < line.Length; i++)
                {
                    c = (int)line[i];
                    if ((char)c == ' ')
                    {
                        offsetX += size / 3;
                    }
                    else
                    {
                        glyph = _currentFont.GetFontGlyph(c);

                        if (glyph.Loaded)
                        {
                            int kerning = 0;
                            if (i > 0)
                            {
                                if (prevGlyph != null)
                                    kerning = (int)_currentFont.GetKerning(prevGlyph.GlpyhID, glyph.GlpyhID);
                            }

                            Vector3 glyphOffset = offset + new Vector3((offsetX + glyph.BearingX) + kerning, lineHeight - glyph.BearingY + offsetY, 0);

                            DrawFontGlyph(glyph, ref position, ref rotation, ref glyphOffset, ref startColour, ref endColour);
                            offsetX += glyph.AdvanceX + kerning + 1;
                        }
                    }
                    prevC = c;
                    prevGlyph = glyph;
                }
                offsetY += lineHeight;
                offsetX = 0;
            }
        }

        private static void DrawFontGlyph(TrueTypeFont.FontGlyph glyph, ref Vector3 position, ref Vector3 rotation, ref Vector3 offset, ref Color4 startColour, ref Color4 endColour)
        {
            if (_checkCull)
            {
                Vector3 cullScale = new Vector3(glyph.RealWidth, glyph.RealHeight, 1);

                Vector2 minBounds = Vector2.Zero;
                Vector2 maxBounds = Vector2.One;
                if (!InsideScreenClip(ref minBounds, ref maxBounds, ref position, ref cullScale, ref rotation, ref offset))
                    return;
            }

            Shape shape = glyph.GlyphRect;

            int textureID = glyph.TextureID;
            Texture.BindTexture(textureID);

            SetColour(ref startColour, ref endColour);

            shape.Bind(_currentShader);

            if (_calculateGradientRect)
            {
                SetGradientRectangle(position.X + offset.X, position.Y + offset.Y, glyph.RealWidth, glyph.RealHeight);
                Vector2 offset2d = offset.Xy;
                SetGradientOffset(ref offset2d);
            }

            ApplyWorldMatrix();

            int uModelLocation = _currentShader.GetUniformLocation("uModel");

            Matrix4 rotX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X));
            Matrix4 rotY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation.Y));
            Matrix4 rotZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z));
            Matrix4 rotationMatrix = rotX * rotY * rotZ;
            SetGradientRotation(rotationMatrix);

            Matrix4 matrix = Matrix4.CreateScale(glyph.RealWidth, glyph.RealHeight, 1) * Matrix4.CreateTranslation(offset) * rotationMatrix * Matrix4.CreateTranslation(position);

            GL.UniformMatrix4(uModelLocation, true, ref matrix);

            GL.DrawElements(PrimitiveType.Polygon, shape.IndicesLength(), DrawElementsType.UnsignedInt, 0);
        }

        public static void DrawPoint(ref Vector3 position, float scale, Color4 colour)
        {
            if (_checkCull)
            {
                Vector3 cullPositon = new Vector3(position.X - (scale / 2), position.Y - (scale / 2), 0);
                Vector3 cullScale = new Vector3(scale, scale, 1);

                Vector2 minBounds = Vector2.Zero;
                Vector2 maxBounds = Vector2.One;
                if (!InsideScreenClip(ref minBounds, ref maxBounds, ref cullPositon, ref cullScale, ref _zeroVector, ref _zeroVector))
                    return;
            }

            Texture.BindNone();
            Shape point = ShapeFactory.Point;
            point.Bind(_currentShader);

            GL.PointSize(scale);

            SetColour(ref colour, ref colour);

            ApplyWorldMatrix();

            int uModelLocation = _currentShader.GetUniformLocation("uModel");
            Matrix4 matrix = Matrix4.CreateTranslation(position);
            GL.UniformMatrix4(uModelLocation, true, ref matrix);

            GL.DrawElements(PrimitiveType.Points, point.IndicesLength(), DrawElementsType.UnsignedInt, 0);

        }

        public static void DrawLine(ref Vector3 startPosition, ref Vector3 endPosition, float width, Color4 colour)
        {
            Vector3 AB = endPosition - startPosition;

            Vector3 scale = new Vector3(AB.Length, width, 1);

            float angle = Vector3.CalculateAngle(AB, Vector3.UnitX);
            if (Vector3.Dot(AB, Vector3.UnitY) < 0)
                angle = -angle;
            Vector3 rot = new Vector3(0, 0, MathHelper.RadiansToDegrees(angle));

            Vector3 offset = new Vector3(0, -width / 2f, 0);
            offset = Vector3.Transform(offset, Matrix4.CreateRotationZ(angle));
            startPosition += offset;

            FillShape(ShapeFactory.Rectangle, ref startPosition, ref scale, ref rot, ref colour);
        }

    }
}
