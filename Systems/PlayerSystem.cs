using System;
using Components;
using Engine.Components;
using Engine.ECS;
using Engine.Particles;
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

        var playerEntity = _entityManager.GetEntityWithComponent<PlayerComponent>();
        if (playerEntity is null)
        {
            return;
        }

        HandlePlayerMovement(playerEntity, gameTime);
        HandleOtherInput(playerEntity);
        HandlePlayerJumped(playerEntity);
        HandlePlayerShoot(playerEntity);

        UpdatePlayerShadow(playerEntity);
    }

    private void UpdatePlayerShadow(Entity playerEntity)
    {
        var playerShadowEntity = _entityManager.GetEntity("playerShadow");
        if (playerShadowEntity is null)
        {
            return;
        }

        var playerShadowRendering = playerShadowEntity.GetComponent<RenderingComponent>();
        var playerHeightComponent = playerEntity.GetComponent<HeightComponent>();
        playerShadowRendering.Scale = 0.1f + (0.1f / Math.Max(playerHeightComponent.Z / 20f, 0.25f));
    }

    private void HandlePlayerJumped(Entity playerEntity)
    {
        var heightComponent = playerEntity.GetComponent<HeightComponent>();
        var playerPosition = playerEntity.GetComponent<PositionComponent>();

        if (heightComponent.Grounded && !heightComponent.WasGrounded)
        {
            var newEntityId = $"playerJumpParticles-{Guid.NewGuid()}";
            _entityManager
                .CreateEntity(newEntityId)
                .AddComponent(new PositionComponent(playerPosition.Position + new Vector2(12, 32)))
                .AddComponent(
                    new ParticleEmitterComponent(
                        particleType: new ParticleTypeConfig("Particles/playerJump"),
                        spawnConfig: new ParticleSpawnConfig(5, 50f, 0.5f),
                        colourConfig: new ParticleColourConfig(Color.White, Color.White),
                        new ParticleEmitterArc(-50, 50),
                        emitterType: ParticleEmitterType.BURST,
                        new ParticleRenderConfig(playerPosition.Position.Y)
                    )
                );
        }
    }

    private void HandlePlayerShoot(Entity playerEntity)
    {
        var playerPosition = playerEntity.GetComponent<PositionComponent>();

        if (_keyboardHandler.WasKeyPressed(Keys.Right))
        {
            _entityManager
                .CreateEntity($"playerShootRight-{Guid.NewGuid()}")
                .AddComponent(new AttachedComponent("player", Vector2.Zero))
                .AddComponent(new PositionComponent(playerPosition.Position))
                .AddComponent(
                    new ParticleEmitterComponent(
                        new ParticleTypeConfig(
                            "Particles/bullet",
                            false,
                            new ParticleLightingConfig(ParticleLightingOption.EmitsLight, 500f)
                        ),
                        new ParticleSpawnConfig(10, Velocity: 150f, SpawnRate: 5f, LifespanSeconds: 3),
                        new ParticleColourConfig(Color.White),
                        new ParticleEmitterPoint(new Vector2(1, 0)),
                        ParticleEmitterType.CONTINUOUS
                    )
                );
        }
    }

    private void HandleOtherInput(Entity playerEntity)
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

    private void HandlePlayerMovement(Entity playerEntity, GameTime gameTime)
    {
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
