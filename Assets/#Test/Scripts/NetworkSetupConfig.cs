using UnityEngine;

namespace WebMultiplayerTest
{
    [CreateAssetMenu(fileName = "NetworkConfig", menuName = "Network/NetworkConfig")]
    public class NetworkSetupConfig : ScriptableObject
    {
        [Header("UnityTransport")]
        public string ipAddress;
        public ushort port;
        public string listenAddress;
    }
}