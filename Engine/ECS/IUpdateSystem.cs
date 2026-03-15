using Microsoft.Xna.Framework;

namespace Engine.ECS;

public interface IUpdateSystem
{
    void Update(GameTime gameTime);
}