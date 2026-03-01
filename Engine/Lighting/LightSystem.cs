using System.Linq;

using Engine.Components;
using Engine.ECS;

using Microsoft.Xna.Framework.Graphics;

namespace Engine.Lighting;

public class LightSystem : IRenderSystem, IUpdateSystem
{
    private readonly SpriteBatch _spriteBatch;

    public LightSystem(SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;
    }

    public void Draw(EntityManager entityManager, float deltaTime)
    {
        var lightEntity = entityManager.GetEntitiesWithComponents(typeof(LightComponent), typeof(PositionComponent)).FirstOrDefault();
        if (lightEntity == null)
            return;
    }

    public void Update(EntityManager entityManager, float deltaTime)
    {
        // TODO
    }
}