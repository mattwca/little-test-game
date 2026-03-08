using Engine.Components;
using Engine.ECS;
using Engine.Lighting;
using Engine.Rendering;
using Engine.Systems;
using Engine.Utils;

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

        _systemManager.Register(_systemManager.Construct<Helper>());

        _systemManager.EntityManager
            .CreateEntity("camera")
            .AddComponent(new CameraComponent(new Vector2(0, 0), 2f));

        _systemManager.EntityManager
            .CreateEntity("map")
            .AddComponent(new PositionComponent(new Vector2(0, 0)))
            .AddComponent(new TiledRenderingComponent(Content.Load<Texture2D>("floorTile"), 32, 100, 100, Color.White, new Vector2(0, 0), 0, 1f));

        _systemManager.EntityManager
            .CreateEntity("light")
            .AddComponent(new PositionComponent(new Vector2(150, 150), 32, 32))
            .AddComponent(new LightComponent(Color.Gold, 100))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("light"), Color.White, castsShadow: false));

        _systemManager.EntityManager
            .CreateEntity("player")
            .AddComponent(new PositionComponent(new Vector2(150, 50), 32, 32))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("player"), Color.White, layer: 2, castsShadow: true))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("shadow"), new Color(255, 255, 255, 128), new Vector2(8, 24), layer: 1, 0.5f, castsShadow: false))
            .AddComponent(new PlayerComponent());

        _systemManager.EntityManager
            .CreateEntity("wall")
            .AddComponent(new PositionComponent(new Vector2(300, 250), 32, 32))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, layer: 1))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(32, 0)))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(0, -32)))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(0, 32)));

        _systemManager.EntityManager
            .CreateEntity("wall2")
            .AddComponent(new PositionComponent(new Vector2(50, 100), 32, 32))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, layer: 1))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(32, 0)))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(0, -32)))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(0, -64)));

        _systemManager.EntityManager
            .CreateEntity("wall3")
            .AddComponent(new PositionComponent(new Vector2(200, 100), 32, 32))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, layer: 1))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(32, 0)))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(64, 0)))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(96, 0)));

        _systemManager.EntityManager
            .CreateEntity("wall4")
            .AddComponent(new PositionComponent(new Vector2(350, 150), 32, 32))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, layer: 1))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(32, 0)))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(64, 0)))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(64, 32)));

        _systemManager.EntityManager
            .CreateEntity("wall5")
            .AddComponent(new PositionComponent(new Vector2(50, 200), 32, 32))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, layer: 1))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(0, 32)))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(32, 32)))
            .AddComponent(new RenderingComponent(Content.Load<Texture2D>("wall"), Color.White, offset: new Vector2(64, 32)));

        _systemManager.AddSystem<PlayerSystem>();
        _systemManager.Register(_systemManager.Construct<SpriteRenderer>());

        var lightSystem = _systemManager.AddSystem<LightSystem>();
        _systemManager.Register(lightSystem);

        _systemManager.Register(_systemManager.Construct<TileRenderer>());
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