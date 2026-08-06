using UnityEngine;

[DefaultExecutionOrder(-10000)]
public sealed class GameStartup : MonoBehaviour
{
    [SerializeField] private ConfigDatabase configDatabase;
    [SerializeField] private int tickRate = 60;

    private void Awake()
    {
        configDatabase.RebuildCache();

        var compositionRoot = new GameCompositionRoot(configDatabase, tickRate);
        var runner = gameObject.AddComponent<SimulationRunner>();
        runner.Initialize(compositionRoot.Simulation, compositionRoot.TickManager);

        DontDestroyOnLoad(gameObject);
    }
}

