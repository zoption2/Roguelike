using Gameplay;
using UnityEngine;
using Zenject;

public interface IScenarioFactory
{
    public IScenario CreateScenario(TypeOfScenario type, IGameplayService fullService);
    public IScenarioContext CreateContext(TypeOfScenario type);
}

public class ScenarioFactory : IScenarioFactory
{
    [Inject]
    public DiContainer _diContainer;

    public IScenario CreateScenario(TypeOfScenario type, IGameplayService fullService)
    {
        IScenario scenario = null;
        switch (type)
        {
            case TypeOfScenario.DefaultRoom:
                scenario = _diContainer.Resolve<IDefaultScenario>();
                break;
            case TypeOfScenario.MainRoom:
                scenario = _diContainer.Resolve<IChestScenario>();
                break;
                //case TypeOfScenario.Boss:
                //    //scenario = new BossScenario(fullService, context);
                //    scenario = new BossScenario(fullService);
                //    break;
        }
        return scenario;
    }

    public IScenarioContext CreateContext(TypeOfScenario type)
    {
        IScenarioContext context = null;

        switch (type)
        {
            case TypeOfScenario.DefaultRoom:
                context = new RoomContext();
                break;
            case TypeOfScenario.MainRoom:
                context = new RoomContext();
                break;
            default:
                Debug.LogWarning("--|" + this + "Can`t create a scenario context |--");
                break;
        }

        return context;
    }


}