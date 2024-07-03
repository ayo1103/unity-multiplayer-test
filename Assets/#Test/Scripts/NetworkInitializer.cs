using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace WebMultiplayerTest
{
    public class NetworkInitializer : MonoBehaviour
    {
        [Header("Network")]
        [SerializeField] private UnityTransport _unityTransport;
        [SerializeField] private SecretsLoaderHelper _secretsLoaderHelper;

        [Header("UI")]
        [SerializeField] private Camera _offlineCamera;
        [SerializeField] private Button _reconnectButton;

        [Header("Editor Debug Only")]
        public bool autoStartServer;
        public bool autoStartHost;
        public bool autoStartClient;

        private void Awake()
        {
            _reconnectButton.gameObject.SetActive(false);
        }

        private async void Start()
        {
#if UNITY_EDITOR
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
#else
            await InitNetworkConnection();
#endif

            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
        }

        [UsedImplicitly]
        private async Task InitNetworkConnection()
        {
#if UNITY_SERVER
            string addressableName = AddressableNames.ServerNetworkConfig;

#elif UNITY_WEBGL
            string addressableName = AddressableNames.ClientNetworkConfig;
#endif

            var handle = Addressables.LoadAssetAsync<NetworkSetupConfig>(addressableName);
            Debug.Log($"load asset : {addressableName}");

            var networkConfig = await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"load asset : {addressableName} -- Succeeded");
                _unityTransport.SetConnectionData(
                    networkConfig.ipAddress,
                    networkConfig.port,
                    networkConfig.listenAddress);

#if UNITY_SERVER
                NetworkManager.Singleton.StartServer();
#elif UNITY_WEBGL
                NetworkManager.Singleton.StartClient();
#endif
            }
            else
            {
                Debug.LogError($"load asset : {addressableName} -- Failed");
            }
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;
            }
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