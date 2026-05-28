using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Engine.Components;
using Engine.ECS;
using Engine.Physics;
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
    private readonly List<RectangleF> _intersectResults;

    private bool _isUpdatingQuadTree;
    private volatile QuadTree _quadTree;

    public PhysicsSystem(
        SpriteBatch spriteBatch,
        StateManager stateManager,
        EntityManager entityManager,
        ShapeRenderer shapeRenderer
    )
    {
        _spriteBatch = spriteBatch;
        _stateManager = stateManager;
        _entityManager = entityManager;
        _shapeRenderer = shapeRenderer;

        _quadTree = new QuadTree(800, 600);
        _intersectResults = new List<RectangleF>();

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

        foreach (var intersectResult in _intersectResults)
        {
            _shapeRenderer.RenderSquare(intersectResult.ToRectangle(), Color.Green);
        }

        _spriteBatch.End();
    }

    public void Update(GameTime gameTime)
    {
        BuildQuadTree();

        var dynamicBoundingEntities = _entityManager.Entities.Where(entity =>
            entity.HasComponent<PositionComponent>()
            && entity.HasComponent<BoundingBoxComponent>()
            && !entity.GetComponent<BoundingBoxComponent>().IsStatic
        );

        _intersectResults.Clear();

        foreach (var entity in dynamicBoundingEntities)
        {
            var positionComponent = entity.GetComponent<PositionComponent>()!;
            var boundingBoxComponent = entity.GetComponent<BoundingBoxComponent>()!;

            var boundingRect = GetBoundingRectangleForComponents(positionComponent, boundingBoxComponent);
            var intersectors = _quadTree.GetIntersectors(boundingRect);

            foreach (var intersector in intersectors)
            {
                var entityCentre = boundingRect.Centre;
                var intersectorCentre = intersector.Centre;

                var intersectionDirection = entityCentre - intersectorCentre;
                var overlapX = (boundingRect.Width + intersector.Width) / 2f - Math.Abs(intersectionDirection.X);
                var overlapY = (boundingRect.Height + intersector.Height) / 2f - Math.Abs(intersectionDirection.Y);

                if (overlapX < overlapY)
                {
                    positionComponent.Position += new Vector2(Math.Sign(intersectionDirection.X) * overlapX, 0);
                }
                else
                {
                    positionComponent.Position += new Vector2(0, Math.Sign(intersectionDirection.Y) * overlapY);
                }

                boundingRect = GetBoundingRectangleForComponents(positionComponent, boundingBoxComponent);
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

        // var quadTree = await Task.Run(() =>
        // {
        var boundingBoxEntities = _entityManager.GetEntitiesWithComponents(
            typeof(BoundingBoxComponent),
            typeof(PositionComponent)
        );

        var newQuadTree = new QuadTree(800, 600);

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
        // return newQuadTree;
        // });

        _isUpdatingQuadTree = false;
        _quadTree = newQuadTree;
    }
}
