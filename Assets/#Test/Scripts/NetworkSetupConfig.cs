using UnityEngine;

namespace WebMultiplayerTest
{
    [CreateAssetMenu(fileName = "NetworkConfig", menuName = "Network/NetworkConfig")]
    public class NetworkSetupConfig : ScriptableObject
    {
        public string ipAddress;
        public ushort port;
    }
}