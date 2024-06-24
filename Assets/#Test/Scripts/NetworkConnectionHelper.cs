using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkConnectionHelper : MonoBehaviour
{
    [SerializeField] private Camera _offlineCamera;
    [SerializeField] private Button _reconnectButton;

    public bool autoStartServer;
    public bool autoStartHost;
    public bool autoStartClient;

    private void Awake()
    {
        _reconnectButton.gameObject.SetActive(false);
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
            Debug.Log($"connectionEventData = {connectionEventData.ClientId} event = {connectionEventData.EventType}");
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