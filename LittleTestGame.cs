using Components;
using Engine.Animation;
using Engine.Components;
using Engine.ECS;
using Engine.Lighting;
using Engine.Particles;
using Engine.Rendering;
using Engine.Systems;
using Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Systems;

public class LittleTestGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private readonly SystemManager _systemManager;

    public LittleTestGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _systemManager = new SystemManager();

        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 600;
        _graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        var animationManager = new AnimationManager();
        animationManager.AddAnimation(new Animation("playerWalking", 6, 32, 32));

        _systemManager
            .Register(_spriteBatch)
            .Register(GraphicsDevice)
            .Register(Content)
            .Register(animationManager)
            .Register(new FPSCounter());

        _systemManager.Register(_systemManager.Construct<Helper>());
        _systemManager.Register(_systemManager.Construct<ShapeRenderer>());
        _systemManager.Register(_systemManager.Construct<TextRenderer>());
        _systemManager.Register(_systemManager.Construct<SpriteRenderer>());
        _systemManager.Register(_systemManager.Construct<TileRenderer>());
        _systemManager.Register(_systemManager.Construct<ParticleRenderer>());

        _systemManager
            .EntityManager.CreateEntity("camera")
            .AddComponent(new CameraComponent(new Vector2(0, 0), 2f, "player"));

        _systemManager
            .EntityManager.CreateEntity("map")
            .AddComponent(new PositionComponent(new Vector2(0, 0)))
            .AddComponent(
                new TiledRenderingComponent(
                    Content.Load<Texture2D>("floorTile"),
                    32,
                    100,
                    100,
                    Color.White,
                    new Vector2(0, 0),
                    0,
                    1f
                )
            );

        _systemManager
            .EntityManager.CreateEntity("light")
            .AddComponent(new PositionComponent(new Vector2(150, 150), 32, 32))
            .AddComponent(new LightComponent(Color.Gold, 100))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("light"), Color.White, castsShadow: false))
            .AddComponent(new VisibilityComponent(offset: 50));

        _systemManager
            .EntityManager.CreateEntity("light2")
            .AddComponent(new PositionComponent(new Vector2(400, 250), 32, 32))
            .AddComponent(new LightComponent(Color.Gold, 100))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("light"), Color.White, castsShadow: false))
            .AddComponent(new VisibilityComponent(offset: 50));

        _systemManager
            .EntityManager.CreateEntity("light3")
            .AddComponent(new PositionComponent(new Vector2(350, 50), 32, 32))
            .AddComponent(new LightComponent(Color.Gold, 100))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("light"), Color.White, castsShadow: false))
            .AddComponent(new VisibilityComponent(offset: 50));

        _systemManager
            .EntityManager.CreateEntity("player")
            .AddComponent(new PositionComponent(new Vector2(150, 50), 32, 32))
            .AddComponent(new HeightComponent())
            .AddComponent(
                new RenderingComponent(Content.Load<Texture2D>("player"), Color.White, layer: 2, castsShadow: true)
            )
            .AddComponent(new PlayerComponent())
            .AddComponent(new VisibilityComponent())
            .AddComponent(new AnimationComponent("playerWalking", 30))
            .AddComponent(new BoundingBoxComponent(new Vector2(8, 16), 16, 16, false));

        _systemManager
            .EntityManager.CreateEntity("playerShadow")
            .AddComponent(new PositionComponent(Vector2.Zero))
            .AddComponent(new AttachedComponent("player", new Vector2(16, 32)))
            .AddComponent(new VisibilityComponent())
            .AddComponent(
                new RenderingComponent(
                    Content.Load<Texture2D>("shadow"),
                    new Color(255, 255, 255, 128),
                    origin: new Vector2(16, 16),
                    layer: 1,
                    // scale: 1,
                    castsShadow: false
                )
            );

        _systemManager
            .EntityManager.CreateEntity("mapBase")
            .AddComponent(
                new MapComponent(
                    WallTileDefinitions.Definition,
                    new int[,]
                    {
                        { 1, 1, 0, 0, 0, 1, 1, 1, 1, 1 },
                        { 1, 1, 0, 0, 0, 1, 1, 1, 1, 1 },
                        { 1, 1, 1, 1, 0, 0, 0, 0, 1, 1 },
                        { 1, 1, 1, 1, 0, 0, 0, 0, 1, 1 },
                        { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1 },
                        { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1 },
                    },
                    1
                )
            )
            .AddComponent(new PositionComponent(new Vector2(20, 50)));

        _systemManager.AddSystem<VisibilitySystem>();
        _systemManager.AddSystem<PlayerSystem>();
        _systemManager.AddSystem<AttachedSystem>();
        _systemManager.AddSystem<MapSystem>();
        _systemManager.AddSystem<PhysicsSystem>();
        _systemManager.AddSystem<GravitySystem>();
        _systemManager.AddSystem<AnimationSystem>();
        _systemManager.AddSystem<LightSystem>();
        _systemManager.AddSystem<RenderingSystem>();
        _systemManager.AddSystem<ParticleEmitterSystem>();
        _systemManager.AddSystem<DebugTextSystem>();
        _systemManager.AddSystem<CameraSystem>();
    }

    protected override void Update(GameTime gameTime)
    {
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
            Exit();

        _systemManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _systemManager.Render(gameTime);

        base.Draw(gameTime);
    }
}
