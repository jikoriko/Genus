using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Genus2D.Graphics
{
    public class Shape
    {
        private static int _currentVao = -1;

        private int _vaoID = 0;
        private int[] _vboIDs;

        private float[] _vertices;
        private float[] _textureCoords;
        private int[] _indices;

        private Vector2 _textureCoordOffset;
        private Vector2 _textureCoordScale;
        private bool _textureCoordsChanged;

        private Vector2 _boundsMin, _boundsMax;

        private PrimitiveType _primitiveType = PrimitiveType.Polygon;

        public Shape(float[] vertices)
        {
            _vertices = vertices;
            _textureCoords = new float[(vertices.Length / 3) * 2];
            _indices = new int[vertices.Length / 3];

            _boundsMin = new Vector2(vertices[0], vertices[1]);
            _boundsMax = _boundsMin;

            for (int i = 0; i < vertices.Length / 3; i++)
            {
                int index = i * 3;
                if (vertices[index] < _boundsMin.X)
                    _boundsMin.X = vertices[index];
                else if (vertices[index] > _boundsMax.X)
                    _boundsMax.X = vertices[index];
                if (vertices[index + 1] < _boundsMin.Y)
                    _boundsMin.Y = vertices[index + 1];
                else if (vertices[index + 1] > _boundsMax.Y)
                    _boundsMax.Y = vertices[index + 1];

                _textureCoords[i * 2] = vertices[i * 3];
                _textureCoords[(i * 2) + 1] = vertices[(i * 3) + 1];
                _indices[i] = i;
            }

            _textureCoordOffset = new Vector2(0, 0);
            _textureCoordScale = new Vector2(1, 1);
            _textureCoordsChanged = false;
        }

        public Shape(float[] vertices, int[] indices)
        {
            _vertices = vertices;
            _textureCoords = new float[(vertices.Length / 3) * 2];
            _indices = indices;

            _boundsMin = new Vector2(vertices[0], vertices[1]);
            _boundsMax = _boundsMin;

            for (int i = 0; i < vertices.Length / 3; i++)
            {
                int index = i * 3;
                if (vertices[index] < _boundsMin.X)
                    _boundsMin.X = vertices[index];
                else if (vertices[index] > _boundsMax.X)
                    _boundsMax.X = vertices[index];
                if (vertices[index + 1] < _boundsMin.Y)
                    _boundsMin.Y = vertices[index + 1];
                else if (vertices[index + 1] > _boundsMax.Y)
                    _boundsMax.Y = vertices[index + 1];

                _textureCoords[i * 2] = vertices[i * 3];
                _textureCoords[(i * 2) + 1] = vertices[(i * 3) + 1];
            }

            _textureCoordOffset = new Vector2(0, 0);
            _textureCoordScale = new Vector2(1, 1);
            _textureCoordsChanged = false;
        }

        public Shape(float[] vertices, PrimitiveType primitiveType)
        {
            _vertices = vertices;
            _textureCoords = new float[(vertices.Length / 3) * 2];
            _indices = new int[vertices.Length / 3];

            _boundsMin = new Vector2(vertices[0], vertices[1]);
            _boundsMax = _boundsMin;

            for (int i = 0; i < vertices.Length / 3; i++)
            {
                int index = i * 3;
                if (vertices[index] < _boundsMin.X)
                    _boundsMin.X = vertices[index];
                else if (vertices[index] > _boundsMax.X)
                    _boundsMax.X = vertices[index];
                if (vertices[index + 1] < _boundsMin.Y)
                    _boundsMin.Y = vertices[index + 1];
                else if (vertices[index + 1] > _boundsMax.Y)
                    _boundsMax.Y = vertices[index + 1];

                _textureCoords[i * 2] = vertices[i * 3];
                _textureCoords[(i * 2) + 1] = vertices[(i * 3) + 1];
                _indices[i] = i;
            }

            _textureCoordOffset = new Vector2(0, 0);
            _textureCoordScale = new Vector2(1, 1);
            _textureCoordsChanged = false;
            _primitiveType = primitiveType;
        }

        public Shape(float[] vertices, int[] indices, PrimitiveType primitiveType)
        {
            _vertices = vertices;
            _textureCoords = new float[(vertices.Length / 3) * 2];
            _indices = indices;

            _boundsMin = new Vector2(vertices[0], vertices[1]);
            _boundsMax = _boundsMin;

            for (int i = 0; i < vertices.Length / 3; i++)
            {
                int index = i * 3;
                if (vertices[index] < _boundsMin.X)
                    _boundsMin.X = vertices[index];
                else if (vertices[index] > _boundsMax.X)
                    _boundsMax.X = vertices[index];
                if (vertices[index + 1] < _boundsMin.Y)
                    _boundsMin.Y = vertices[index + 1];
                else if (vertices[index + 1] > _boundsMax.Y)
                    _boundsMax.Y = vertices[index + 1];

                _textureCoords[i * 2] = vertices[i * 3];
                _textureCoords[(i * 2) + 1] = vertices[(i * 3) + 1];
            }

            _textureCoordOffset = new Vector2(0, 0);
            _textureCoordScale = new Vector2(1, 1);
            _textureCoordsChanged = false;
            _primitiveType = primitiveType;
        }

        public PrimitiveType GetPrimitiveType()
        {
            return _primitiveType;
        }

        public void SetPrimitiveType(PrimitiveType primitiveType)
        {
            _primitiveType = primitiveType;
        }

        public Vector2 GetMinBounds()
        {
            return _boundsMin;
        }

        public Vector2 GetMaxBounds()
        {
            return _boundsMax;
        }

        public void SetTextureCoordOffset(float x, float y)
        {
            if (_textureCoordOffset.X != x || _textureCoordOffset.Y != y)
            {
                _textureCoordOffset.X = x;
                _textureCoordOffset.Y = y;
                _textureCoordsChanged = true;
            }
        }

        public void SetTextureCoordScale(float w, float h)
        {
            if (_textureCoordScale.X != w || _textureCoordScale.Y != h)
            {
                _textureCoordScale.X = w;
                _textureCoordScale.Y = h;
                _textureCoordsChanged = true;
            }
        }

        private void SetTextureCoords()
        {
            for (int i = 0; i < _vertices.Length / 3; i++)
            {
                float x = _textureCoordOffset.X + (_vertices[i * 3] * _textureCoordScale.X);
                float y = _textureCoordOffset.Y + (_vertices[(i * 3) + 1] * _textureCoordScale.Y);

                _textureCoords[i * 2] = x;
                _textureCoords[(i * 2) + 1] = y;
            }
        }

        public int IndicesLength()
        {
            return _indices.Length;
        }


        public void Bind(Shader shader)
        {
            if (_vaoID == 0)
            {
                _vaoID = GL.GenVertexArray();
                _vboIDs = new int[3];
                GL.GenBuffers(3, _vboIDs);

                GL.BindVertexArray(_vaoID);
                _currentVao = _vaoID;

                BindVertices();
                int vPositionLocation = shader.GetAttribLocation("vPosition");
                GL.EnableVertexAttribArray(vPositionLocation);
                GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

                BindTextureCoords();
                int vTexCoordsLocation = shader.GetAttribLocation("vTexCoords");
                GL.EnableVertexAttribArray(vTexCoordsLocation);
                GL.VertexAttribPointer(vTexCoordsLocation, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

                BindIndices();
            }
            else if (_currentVao != _vaoID)
            {
                GL.BindVertexArray(_vaoID);
                _currentVao = _vaoID;
            }
            if (_textureCoordsChanged)
            {
                SetTextureCoords();
                BindTextureCoords();
            }
        }

        private void BindVertices()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboIDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_vertices.Length * sizeof(float)), _vertices, BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (_vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }
        }

        private void BindTextureCoords()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboIDs[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_textureCoords.Length * sizeof(float)), _textureCoords, BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (_textureCoords.Length * sizeof(float) != size)
            {
                throw new ApplicationException("texture coordinate data not loaded onto graphics card correctly");
            }
        }

        private void BindIndices()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _vboIDs[2]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_indices.Length * sizeof(int)), _indices, BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (_indices.Length * sizeof(int) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }
        }

        public void Destroy()
        {
            if (_vaoID != 0)
            {
                if (_currentVao == _vaoID)
                    _currentVao = -1;
                GL.DeleteBuffers(3, _vboIDs);
                _vboIDs = null;
                GL.DeleteVertexArray(_vaoID);
                _vaoID = 0;
            }
        }

    }
}
