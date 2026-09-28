namespace Riten_Tutorial.src;

public class BlockList
{
    public static List<Block> Blocks = new List<Block>();

    public void Awake()
    {
        // Dirt Block
        Block dirt = new Block("Dirt", false, 2, 0);
        Blocks.Add(dirt);

        // Grass Block
        Blocks.Add(new Block("Grass", false, 0, 0, 3, 0, 2, 0));
    }
}
