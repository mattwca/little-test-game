using System;
using Components;
using Engine.Components;
using Engine.ECS;
using Engine.Events;
using Engine.Particles;
using Engine.Utils;
using Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Systems;

public class PlayerSystem : IUpdateSystem
{
    private readonly StateManager _stateManager;
    private readonly EntityManager _entityManager;
    private readonly KeyboardHandler _keyboardHandler;
    private readonly EventBus _eventBus;

    private const float JUMP_IMPULSE = 200f;

    public PlayerSystem(StateManager stateManager, EntityManager entityManager, EventBus eventBus)
    {
        _stateManager = stateManager;
        _entityManager = entityManager;
        _keyboardHandler = new KeyboardHandler();
        _eventBus = eventBus;
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
                .AddComponent(new PositionComponent(playerPosition.Position + new Vector2(16, 32)))
                .AddComponent(
                    new ParticleEmitterComponent(
                        particleType: new ParticleTypeConfig("Particles/playerJump", 8, 8),
                        spawnConfig: new ParticleSpawnConfig(5, 50f, 0.5f),
                        colourConfig: new ParticleColourConfig(Color.White, Color.White),
                        new ParticleEmitterCircle(), // (-50, 50),
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
                            8,
                            8,
                            false,
                            new ParticleLightingConfig(
                                ParticleLightingOption.EmitsLight,
                                1f,
                                500f,
                                5f,
                                0.95f,
                                0.9f,
                                10f
                            )
                        ),
                        new ParticleSpawnConfig(10, Velocity: 150f, SpawnRate: 5f, LifespanSeconds: 3),
                        new ParticleColourConfig(Color.Red),
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

        if (_keyboardHandler.WasKeyPressed(Keys.Space) && heightComponent.Grounded)
        {
            heightComponent.ZVelocity = JUMP_IMPULSE;
        }

        if (_keyboardHandler.WasKeyPressed(Keys.LeftShift))
        {
            animationComponent.PlaybackSpeed = 10f;

            _eventBus.Publish(
                new DodgeEvent(
                    playerEntity.Id,
                    movementVector,
                    0.25f,
                    0.1f,
                    () => animationComponent.PlaybackSpeed = 1f
                )
            );
        }

        animationComponent.Enabled = movementVector != Vector2.Zero;

        playerEntity.ReplaceComponent(
            new VelocityComponent(movementVector * (float)gameTime.ElapsedGameTime.TotalSeconds * 100)
        );
    }
}
