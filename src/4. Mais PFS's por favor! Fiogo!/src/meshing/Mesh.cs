using Silk.NET.OpenGL;
using Program = Riten_Tutorial.src.Program;

public class Mesh : IDisposable
{
    private GL _gl = Program.GL;
    
    // Dados da malha
    public float[] Vertices = [];
    public uint[] Indices = [];
    public uint VertexArrayObject;

    // construtor
    public Mesh(Vertex vertex)
    {
        _useColor = vertex.Colors.Length > 0;
        _useTexture = vertex.TexCoords.Length > 0;

        // --------------------------------------------------

        _vertexStride = _sizePos;

        if (_useColor)
        {
            _vertexStride += _sizeColor;
        }
        if (_useTexture)
        {
            _vertexStride += _sizeTex;
        }

        // --------------------------------------------------

        _vertexPointer = 0;

        _vertexCount = (uint)vertex.Positions.Length;

        Vertices = new float[_vertexCount * _vertexStride];

        for (int i = 0; i < _vertexCount; i++)
        {
            Vertices[_vertexPointer++] = vertex.Positions[i].X;
            Vertices[_vertexPointer++] = vertex.Positions[i].Y;
            Vertices[_vertexPointer++] = vertex.Positions[i].Z;

            if (_useColor)
            {
                Vertices[_vertexPointer++] = vertex.Colors[i].R / 255.0f;
                Vertices[_vertexPointer++] = vertex.Colors[i].G / 255.0f;
                Vertices[_vertexPointer++] = vertex.Colors[i].B / 255.0f;
                Vertices[_vertexPointer++] = vertex.Colors[i].A / 255.0f;
            }
            if (_useTexture)
            {
                Vertices[_vertexPointer++] = vertex.TexCoords[i].X;
                Vertices[_vertexPointer++] = vertex.TexCoords[i].Y;
            }
        }

        // --------------------------------------------------

        _indexCount = (uint)vertex.Indices.Length;

        Indices = new uint[_indexCount];

        for (int i = 0; i < _indexCount; i++)
        {
            Indices[i] = vertex.Indices[i];
        }

        // --------------------------------------------------

        // agora que temos todos os dados necessários, defina os buffers de vértices e seus ponteiros de atributos.
        SetupMesh();
    }

    // renderizar a malha
    public void Draw(Shader shader)
    {
        shader.SetBool("useColor", _useColor);
        shader.SetBool("useTexture", _useTexture);

        // desenhar malha
        _gl.BindVertexArray(VertexArrayObject);

        DrawArraysFill(shader);
        DrawArraysLine(shader);

        _gl.BindVertexArray(0);
    }

    // renderizar dados
    private uint _vertexBufferObject;
    private uint _elementBufferObject;

    private uint _vertexStride;
    private uint _vertexPointer;

    private uint _vertexCount;
    private uint _indexCount;

    private const uint _indexPos = 0;
    private const uint _indexColor = 1;
    private const uint _indexTex = 2;

    private const int _sizePos = 3;
    private const int _sizeColor = 4;
    private const int _sizeTex = 2;

    private bool _useColor = false;
    private bool _useTexture = false;

    private void DrawArraysFill(Shader shader)
    {
        if (Program.ShadingMode == ShadingMode.Shaded ||
            Program.ShadingMode == ShadingMode.Shaded_Wireframe)
        {
            if (_useColor)
            {
                _gl.EnableVertexAttribArray(_indexColor);
            }
            if (_useTexture)
            {
                _gl.EnableVertexAttribArray(_indexTex);
            }

            _gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);

            shader.SetBool("useWireframe", false);

            DrawArrays();
        }
    }

    private void DrawArraysLine(Shader shader)
    {
        if (Program.ShadingMode == ShadingMode.Wireframe ||
            Program.ShadingMode == ShadingMode.Shaded_Wireframe)
        {
            if (_useColor)
            {
                _gl.DisableVertexAttribArray(_indexColor);
            }
            if (_useTexture)
            {
                _gl.DisableVertexAttribArray(_indexTex);
            }

            _gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);

            _gl.Enable(GLEnum.PolygonOffsetLine);
            _gl.PolygonOffset(-1.0f, -1.0f);

            shader.SetBool("useWireframe", true);

            DrawArrays();

            _gl.Disable(GLEnum.PolygonOffsetLine);
        }
    }

    private unsafe void DrawArrays()
    {
        if (_indexCount > 0)
        {
            _gl.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, (void*)0);
        }
        else
        {
            _gl.DrawArrays(PrimitiveType.Triangles, 0, _vertexCount);
        }
    }

    // inicializa todos os objetos/arrays de buffer
    private void SetupMesh()
    {
        // criar buffers/arrays
        _gl.GenVertexArrays(1, out VertexArrayObject);
        _gl.BindVertexArray(VertexArrayObject);

        _vertexBufferObject = BufferObject(BufferTargetARB.ArrayBuffer, Vertices);

        if (_indexCount > 0)
        {
            _elementBufferObject = BufferObject(BufferTargetARB.ElementArrayBuffer, Indices);
        }

        // define os ponteiros de atributos de vértice
        uint stride = _vertexStride;
        int pointer = 0;

        // position attribute
        VertexAttribPointer(_indexPos, _sizePos, stride, pointer);
        pointer += _sizePos;

        // color attribute
        if (_useColor)
        {
            VertexAttribPointer(_indexColor, _sizeColor, stride, pointer);
            pointer += _sizeColor;
        }

        // texture attribute
        if (_useTexture)
        {
            VertexAttribPointer(_indexTex, _sizeTex, stride, pointer);
        }

        _gl.BindVertexArray(0);
    }

    private unsafe uint BufferObject<T>(BufferTargetARB target, Span<T> data) where T : unmanaged
    {
        uint buffer;
        _gl.GenBuffers(1, out buffer);

        _gl.BindBuffer(target, buffer);
        fixed (void* buf = data)
        {
            _gl.BufferData(target, (uint)(data.Length * sizeof(T)), buf, BufferUsageARB.StaticDraw);
        }

        return buffer;
    }

    private unsafe void VertexAttribPointer(uint index, int size, uint stride, int pointer)
    {
        _gl.EnableVertexAttribArray(index);
        _gl.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, stride * sizeof(float), (void*)(pointer * sizeof(float)));
    }

    public void Dispose()
    {
        _gl.DeleteVertexArrays(1, ref VertexArrayObject);
        _gl.DeleteBuffers(1, ref _vertexBufferObject);

        if (_indexCount > 0)
        {
            _gl.DeleteBuffers(1, ref _elementBufferObject);
        }
    }
}
