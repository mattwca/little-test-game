using System;
using System.Linq;
using Engine.Components;
using Engine.Configuration;
using Engine.ECS;
using Engine.Events;
using Engine.Physics;
using Engine.Rendering;
using Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Systems;

public class CollisionResolutionSystem : IUpdateSystem, IRenderSystem
{
    private readonly SpriteBatch _spriteBatch;
    private readonly StateManager _stateManager;
    private readonly EntityManager _entityManager;
    private readonly ShapeRenderer _shapeRenderer;
    private readonly EventBus _eventBus;
    private readonly GameConfiguration _gameConfiguration;

    private bool _isUpdatingQuadTree;
    private volatile QuadTree _quadTree;

    public CollisionResolutionSystem(
        SpriteBatch spriteBatch,
        StateManager stateManager,
        EntityManager entityManager,
        ShapeRenderer shapeRenderer,
        EventBus eventBus,
        GameConfiguration gameConfiguration
    )
    {
        _spriteBatch = spriteBatch;
        _stateManager = stateManager;
        _entityManager = entityManager;
        _shapeRenderer = shapeRenderer;
        _eventBus = eventBus;
        _gameConfiguration = gameConfiguration;

        _quadTree = new QuadTree(gameConfiguration.WorldWidth, gameConfiguration.WorldHeight);
        _isUpdatingQuadTree = false;

        BuildQuadTree();
    }

    public void Draw(GameTime gameTime)
    {
        if (!_stateManager.GetBool("debugModeEnabled"))
        {
            return;
        }

        var cameraEntity = _entityManager.GetEntityWithComponent<CameraComponent>()!;
        var cameraComponent = cameraEntity.GetComponent<CameraComponent>();

        _spriteBatch.Begin(transformMatrix: cameraComponent.Transform, blendState: BlendState.AlphaBlend);

        // Render all static bounding boxes in the quad tree
        _quadTree.WalkTree(
            (leaf) =>
            {
                _shapeRenderer.RenderSquare(leaf.NodeBox.ToRectangle(), Color.Red);

                foreach (var bb in leaf.BoundingBoxes)
                {
                    _shapeRenderer.RenderSquare(bb.ToRectangle(), Color.Red);
                }
            },
            (branch) =>
            {
                _shapeRenderer.RenderSquare(branch.NodeBox.ToRectangle(), Color.Red);
            }
        );

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
                _shapeRenderer.RenderSquare(rectangle.ToRectangle(), Color.Blue);
            }
        }

        _spriteBatch.End();
    }

    public void Update(GameTime gameTime)
    {
        BuildQuadTree();

        var dynamicBoundingEntities = _entityManager.Entities.Where(entity =>
            entity.HasComponents(typeof(PositionComponent), typeof(VelocityComponent), typeof(BoundingBoxComponent))
            && !entity.GetComponent<BoundingBoxComponent>().IsStatic
        );

        foreach (var entity in dynamicBoundingEntities)
        {
            var positionComponent = entity.GetComponent<PositionComponent>()!;
            var boundingBoxComponent = entity.GetComponent<BoundingBoxComponent>()!;
            var velocityComponent = entity.GetComponent<VelocityComponent>();

            var delta = velocityComponent.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;

            // X pass
            positionComponent.Position += new Vector2(delta.X, 0);

            var boundingRect = GetBoundingRectangleForComponents(positionComponent, boundingBoxComponent);
            var intersectors = _quadTree.GetIntersectors(boundingRect);

            foreach (var intersector in intersectors)
            {
                var entityCentre = boundingRect.Centre;
                var intersectorCentre = intersector.Centre;

                var intersectionDirection = entityCentre - intersectorCentre;
                var overlapX = (boundingRect.Width + intersector.Width) / 2f - Math.Abs(intersectionDirection.X);

                var normal = new Vector2(Math.Sign(intersectionDirection.X), 0);
                positionComponent.Position += normal * overlapX;

                velocityComponent.Velocity = new Vector2(0, velocityComponent.Velocity.Y);
                boundingRect = GetBoundingRectangleForComponents(positionComponent, boundingBoxComponent);
                _eventBus.Publish(new CollisionEvent(entity.Id, intersector.EntityId!, Vector2.Zero, normal, overlapX));
            }

            // Y pass
            positionComponent.Position += new Vector2(0, delta.Y);

            boundingRect = GetBoundingRectangleForComponents(positionComponent, boundingBoxComponent);
            intersectors = _quadTree.GetIntersectors(boundingRect);

            foreach (var intersector in intersectors)
            {
                var entityCentre = boundingRect.Centre;
                var intersectorCentre = intersector.Centre;

                var intersectionDirection = entityCentre - intersectorCentre;
                var overlapY = (boundingRect.Height + intersector.Height) / 2f - Math.Abs(intersectionDirection.Y);

                var normal = new Vector2(0f, Math.Sign(intersectionDirection.Y));
                positionComponent.Position += normal * overlapY;
                velocityComponent.Velocity = new Vector2(velocityComponent.Velocity.X, 0);

                boundingRect = GetBoundingRectangleForComponents(positionComponent, boundingBoxComponent);

                _eventBus.Publish(new CollisionEvent(entity.Id, intersector.EntityId!, Vector2.Zero, normal, overlapY));
            }
        }
    }

    private RectangleF GetBoundingRectangleForComponents(
        PositionComponent positionComponent,
        BoundingBoxComponent boundingBoxComponent
    )
    {
        var position = positionComponent.Position + boundingBoxComponent.Offset;
        return new RectangleF(position.X, position.Y, boundingBoxComponent.Width, boundingBoxComponent.Height);
    }

    private void BuildQuadTree()
    {
        if (_isUpdatingQuadTree)
        {
            return;
        }

        _isUpdatingQuadTree = true;

        var boundingBoxEntities = _entityManager.GetEntitiesWithComponents(
            typeof(BoundingBoxComponent),
            typeof(PositionComponent)
        );

        var newQuadTree = new QuadTree(_gameConfiguration.WorldWidth, _gameConfiguration.WorldHeight);

        foreach (var entity in boundingBoxEntities)
        {
            var positionComponent = entity.GetComponent<PositionComponent>();
            var boundingBoxComponents = entity.GetComponentsWith<BoundingBoxComponent>((bb) => bb.IsStatic);

            foreach (var bbComponent in boundingBoxComponents)
            {
                var worldPosition = positionComponent.Position + bbComponent.Offset;
                var bbRect = new RectangleF(worldPosition.X, worldPosition.Y, bbComponent.Width, bbComponent.Height);

                newQuadTree.AddIntersector(bbRect);
            }
        }

        _isUpdatingQuadTree = false;
        _quadTree = newQuadTree;
    }
}
