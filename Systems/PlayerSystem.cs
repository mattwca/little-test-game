using System;
using System.Linq;
using Components;
using Engine.Components;
using Engine.ECS;
using Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Systems;

public class PlayerSystem : IUpdateSystem
{
    private readonly StateManager _stateManager;
    private readonly EntityManager _entityManager;
    private readonly KeyboardHandler _keyboardHandler;
    private readonly Texture2D _bulletTexture;

    private const float JUMP_IMPULSE = 200f;

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

        UpdatePlayerShadow();
    }

    private void UpdatePlayerShadow()
    {
        var playerEntity = _entityManager.GetEntityWithComponent<PlayerComponent>();
        var playerShadowEntity = _entityManager.GetEntity("playerShadow");

        var playerHeightComponent = playerEntity.GetComponent<HeightComponent>();
        var playerShadowRendering = playerShadowEntity.GetComponent<RenderingComponent>();

        playerShadowRendering.Scale = 0.1f + (0.1f / Math.Max(playerHeightComponent.Z / 20f, 0.25f));

        if (playerHeightComponent.Grounded)
        {
            return;
        }
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
        var heightComponent = playerEntity.GetComponent<HeightComponent>();

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

        if (Keyboard.GetState().IsKeyDown(Keys.Space) && heightComponent.Grounded)
        {
            heightComponent.ZVelocity = JUMP_IMPULSE;
        }

        animationComponent.Enabled = movementVector != Vector2.Zero;
        positionComponent.Position += movementVector * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
    }
}
