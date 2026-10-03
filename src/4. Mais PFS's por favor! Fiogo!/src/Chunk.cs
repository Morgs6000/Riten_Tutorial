using System.Collections;
using System.Drawing;
using System.Numerics;
using System.Reflection.Metadata;

namespace Riten_Tutorial.src;

public class Chunk : IDisposable
{
    public Vector3 Position;

    public Block[,,] map = null!;
    public static uint Width = 20, Height = 10;

    public static List<Chunk> Chunks = new List<Chunk>();
    int ChunkID = 0;

    List<Vector3> vertices = new List<Vector3>();
    List<uint> triangulos = new List<uint>();
    List<Vector2> uvs = new List<Vector2>();

    float TextureOffset = 1.0f / 16.0f;

    Vertex vertex = null!;

    private Mesh _mesh = null!;

    public static bool working = false;
    bool ready = false;

    public void Start()
    {
        Chunks.Add(this);
    }

    public void Update()
    {
        if (working == false && ready == false)
        {
            ready = true;
            StartFunction();
        }
    }

    public void StartFunction()
    {
        working = true;

        vertex = new Vertex();

        map = new Block[Width, Height, Width];

        var e1 = CalculateMap();
        while (e1.MoveNext()) { }
    }

    public IEnumerator CalculateMap()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Width; z++)
                {
                    if (y < 5)
                    {
                        map[x, y, z] = Block.GetBlock("Dirt");
                    }
                    if (y == 5 && Random.Range(0, 5) == 1)
                    {
                        map[x, y, z] = Block.GetBlock("Grass");
                    }
                }
            }
        }

        yield return 0;

        var e2 = CalculateMesh();
        while (e2.MoveNext()) { }
    }

    public IEnumerator CalculateMesh()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Width; z++)
                {
                    if (map[x, y, z] != null)
                    {
                        if (IsBlockTransparent(x - 1, y, z))
                        {
                            AddCubeLeft(x, y, z, map[x, y, z]);
                        }
                        if (IsBlockTransparent(x + 1, y, z))
                        {
                            AddCubeRight(x, y, z, map[x, y, z]);
                        }
                        if (IsBlockTransparent(x, y - 1, z))
                        {
                            AddCubeBottom(x, y, z, map[x, y, z]);
                        }
                        if (IsBlockTransparent(x, y + 1, z))
                        {
                            AddCubeTop(x, y, z, map[x, y, z]);
                        }
                        if (IsBlockTransparent(x, y, z - 1))
                        {
                            AddCubeBack(x, y, z, map[x, y, z]);
                        }
                        if (IsBlockTransparent(x, y, z + 1))
                        {
                            AddCubeFront(x, y, z, map[x, y, z]);
                        }
                    }
                }
            }
        }

        vertex.Positions = vertices.ToArray();
        vertex.Indices = triangulos.ToArray();
        vertex.TexCoords = uvs.ToArray();

        _mesh = new Mesh(vertex);

        yield return 0;

        working = false;
    }

    public void AddCubeLeft(int x, int y, int z, Block b)
    {
        // x = x + (int)MathF.Floor(Position.X);
        // y = y + (int)MathF.Floor(Position.Y);
        // z = z + (int)MathF.Floor(Position.Z);

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(1 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));
        triangulos.Add((uint)(3 + vertices.Count));

        float u0 = TextureOffset * b.TextureXSide;
        float v0 = TextureOffset * b.TextureYSide;
        
        float u1 = u0 + TextureOffset;
        float v1 = v0 + TextureOffset;

        uvs.Add(new Vector2(u0, v1));
        uvs.Add(new Vector2(u1, v1));
        uvs.Add(new Vector2(u1, v0));
        uvs.Add(new Vector2(u0, v0));

        float x0 = x + 0.0f;
        float y0 = y + 0.0f;
        float z0 = z + 0.0f;

        float x1 = x + 1.0f;
        float y1 = y + 1.0f;
        float z1 = z + 1.0f;

        vertices.Add(new Vector3(x0, y0, z0));
        vertices.Add(new Vector3(x0, y0, z1));
        vertices.Add(new Vector3(x0, y1, z1));
        vertices.Add(new Vector3(x0, y1, z0));
    }

    public void AddCubeRight(int x, int y, int z, Block b)
    {
        // x = x + (int)MathF.Floor(Position.X);
        // y = y + (int)MathF.Floor(Position.Y);
        // z = z + (int)MathF.Floor(Position.Z);

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(1 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));
        triangulos.Add((uint)(3 + vertices.Count));

        float u0 = TextureOffset * b.TextureXSide;
        float v0 = TextureOffset * b.TextureYSide;
        
        float u1 = u0 + TextureOffset;
        float v1 = v0 + TextureOffset;

        uvs.Add(new Vector2(u0, v1));
        uvs.Add(new Vector2(u1, v1));
        uvs.Add(new Vector2(u1, v0));
        uvs.Add(new Vector2(u0, v0));

        float x0 = x + 0.0f;
        float y0 = y + 0.0f;
        float z0 = z + 0.0f;

        float x1 = x + 1.0f;
        float y1 = y + 1.0f;
        float z1 = z + 1.0f;

        vertices.Add(new Vector3(x1, y0, z1));
        vertices.Add(new Vector3(x1, y0, z0));
        vertices.Add(new Vector3(x1, y1, z0));
        vertices.Add(new Vector3(x1, y1, z1));
    }

    public void AddCubeBottom(int x, int y, int z, Block b)
    {
        // x = x + (int)MathF.Floor(Position.X);
        // y = y + (int)MathF.Floor(Position.Y);
        // z = z + (int)MathF.Floor(Position.Z);

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(1 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));
        triangulos.Add((uint)(3 + vertices.Count));

        float u0 = TextureOffset * b.TextureXBottom;
        float v0 = TextureOffset * b.TextureYBottom;
        
        float u1 = u0 + TextureOffset;
        float v1 = v0 + TextureOffset;

        uvs.Add(new Vector2(u0, v1));
        uvs.Add(new Vector2(u1, v1));
        uvs.Add(new Vector2(u1, v0));
        uvs.Add(new Vector2(u0, v0));

        float x0 = x + 0.0f;
        float y0 = y + 0.0f;
        float z0 = z + 0.0f;

        float x1 = x + 1.0f;
        float y1 = y + 1.0f;
        float z1 = z + 1.0f;

        vertices.Add(new Vector3(x0, y0, z0));
        vertices.Add(new Vector3(x1, y0, z0));
        vertices.Add(new Vector3(x1, y0, z1));
        vertices.Add(new Vector3(x0, y0, z1));
    }

    public void AddCubeTop(int x, int y, int z, Block b)
    {
        // x = x + (int)MathF.Floor(Position.X);
        // y = y + (int)MathF.Floor(Position.Y);
        // z = z + (int)MathF.Floor(Position.Z);

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(1 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));
        triangulos.Add((uint)(3 + vertices.Count));

        float u0 = TextureOffset * b.TextureX;
        float v0 = TextureOffset * b.TextureY;
        
        float u1 = u0 + TextureOffset;
        float v1 = v0 + TextureOffset;
        
        uvs.Add(new Vector2(u0, v1));
        uvs.Add(new Vector2(u1, v1));
        uvs.Add(new Vector2(u1, v0));
        uvs.Add(new Vector2(u0, v0));

        float x0 = x + 0.0f;
        float y0 = y + 0.0f;
        float z0 = z + 0.0f;

        float x1 = x + 1.0f;
        float y1 = y + 1.0f;
        float z1 = z + 1.0f;

        vertices.Add(new Vector3(x0, y1, z1));
        vertices.Add(new Vector3(x1, y1, z1));
        vertices.Add(new Vector3(x1, y1, z0));
        vertices.Add(new Vector3(x0, y1, z0));
    }

    public void AddCubeBack(int x, int y, int z, Block b)
    {
        // x = x + (int)MathF.Floor(Position.X);
        // y = y + (int)MathF.Floor(Position.Y);
        // z = z + (int)MathF.Floor(Position.Z);

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(1 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));
        triangulos.Add((uint)(3 + vertices.Count));

        float u0 = TextureOffset * b.TextureXSide;
        float v0 = TextureOffset * b.TextureYSide;
        
        float u1 = u0 + TextureOffset;
        float v1 = v0 + TextureOffset;

        uvs.Add(new Vector2(u0, v1));
        uvs.Add(new Vector2(u1, v1));
        uvs.Add(new Vector2(u1, v0));
        uvs.Add(new Vector2(u0, v0));

        float x0 = x + 0.0f;
        float y0 = y + 0.0f;
        float z0 = z + 0.0f;

        float x1 = x + 1.0f;
        float y1 = y + 1.0f;
        float z1 = z + 1.0f;

        vertices.Add(new Vector3(x1, y0, z0));
        vertices.Add(new Vector3(x0, y0, z0));
        vertices.Add(new Vector3(x0, y1, z0));
        vertices.Add(new Vector3(x1, y1, z0));
    }

    public void AddCubeFront(int x, int y, int z, Block b)
    {
        // x = x + (int)MathF.Floor(Position.X);
        // y = y + (int)MathF.Floor(Position.Y);
        // z = z + (int)MathF.Floor(Position.Z);

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(1 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));

        triangulos.Add((uint)(0 + vertices.Count));
        triangulos.Add((uint)(2 + vertices.Count));
        triangulos.Add((uint)(3 + vertices.Count));

        float u0 = TextureOffset * b.TextureXSide;
        float v0 = TextureOffset * b.TextureYSide;
        
        float u1 = u0 + TextureOffset;
        float v1 = v0 + TextureOffset;

        uvs.Add(new Vector2(u0, v1));
        uvs.Add(new Vector2(u1, v1));
        uvs.Add(new Vector2(u1, v0));
        uvs.Add(new Vector2(u0, v0));

        float x0 = x + 0.0f;
        float y0 = y + 0.0f;
        float z0 = z + 0.0f;

        float x1 = x + 1.0f;
        float y1 = y + 1.0f;
        float z1 = z + 1.0f;

        vertices.Add(new Vector3(x0, y0, z1));
        vertices.Add(new Vector3(x1, y0, z1));
        vertices.Add(new Vector3(x1, y1, z1));
        vertices.Add(new Vector3(x0, y1, z1));
    }

    bool IsBlockTransparent(int x, int y, int z)
    {
        if (x >= Width  || x < 0 ||
            y >= Height || y < 0 ||
            z >= Width  || z < 0)
        {
            return true;
        }
        if (map[x, y, z] == null)
        {
            return true;
        }

        return false;
    }

    public static Chunk? GetChunk(int x, int y, int z)
    {
        for (int i = 0; i < Chunks.Count; i++)
        {
            Vector3 pos = new Vector3(x, y, z);
            Vector3 cpos = Chunks[i].Position;

            if (cpos.Equals(pos))
            {
                return Chunks[i];
            }

            if (pos.X < cpos.X || pos.X >= cpos.X + Width ||
                pos.Y < cpos.Y || pos.Y >= cpos.Y + Height ||
                pos.Z < cpos.Z || pos.Z >= cpos.Z + Width)
            {
                continue;
            }

            return Chunks[i];
        }

        return null;
    }

    public void Draw(Shader shader)
    {
        Matrix4x4 model = Matrix4x4.Identity;
        model *= Matrix4x4.CreateTranslation(Position);
        shader.SetMat4("model", model);

        _mesh.Draw(shader);
    }

    public void Dispose()
    {
        _mesh.Dispose();
    }
}

