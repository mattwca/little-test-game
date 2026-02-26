namespace Engine.ECS;

public interface IUpdateSystem
{
    void Update(EntityManager entityManager, float deltaTime);
}