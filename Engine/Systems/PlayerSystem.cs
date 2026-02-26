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
        var renderComponent = playerEntity.GetComponent<RenderingComponent>();

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

        renderComponent.Position += movementVector * deltaTime * 100;
    }
}