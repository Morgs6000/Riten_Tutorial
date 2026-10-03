using System.Numerics;

namespace Riten_Tutorial.src;

public class PlayerController
{
    public Vector3 Position;

    // public Chunk chunkPrefab = null!;
    public int viewRange = 30;

    public void Update(Shader shader)
    {
        for (float x = Position.X - viewRange; x < Position.X + viewRange; x += Chunk.Width)
        {
            for (float z = Position.Z - viewRange; z < Position.Z + viewRange; z += Chunk.Width)
            {
                int xx = (int)(MathF.Floor(x / Chunk.Width) * Chunk.Width);
                int zz = (int)(MathF.Floor(z / Chunk.Width) * Chunk.Width);

                Chunk chunk = Chunk.GetChunk(
                    (int)MathF.Floor(xx),
                    0,
                    (int)MathF.Floor(zz)
                )!;

                if (chunk == null)
                {
                    chunk = new Chunk();
                    chunk.Position = new Vector3(xx, 0.0f, zz);
                    chunk.Start();
                }

                chunk.Update();
                chunk.Draw(shader);
            }
        }
    }
}
