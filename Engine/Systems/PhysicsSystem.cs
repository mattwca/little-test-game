using System;
using System.Linq;

using Engine.Components;
using Engine.ECS;
using Engine.Rendering;
using Engine.Utils;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Systems;

public class PhysicsSystem : IUpdateSystem, IRenderSystem
{
    private readonly SpriteBatch _spriteBatch;
    private readonly StateManager _stateManager;
    private readonly EntityManager _entityManager;
    private readonly ShapeRenderer _shapeRenderer;
    private readonly QuadTree _quadTree;

    public PhysicsSystem(SpriteBatch spriteBatch, StateManager stateManager, EntityManager entityManager, ShapeRenderer shapeRenderer)
    {
        this._spriteBatch = spriteBatch;
        this._stateManager = stateManager;
        this._entityManager = entityManager;
        this._shapeRenderer = shapeRenderer;
        this._quadTree = new QuadTree(800, 600);

        this.BuildQuadTree();
    }

    public void Draw(GameTime gameTime)
    {
        if (!this._stateManager.GetBool("debugMode"))
        {
            return;
        }

        var cameraEntity = this._entityManager.GetEntityWithComponent<CameraComponent>()!;
        var cameraComponent = cameraEntity.GetComponent<CameraComponent>();

        this._spriteBatch.Begin(transformMatrix: cameraComponent.Transform, blendState: BlendState.AlphaBlend);

        // Render all static bounding boxes in the quad tree
        this._quadTree.WalkTree((leaf) =>
        {
            this._shapeRenderer.RenderSquare(leaf.NodeBox, Color.Red);

            foreach (var bb in leaf.BoundingBoxes)
            {
                this._shapeRenderer.RenderSquare(bb, Color.Red);
            }
        }, (branch) =>
        {
            this._shapeRenderer.RenderSquare(branch.NodeBox, Color.Red);
        });

        var entities = _entityManager
            .GetEntitiesWithComponents(typeof(PositionComponent), typeof(BoundingBoxComponent))
            .Where((entity) => !entity.GetComponent<BoundingBoxComponent>().IsStatic)
            .ToList();

        foreach (var entity in entities)
        {
            var positionComponent = entity.GetComponent<PositionComponent>();
            var dynamicBoundingBoxes = entity.GetComponentsWith<BoundingBoxComponent>((bb) => !bb.IsStatic);

            foreach (var boundingBox in dynamicBoundingBoxes)
            {
                var rectangle = GetBoundingRectangleForComponents(positionComponent, boundingBox);
                _shapeRenderer.RenderSquare(rectangle, Color.Blue);
            }
        }

        this._spriteBatch.End();
    }

    public void Update(GameTime gameTime)
    {
        var playerEntity = _entityManager.GetEntity("player");
        var playerPosition = playerEntity.GetComponent<PositionComponent>();
        var boundingBox = playerEntity.GetComponent<BoundingBoxComponent>();

        var intersectors = _quadTree.GetIntersectors(GetBoundingRectangleForComponents(playerPosition, boundingBox));
        foreach (var intersector in intersectors)
        {
            Console.WriteLine("Intersector: {0}, {1}", intersector.X, intersector.Y);
        }
    }

    private Rectangle GetBoundingRectangleForComponents(PositionComponent positionComponent, BoundingBoxComponent boundingBoxComponent)
    {
        var position = positionComponent.Position + boundingBoxComponent.Offset;
        return new Rectangle((int)position.X, (int)position.Y, boundingBoxComponent.Width, boundingBoxComponent.Height);
    }

    private void BuildQuadTree()
    {
        var boundingBoxEntities = _entityManager.GetEntitiesWithComponents(typeof(BoundingBoxComponent), typeof(PositionComponent));

        foreach (var entity in boundingBoxEntities)
        {
            var positionComponent = entity.GetComponent<PositionComponent>();
            var boundingBoxComponents = entity.GetComponentsWith<BoundingBoxComponent>((bb) => bb.IsStatic);

            foreach (var bbComponent in boundingBoxComponents)
            {
                var worldPosition = positionComponent.Position + bbComponent.Offset;
                var bbRect = new Rectangle((int)worldPosition.X, (int)worldPosition.Y, bbComponent.Width, bbComponent.Height);

                _quadTree.AddIntersector(bbRect);
            }
        }
    }
}