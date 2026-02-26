using Engine.Components;
using Engine.ECS;
using Engine.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class LittleTestGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private SystemManager _systemManager;

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

        var cameraEntity = _systemManager.EntityManager.CreateEntity("camera");
        cameraEntity.AddComponent(new CameraComponent(new Vector2(0, 0), 2f));

        var playerEntity = _systemManager.EntityManager.CreateEntity("player");
        playerEntity.AddComponent(new RenderingComponent(Content.Load<Texture2D>("player"), new Vector2(100, 100), Color.White));
        playerEntity.AddComponent(new PlayerComponent());

        var mapEntity = _systemManager.EntityManager.CreateEntity("map");
        mapEntity.AddComponent(new TiledRenderingComponent(
            Content.Load<Texture2D>("floorTile"),
            new Vector2(0, 0),
            Color.White,
            32,
            10,
            10,
            0
        ));

        _systemManager.AddSystem(new PlayerSystem());
        _systemManager.AddSystem(new TiledRenderingSystem(_spriteBatch));
        _systemManager.AddSystem(new RenderingSystem(_spriteBatch));
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
