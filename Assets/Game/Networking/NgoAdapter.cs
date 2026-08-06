#if NETCODE_PRESENT
using Unity.Netcode;

public sealed class NgoAdapter
{
    private readonly NetworkManager _networkManager;

    public NgoAdapter(NetworkManager networkManager)
    {
        _networkManager = networkManager;
    }

    public void StartHost()
    {
        _networkManager.StartHost();
    }

    public void StartClient()
    {
        _networkManager.StartClient();
    }

    public void Shutdown()
    {
        _networkManager.Shutdown();
    }
}
#else
public sealed class NgoAdapter {
    public void StartHost() {
    }

    public void StartClient() {
    }

    public void Shutdown() {
    }
}
#endif