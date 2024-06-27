using JetBrains.Annotations;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

namespace WebMultiplayerTest
{
    public class NetworkConnectionHelper : MonoBehaviour
    {
        [SerializeField] private NetworkSetupConfig _networkSetupConfig;
        [SerializeField] private UnityTransport _unityTransport;

        [SerializeField] private Camera _offlineCamera;
        [SerializeField] private Button _reconnectButton;

        [Header("Editor Debug Only")]
        public bool autoStartServer;
        public bool autoStartHost;
        public bool autoStartClient;

        private void Awake()
        {
            _reconnectButton.gameObject.SetActive(false);
            var ipAddress = _networkSetupConfig.ipAddress;
            var port = _networkSetupConfig.port;
            _unityTransport.SetConnectionData(ipAddress, port);
        }

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

            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
        }

        //
        // [UsedImplicitly]
        // private void SetupNetworkConnection()
        // {
        //     switch (_networkSetupConfig.buildType)
        //     {
        //         case BuildType.Server:
        //             NetworkManager.Singleton.StartServer();
        //             break;
        //         case BuildType.Client:
        //             NetworkManager.Singleton.StartClient();
        //             break;
        //     }
        // }

        private void OnDestroy()
        {
            NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;
        }

        private void OnEnable()
        {
            _reconnectButton.onClick.AddListener(OnClickReconnectButton);
        }

        private void OnDisable()
        {
            _reconnectButton.onClick.RemoveListener(OnClickReconnectButton);
        }

        private void OnConnectionEvent(NetworkManager networkManager, ConnectionEventData connectionEventData)
        {
            if (connectionEventData.ClientId == NetworkManager.Singleton.LocalClientId)
            {
                Debug.Log(
                    $"connectionEventData = {connectionEventData.ClientId} event = {connectionEventData.EventType}");
                switch (connectionEventData.EventType)
                {
                    case ConnectionEvent.ClientDisconnected:
                        _offlineCamera.enabled = true;
                        _reconnectButton.gameObject.SetActive(true);
                        break;

                    case ConnectionEvent.ClientConnected:
                        _offlineCamera.enabled = false;
                        _reconnectButton.gameObject.SetActive(false);
                        break;
                }
            }
        }

        private void OnClickReconnectButton()
        {
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.StartClient();
        }
    }
}