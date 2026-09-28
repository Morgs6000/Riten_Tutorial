using System.Numerics;

namespace Riten_Tutorial.src;

public class Chunk : IDisposable
{
    public Vector3 Position;

    public uint[,,] map = null!;
    public uint Widht = 20, Height = 10;

    List<Vector3> vertices = new List<Vector3>();
    List<uint> triangulos = new List<uint>();

    Vertex vertex = null!;

    private Mesh _mesh = null!;

    public void Start()
    {
        vertex = new Vertex();

        map = new uint[20, 10, 20];

        for (int x = 0; x < Widht; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Widht; z++)
                {
                    if (y < 5)
                    {
                        map[x, y, z] = 1;
                    }
                }
            }
        }

        CalculateMesh();
    }

    public void CalculateMesh()
    {
        for (int x = 0; x < Widht; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Widht; z++)
                {
                    if (y < 5)
                    {
                        if (map[x, y, z] != 0)
                        {
                            if (IsBlockTransparent(x - 1, y, z))
                            {
                                AddCubeLeft(x, y, z);
                            }
                            if (IsBlockTransparent(x + 1, y, z))
                            {
                                AddCubeRight(x, y, z);
                            }
                            if (IsBlockTransparent(x, y - 1, z))
                            {
                                AddCubeBottom(x, y, z);
                            }
                            if (IsBlockTransparent(x, y + 1, z))
                            {
                                AddCubeTop(x, y, z);
                            }
                            if (IsBlockTransparent(x, y, z - 1))
                            {
                                AddCubeBack(x, y, z);
                            }
                            if (IsBlockTransparent(x, y, z + 1))
                            {
                                AddCubeFront(x, y, z);
                            }
                        }
                    }
                }
            }
        }


        vertex.Positions = vertices.ToArray();
        vertex.Indices = triangulos.ToArray();

        _mesh = new Mesh(vertex);
    }

    public void AddCubeLeft(int x, int y, int z)
    {
        x = x + (int)MathF.Floor(Position.X);
        y = y + (int)MathF.Floor(Position.Y);
        z = z + (int)MathF.Floor(Position.Z);

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(1 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));
        triangulos.Add((uint)(3 + vertices.Count));

        vertices.Add(new Vector3(x + 0.0f, y + 0.0f, z + 0.0f));
        vertices.Add(new Vector3(x + 0.0f, y + 0.0f, z + 1.0f));
        vertices.Add(new Vector3(x + 0.0f, y + 1.0f, z + 1.0f));
        vertices.Add(new Vector3(x + 0.0f, y + 1.0f, z + 0.0f));
    }

    public void AddCubeRight(int x, int y, int z)
    {
        x = x + (int)MathF.Floor(Position.X);
        y = y + (int)MathF.Floor(Position.Y);
        z = z + (int)MathF.Floor(Position.Z);

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(1 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));
        triangulos.Add((uint)(3 + vertices.Count));

        vertices.Add(new Vector3(x + 1.0f, y + 0.0f, z + 1.0f));
        vertices.Add(new Vector3(x + 1.0f, y + 0.0f, z + 0.0f));
        vertices.Add(new Vector3(x + 1.0f, y + 1.0f, z + 0.0f));
        vertices.Add(new Vector3(x + 1.0f, y + 1.0f, z + 1.0f));
    }

    public void AddCubeBottom(int x, int y, int z)
    {
        x = x + (int)MathF.Floor(Position.X);
        y = y + (int)MathF.Floor(Position.Y);
        z = z + (int)MathF.Floor(Position.Z);

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(1 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));
        triangulos.Add((uint)(3 + vertices.Count));

        vertices.Add(new Vector3(x + 0.0f, y + 0.0f, z + 0.0f));
        vertices.Add(new Vector3(x + 1.0f, y + 0.0f, z + 0.0f));
        vertices.Add(new Vector3(x + 1.0f, y + 0.0f, z + 1.0f));
        vertices.Add(new Vector3(x + 0.0f, y + 0.0f, z + 1.0f));
    }

    public void AddCubeTop(int x, int y, int z)
    {
        x = x + (int)MathF.Floor(Position.X);
        y = y + (int)MathF.Floor(Position.Y);
        z = z + (int)MathF.Floor(Position.Z);

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(1 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));
        triangulos.Add((uint)(3 + vertices.Count));

        vertices.Add(new Vector3(x + 0.0f, y + 1.0f, z + 1.0f));
        vertices.Add(new Vector3(x + 1.0f, y + 1.0f, z + 1.0f));
        vertices.Add(new Vector3(x + 1.0f, y + 1.0f, z + 0.0f));
        vertices.Add(new Vector3(x + 0.0f, y + 1.0f, z + 0.0f));
    }

    public void AddCubeBack(int x, int y, int z)
    {
        x = x + (int)MathF.Floor(Position.X);
        y = y + (int)MathF.Floor(Position.Y);
        z = z + (int)MathF.Floor(Position.Z);

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(1 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));
        triangulos.Add((uint)(3 + vertices.Count));

        vertices.Add(new Vector3(x + 1.0f, y + 0.0f, z + 0.0f));
        vertices.Add(new Vector3(x + 0.0f, y + 0.0f, z + 0.0f));
        vertices.Add(new Vector3(x + 0.0f, y + 1.0f, z + 0.0f));
        vertices.Add(new Vector3(x + 1.0f, y + 1.0f, z + 0.0f));
    }

    public void AddCubeFront(int x, int y, int z)
    {
        x = x + (int)MathF.Floor(Position.X);
        y = y + (int)MathF.Floor(Position.Y);
        z = z + (int)MathF.Floor(Position.Z);

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(1 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));
        triangulos.Add((uint)(3 + vertices.Count));

        vertices.Add(new Vector3(x + 0.0f, y + 0.0f, z + 1.0f));
        vertices.Add(new Vector3(x + 1.0f, y + 0.0f, z + 1.0f));
        vertices.Add(new Vector3(x + 1.0f, y + 1.0f, z + 1.0f));
        vertices.Add(new Vector3(x + 0.0f, y + 1.0f, z + 1.0f));
    }

    bool IsBlockTransparent(int x, int y, int z)
    {
        if (x >= Widht  || x < 0 ||
            y >= Height || y < 0 ||
            z >= Widht  || z < 0)
        {
            return true;
        }
        if (map[x, y, z] == 0)
        {
            return true;
        }

        return false;
    }

    public void Draw(Shader shader)
    {
        _mesh.Draw(shader);
    }

    public void Dispose()
    {
        _mesh.Dispose();
    }
}
