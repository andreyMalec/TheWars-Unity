using UnityEngine;

[DefaultExecutionOrder(-10000)]
public sealed class GameStartup : MonoBehaviour {
    [SerializeField] private ConfigDatabase configDatabase;
    [SerializeField] private int tickRate = 60;
    [SerializeField] private ConfigId[] initialBaseConfigIds = { new(1), new(1) };
    [SerializeField] private int[] initialBaseTeams = { 1, 2 };
    [SerializeField] private Vector2[] initialBasePositions = { new Vector2(-8f, 0f), new Vector2(8f, 0f) };

    private void Awake() {
        configDatabase.RebuildCache();

        var compositionRoot = new GameCompositionRoot(configDatabase, tickRate);
        InitializeBases(compositionRoot.Simulation);

        var runner = gameObject.AddComponent<SimulationRunner>();
        runner.Initialize(compositionRoot.Simulation, compositionRoot.TickManager);

        var presenter = gameObject.AddComponent<FramePresenter>();
        presenter.Initialize(compositionRoot.Simulation);

        DontDestroyOnLoad(gameObject);
    }

    private void InitializeBases(Simulation simulation) {
        var count = initialBaseConfigIds.Length;
        if (initialBaseTeams.Length < count) {
            count = initialBaseTeams.Length;
        }

        if (initialBasePositions.Length < count) {
            count = initialBasePositions.Length;
        }

        for (var i = 0; i < count; i++) {
            var config = configDatabase.GetBaseConfig(initialBaseConfigIds[i]);
            var state = new BaseState {
                Id = simulation.Frame.World.GenerateEntityId(),
                Team = initialBaseTeams[i],
                ConfigId = config.Id,
                Position = initialBasePositions[i],
                Health = config.StartHealth,
                Level = 1,
                Resources = config.StartResources
            };

            simulation.Frame.World.AddBase(state);
        }
    }
}