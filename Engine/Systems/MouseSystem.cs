using Engine.Components;
using Engine.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Systems;

public class MouseSystem : IUpdateSystem
{
    private readonly EntityManager _entityManager;

    public MouseSystem(EntityManager entityManager)
    {
        _entityManager = entityManager;
    }

    public void Update(GameTime gameTime)
    {
        var mouseEntities = _entityManager.GetEntitiesWithComponents(typeof(MouseComponent), typeof(PositionComponent));

        foreach (var mouseEntity in mouseEntities)
        {
            var positionComponent = mouseEntity.GetComponent<PositionComponent>();
            positionComponent.Position = Mouse.GetState().Position.ToVector2();
        }
    }
}
