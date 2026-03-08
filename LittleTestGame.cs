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

        // _graphics.PreferredBackBufferWidth = 600;
        // _graphics.PreferredBackBufferHeight = 600;
        // _graphics.ApplyChanges();

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
        cameraEntity.AddComponent(new CameraComponent(new Vector2(0, 0), 1.5f));

        var mapEntity = _systemManager.EntityManager.CreateEntity("map");
        mapEntity.AddComponent(new PositionComponent(new Vector2(0, 0)));
        mapEntity.AddComponent(new TiledRenderingComponent(Content.Load<Texture2D>("floorTile"), 16, 100, 100, Color.White, new Vector2(0, 0), 0, 1f));

        var lightEntity = _systemManager.EntityManager.CreateEntity("light");
        lightEntity.AddComponent(new PositionComponent(new Vector2(150, 150), 32, 32));
        lightEntity.AddComponent(new LightComponent(Color.White, 100));
        lightEntity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("light"), Color.White));

        var playerEntity = _systemManager.EntityManager.CreateEntity("player");
        playerEntity.AddComponent(new PositionComponent(new Vector2(150, 50), 32, 32));
        playerEntity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("player"), Color.White, layer: 2));
        playerEntity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("shadow"), new Color(255, 255, 255, 128), new Vector2(8, 24), layer: 0, 0.5f));
        playerEntity.AddComponent(new PlayerComponent());

        var wallEntity = _systemManager.EntityManager.CreateEntity("wall");
        wallEntity.AddComponent(new PositionComponent(new Vector2(300, 250), 32, 32));
        wallEntity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, layer: 1));
        wallEntity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(32, 0)));
        wallEntity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(0, -32)));
        wallEntity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(0, 32)));

        var wall2Entity = _systemManager.EntityManager.CreateEntity("wall2");
        wall2Entity.AddComponent(new PositionComponent(new Vector2(50, 100), 32, 32));
        wall2Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, layer: 1));
        wall2Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(32, 0)));
        wall2Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(0, -32)));
        wall2Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(0, -64)));


        var wall3Entity = _systemManager.EntityManager.CreateEntity("wall3");
        wall3Entity.AddComponent(new PositionComponent(new Vector2(200, 100), 32, 32));
        wall3Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, layer: 1));
        wall3Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(32, 0)));
        wall3Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(64, 0)));
        wall3Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(96, 0)));

        var wall4Entity = _systemManager.EntityManager.CreateEntity("wall4");
        wall4Entity.AddComponent(new PositionComponent(new Vector2(350, 150), 32, 32));
        wall4Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, layer: 1));
        wall4Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(32, 0)));
        wall4Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(64, 0)));
        wall4Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(64, 32)));

        var wall5Entity = _systemManager.EntityManager.CreateEntity("wall5");
        wall5Entity.AddComponent(new PositionComponent(new Vector2(50, 200), 32, 32));
        wall5Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, layer: 1));
        wall5Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(0, 32)));
        wall5Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(32, 32)));
        wall5Entity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(64, 32)));

        _systemManager.AddSystem<PlayerSystem>();

        var lightSystem = _systemManager.AddSystem<LightSystem>();
        _systemManager.Register(lightSystem);

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