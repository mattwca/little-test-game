namespace Engine.ECS;

public interface IRenderSystem
{
    void Draw(EntityManager entityManager, float deltaTime);
}