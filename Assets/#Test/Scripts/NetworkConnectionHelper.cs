using Unity.Netcode;
using UnityEngine;

public class NetworkConnectionHelper : MonoBehaviour
{
    public bool autoStartServer;
    public bool autoStartHost;
    public bool autoStartClient;

    public void Start()
    {
        if (autoStartServer)
        {
            NetworkManager.Singleton.StartServer();
        }

        if (autoStartHost)
        {
            NetworkManager.Singleton.StartHost();
        }

        if (autoStartClient)
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}