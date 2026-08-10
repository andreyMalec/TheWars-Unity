using UnityEngine;

[DefaultExecutionOrder(-10000)]
public sealed class GameStartup : MonoBehaviour {
    [SerializeField] private GameObject playerUiPrefab;
    [SerializeField] private ConfigDatabase configDatabase;
    [SerializeField] private Vector2[] initialBasePositions = { new Vector2(-8f, 0f), new Vector2(8f, 0f) };

    private void Awake() {
        configDatabase.RebuildCache();

        var compositionRoot = new GameCompositionRoot(configDatabase, Simulation.TickRate);
        InitializeBases(compositionRoot.Simulation);

        var runner = gameObject.AddComponent<SimulationRunner>();
        runner.Initialize(compositionRoot.Simulation, compositionRoot.TickManager);

        var playerCommandProcessor = new PlayerCommandProcessor(compositionRoot.Simulation);
        var playerUi = Instantiate(playerUiPrefab);
        var playerInputController = playerUi.GetComponent<PlayerInputController>();
        playerInputController.Initialize(playerCommandProcessor);

        var playerUiView = playerUi.GetComponent<PlayerUiView>();
        playerUiView.Bind(configDatabase);

        var presenter = gameObject.AddComponent<FramePresenter>();
        presenter.Initialize(compositionRoot.Simulation, playerUiView);

        DontDestroyOnLoad(gameObject);
    }

    private void InitializeBases(Simulation simulation) {
        var count = 2;

        for (var i = 0; i < count; i++) {
            var config = configDatabase.GetConfig<BaseConfig>(Epoch.StoneAge, EntityType.Type1);
            var state = new BaseState {
                Id = simulation.Frame.GenerateEntityId(),
                Team = (Team)i,
                ConfigId = config.id,
                Position = initialBasePositions[i],
                Health = config.startHealth,
                Epoch = Epoch.StoneAge,
                Resources = config.startResources,
                Slots = new Slot[config.slotPositions.Length]
            };
            for (int j = 0; j < state.Slots.Length; j++) {
                state.Slots[j] = new Slot();
            }

            state.Slots[0].IsActive = true;

            simulation.Frame.AddBase(state);
        }
    }
}