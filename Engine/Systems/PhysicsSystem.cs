using System;
using System.Collections.Generic;
using System.Linq;

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
    private readonly QuadTree _quadTree;

    private readonly List<RectangleF> _intersectResults;

    public PhysicsSystem(SpriteBatch spriteBatch, StateManager stateManager, EntityManager entityManager, ShapeRenderer shapeRenderer)
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
        if (!_stateManager.GetBool("renderBoundingBoxes"))
        {
            return;
        }

        var cameraEntity = _entityManager.GetEntityWithComponent<CameraComponent>()!;
        var cameraComponent = cameraEntity.GetComponent<CameraComponent>();

        _spriteBatch.Begin(transformMatrix: cameraComponent.Transform, blendState: BlendState.AlphaBlend);

        // Render all static bounding boxes in the quad tree
        _quadTree.WalkTree((leaf) =>
        {
            _shapeRenderer.RenderSquare(leaf.NodeBox.ToRectangle(), Color.Red);

            foreach (var bb in leaf.BoundingBoxes)
            {
                _shapeRenderer.RenderSquare(bb.ToRectangle(), Color.Red);
            }
        }, (branch) =>
        {
            _shapeRenderer.RenderSquare(branch.NodeBox.ToRectangle(), Color.Red);
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
        var dynamicBoundingEntities = _entityManager.Entities.Where(entity =>
            entity.HasComponent<PositionComponent>() &&
            entity.HasComponent<BoundingBoxComponent>() &&
            !entity.GetComponent<BoundingBoxComponent>().IsStatic
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
                } else
                {
                    positionComponent.Position += new Vector2(0, Math.Sign(intersectionDirection.Y) * overlapY);
                }

                // var intersectionResult = RectangleF.Intersect(boundingRect, intersector);
                // _intersectResults.Add(intersectionResult);

                // if (intersectionResult.Width < intersectionResult.Height)
                // {
                //     if (intersectionResult.X < positionComponent.Position.X)
                //     {
                //         positionComponent.Position += new Vector2(intersectionResult.Width, 0);
                //     }
                //     else
                //     {
                //         positionComponent.Position -= new Vector2(intersectionResult.Width, 0);
                //     }
                // }
                // else
                // {
                //     if (intersectionResult.Y > boundingRect.Y)
                //     {
                //         positionComponent.Position -= new Vector2(0, intersectionResult.Height);
                //     }
                //     else
                //     {
                //         positionComponent.Position += new Vector2(0, intersectionResult.Height);
                //     }
                // }

                boundingRect = GetBoundingRectangleForComponents(positionComponent, boundingBoxComponent);
            }
        }
    }

    private RectangleF GetBoundingRectangleForComponents(PositionComponent positionComponent, BoundingBoxComponent boundingBoxComponent)
    {
        var position = positionComponent.Position + boundingBoxComponent.Offset;
        return new RectangleF(position.X, position.Y, boundingBoxComponent.Width, boundingBoxComponent.Height);
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
                var bbRect = new RectangleF(worldPosition.X, worldPosition.Y, bbComponent.Width, bbComponent.Height);

                _quadTree.AddIntersector(bbRect);
            }
        }
    }
}