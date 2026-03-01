using System.Linq;

using Engine.Components;
using Engine.ECS;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class PlayerSystem : IUpdateSystem
{
    public PlayerSystem() { }

    public void Update(EntityManager entityManager, float deltaTime)
    {
        var playerEntity = entityManager.GetEntitiesWithComponent<PlayerComponent>().First();
        var positionComponent = playerEntity.GetComponent<PositionComponent>();

        var movementVector = new Vector2();

        if (Keyboard.GetState().IsKeyDown(Keys.W))
        {
            movementVector.Y -= 1;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.S))
        {
            movementVector.Y += 1;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            movementVector.X -= 1;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            movementVector.X += 1;
        }

        positionComponent.Position += movementVector * deltaTime * 100;
    }
}