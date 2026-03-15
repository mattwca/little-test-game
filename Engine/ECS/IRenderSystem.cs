using Microsoft.Xna.Framework;

namespace Engine.ECS;

public interface IRenderSystem
{
    void Draw(GameTime gameTime);
}