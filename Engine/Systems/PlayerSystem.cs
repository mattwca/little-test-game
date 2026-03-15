using System.Linq;

using Engine.Components;
using Engine.ECS;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class PlayerSystem : IUpdateSystem
{
    private readonly EntityManager _entityManager;

    public PlayerSystem(EntityManager entityManager)
    {
        _entityManager = entityManager;
    }

    public void Update(GameTime gameTime)
    {
        var playerEntity = _entityManager.GetEntitiesWithComponent<PlayerComponent>().First();
        var positionComponent = playerEntity.GetComponent<PositionComponent>();
        var animationComponent = playerEntity.GetComponent<AnimationComponent>();

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

        animationComponent.Enabled = movementVector != Vector2.Zero;
        positionComponent.Position += movementVector * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
    }
}