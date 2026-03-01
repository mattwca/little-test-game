using System.Collections.Generic;

namespace Engine.ECS;

public class SystemManager
{
    public EntityManager EntityManager { get; }

    private readonly List<IUpdateSystem> _updateSystems;
    private readonly List<IRenderSystem> _renderSystems;

    public SystemManager()
    {
        EntityManager = new EntityManager();

        _updateSystems = [];
        _renderSystems = [];
    }

    public void AddSystem(IUpdateSystem system) => _updateSystems.Add(system);
    public void AddSystem(IRenderSystem system) => _renderSystems.Add(system);

    public void Update(float deltaTime)
    {
        _updateSystems.ForEach(system => system.Update(EntityManager, deltaTime));
    }

    public void Render(float deltaTime)
    {
        _renderSystems.ForEach(system => system.Draw(EntityManager, deltaTime));
    }
}