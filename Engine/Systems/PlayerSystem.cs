using System;
using System.Linq;

using Engine.Components;
using Engine.ECS;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class PlayerSystem : IUpdateSystem
{
    private readonly StateManager _stateManager;
    private readonly EntityManager _entityManager;

    public PlayerSystem(StateManager stateManager, EntityManager entityManager)
    {
        _stateManager = stateManager;
        _entityManager = entityManager;
    }

    public void Update(GameTime gameTime)
    {
        this.HandlePlayerMovement(gameTime);
        this.HandleOtherInput();
    }

    private void HandleOtherInput()
    {
        if (Keyboard.GetState().IsKeyDown(Keys.OemTilde))
        {
            _stateManager.SetBool("debugMode", true);
        }
    }

    private void HandlePlayerMovement(GameTime gameTime)
    {
        var playerEntity = _entityManager.GetEntitiesWithComponent<PlayerComponent>().First();
        var positionComponent = playerEntity.GetComponent<PositionComponent>();
        var animationComponent = playerEntity.GetComponent<AnimationComponent>();
        var renderingComponent = playerEntity.GetComponent<RenderingComponent>();

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
            renderingComponent.FlipX = false;
        }
        
        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            movementVector.X += 1;
            renderingComponent.FlipX = true;
        }

        animationComponent.Enabled = movementVector != Vector2.Zero;
        positionComponent.Position += movementVector * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
    }
}