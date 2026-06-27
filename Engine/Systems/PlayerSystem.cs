using System;
using System.Linq;
using Engine.Components;
using Engine.ECS;
using Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class PlayerSystem : IUpdateSystem
{
    private readonly StateManager _stateManager;
    private readonly EntityManager _entityManager;
    private readonly KeyboardHandler _keyboardHandler;
    private readonly Texture2D _bulletTexture;

    public PlayerSystem(StateManager stateManager, EntityManager entityManager, ContentManager contentManager)
    {
        _stateManager = stateManager;
        _entityManager = entityManager;
        _keyboardHandler = new KeyboardHandler();

        _bulletTexture = contentManager.Load<Texture2D>("bullet");
    }

    public void Update(GameTime gameTime)
    {
        _keyboardHandler.OnUpdate();

        HandlePlayerMovement(gameTime);
        HandleOtherInput();
    }

    private void HandleOtherInput()
    {
        if (_keyboardHandler.WasKeyPressed(Keys.OemTilde))
        {
            _stateManager.ToggleBool("debugModeEnabled");
        }

        if (_keyboardHandler.WasKeyPressed(Keys.P))
        {
            _stateManager.ToggleBool("renderLightingMap");
        }
    }

    private void HandlePlayerMovement(GameTime gameTime)
    {
        var playerEntity = _entityManager.GetEntitiesWithComponent<PlayerComponent>().First();
        var playerComponent = playerEntity.GetComponent<PlayerComponent>();
        var positionComponent = playerEntity.GetComponent<PositionComponent>();
        var animationComponent = playerEntity.GetComponent<AnimationComponent>();
        var renderingComponent = playerEntity.GetComponent<RenderingComponent>();

        var movementVector = new Vector2();

        if (Keyboard.GetState().IsKeyDown(Keys.W))
        {
            movementVector.Y -= 1;
            playerComponent.Direction = new Vector2(0, 1);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.S))
        {
            movementVector.Y += 1;
            playerComponent.Direction = new Vector2(0, -1);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            movementVector.X -= 1;
            renderingComponent.FlipX = false;
            playerComponent.Direction = new Vector2(-1, 0);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            movementVector.X += 1;
            renderingComponent.FlipX = true;
            playerComponent.Direction = new Vector2(1, 0);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Space))
        {
            _entityManager
                .CreateEntity($"bullet:{Guid.NewGuid().ToString().Replace("-", "")}")
                .AddComponent(new PositionComponent(positionComponent.Position, 16, 16))
                .AddComponent(new RenderingComponent(_bulletTexture));
            // .AddComponent(new VelocityComponent(playerComponent.Direction));
        }

        animationComponent.Enabled = movementVector != Vector2.Zero;
        positionComponent.Position += movementVector * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
    }
}
