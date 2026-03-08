using Engine.Components;
using Engine.ECS;

using Microsoft.Xna.Framework;

namespace Engine.Utils;

public class Helper(EntityManager entityManager)
{
    private readonly EntityManager _entityManager = entityManager;

    public Matrix GetCameraTransform()
    {
        var cameraEntity = _entityManager.GetEntityWithComponent<CameraComponent>()!;
        var cameraComponent = cameraEntity.GetComponent<CameraComponent>();
        return cameraComponent.Transform;
    }
}