public class Block
{
    public string BlockName = "";

    public int BlockID;

    public bool Trasnsparent = false;

    public int TextureX;
    public int TextureY;

    public int TextureXSide;
    public int TextureYSide;

    public int TextureXBottom;
    public int TextureYBottom;

    public bool BlockGlow;
    public Color BlockColor = Color.White;

    public Block()
    {
        BlockID = -1;
        Trasnsparent = true;
    }

    public Block(string name, bool transparent, int tX, int tY)
    {
        Trasnsparent = transparent;
        BlockName = name;

        BlockID = BlockList.Blocks.Count();

        TextureX = tX;
        TextureY = tY;

        TextureXSide = tX;
        TextureYSide = tY;

        TextureXBottom = tX;
        TextureYBottom = tY;
    }

    public Block(string name, bool transparent, int tX, int tY, int sX, int sY)
    {
        Trasnsparent = transparent;
        BlockName = name;

        BlockID = BlockList.Blocks.Count;

        TextureX = tX;
        TextureY = tY;

        TextureXSide = sX;
        TextureYSide = sY;

        TextureXBottom = tX;
        TextureYBottom = tY;
    }

    public Block(string name, bool transparent, int tX, int tY, int sX, int sY, int bX, int bY)
    {
        Trasnsparent = transparent;
        BlockName = name;

        BlockID = BlockList.Blocks.Count;

        TextureX = tX;
        TextureY = tY;

        TextureXSide = sX;
        TextureYSide = sY;

        TextureXBottom = bX;
        TextureYBottom = bY;
    }
    
    public void SetColor(Color color, bool glow)
    {
        BlockGlow = glow;
        BlockColor = color;
    }

    public static Block GetBlock(string name)
    {
        foreach (Block b in BlockList.Blocks)
        {
            if (b.BlockName == name)
            {
                return b;
            }
        }

        return new Block();
    }
}
