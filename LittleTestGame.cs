using Engine.Components;
using Engine.ECS;
using Engine.Lighting;
using Engine.Systems;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class LittleTestGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private readonly SystemManager _systemManager;

    public LittleTestGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _systemManager = new SystemManager();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _systemManager
            .Register(_spriteBatch)
            .Register(GraphicsDevice)
            .Register(Content);

        var cameraEntity = _systemManager.EntityManager.CreateEntity("camera");
        cameraEntity.AddComponent(new CameraComponent(new Vector2(0, 0), 2f));

        var playerEntity = _systemManager.EntityManager.CreateEntity("player");
        playerEntity.AddComponent(new PositionComponent(new Vector2(100, 100)));
        playerEntity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("player"), Color.White, layer: 2));
        playerEntity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("shadow"), new Color(255, 255, 255, 128), new Vector2(8, 24), 1, 0.5f));
        playerEntity.AddComponent(new PlayerComponent());

        var mapEntity = _systemManager.EntityManager.CreateEntity("map");
        mapEntity.AddComponent(new PositionComponent(new Vector2(0, 0)));
        mapEntity.AddComponent(new TiledRenderingComponent(Content.Load<Texture2D>("floorTile"), 16, 20, 20, Color.White, new Vector2(0, 0), 0, 1f));

        var lightEntity = _systemManager.EntityManager.CreateEntity("light");
        lightEntity.AddComponent(new PositionComponent(new Vector2(150, 150)));
        lightEntity.AddComponent(new LightComponent(Color.White, 100));
        lightEntity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("light"), Color.White));

        _systemManager.AddSystem<PlayerSystem>();

        var lightSystem = _systemManager.AddSystem<LightSystem>();
        _systemManager.Register(lightSystem);

        _systemManager.AddSystem<TiledRenderingSystem>();
        _systemManager.AddSystem<RenderingSystem>();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _systemManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _systemManager.Render((float)gameTime.ElapsedGameTime.TotalSeconds);

        base.Draw(gameTime);
    }
}