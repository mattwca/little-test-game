namespace Engine.Configuration;

public class GameConfiguration(int worldWidth, int worldHeight)
{
    public int WorldWidth { get; set; } = worldWidth;
    public int WorldHeight { get; set; } = worldHeight;
}
