namespace Gameplay
{
    public interface IScenario
    {
        public void Pause();
        object GetScenarioContext();
        public void OnStateEnd();
        public void Init(IScenarioContext context);
        public IGameplayService GameplayService { get; set; }
        public void LoadMainMenu();
        public void RenewQueue();
        public void ClearTurnsOrder();
    }
}
