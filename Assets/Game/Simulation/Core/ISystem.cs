public interface ISystem {
    void Init(Simulation simulation) {
    }

    void Run(Simulation s, Frame fr);
}