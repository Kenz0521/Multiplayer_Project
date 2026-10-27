using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class NetworkUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private GameObject networkHUD;

    [Header("Joining Screen")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_InputField hostIPInput;
    [SerializeField] private TMP_Text statusText;

    [Header("Network HUD")]
    [SerializeField] private TMP_Text connectionText;
    [SerializeField] private TMP_Text playerCountText;

    [Header("Network Settings")]
    [SerializeField] private ushort port = 7777;

    private NetworkManager networkManager;
    private UnityTransport transport;

    // New Input System action for Escape.
    private InputAction escapeAction;

    private void Awake()
    {
        escapeAction = new InputAction(
            "LeaveSession",
            InputActionType.Button,
            "<Keyboard>/escape"
        );

        escapeAction.performed += OnEscapePressed;
    }

    private void OnEnable()
    {
        escapeAction.Enable();
    }

    private void Start()
    {
        networkManager = NetworkManager.Singleton;

        if (networkManager == null)
        {
            statusText.text =
                "ERROR: NetworkManager not found.";
            return;
        }

        transport =
            networkManager.GetComponent<UnityTransport>();

        if (transport == null)
        {
            statusText.text =
                "ERROR: Unity Transport not found.";
            return;
        }

        networkManager.OnClientConnectedCallback +=
            OnClientConnected;

        networkManager.OnClientDisconnectCallback +=
            OnClientDisconnected;

        networkManager.OnTransportFailure +=
            OnTransportFailure;

        joinPanel.SetActive(true);
        networkHUD.SetActive(false);

        statusText.text = "Ready to play!";
    }

    public void HostGame()
    {
        if (!CanStartConnection())
            return;

        string playerName = GetPlayerName();

        if (playerName == "")
            return;

        // Configure the host to listen on all interfaces.
        transport.SetConnectionData(
            "127.0.0.1",
            port,
            "0.0.0.0"
        );

        statusText.text = "Starting host...";

        bool started = networkManager.StartHost();

        if (started)
        {
            ShowNetworkHUD();
            connectionText.text =
                "Hosting as " + playerName;

            UpdatePlayerCount();
        }
        else
        {
            statusText.text =
                "Could not start host. Check Console.";
        }
    }

    public void JoinGame()
    {
        if (!CanStartConnection())
            return;

        string playerName = GetPlayerName();

        if (playerName == "")
            return;

        string hostIP = hostIPInput.text.Trim();

        if (string.IsNullOrWhiteSpace(hostIP))
        {
            statusText.text =
                "Enter the host's IP address.";
            return;
        }

        transport.SetConnectionData(hostIP, port);

        statusText.text =
            "Connecting as " + playerName + "...";

        bool started = networkManager.StartClient();

        if (!started)
        {
            statusText.text =
                "Could not start client. Check Console.";
        }
    }

    private string GetPlayerName()
    {
        string playerName = playerNameInput.text.Trim();

        if (string.IsNullOrWhiteSpace(playerName))
        {
            statusText.text =
                "Please enter your name.";
            return "";
        }

        return playerName;
    }

    private bool CanStartConnection()
    {
        if (networkManager == null || transport == null)
        {
            statusText.text =
                "NetworkManager or Transport is missing.";
            return false;
        }

        if (networkManager.IsListening)
        {
            statusText.text =
                "A network session is already running.";
            return false;
        }

        return true;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (networkManager == null)
            return;

        if (clientId == networkManager.LocalClientId)
        {
            ShowNetworkHUD();

            if (networkManager.IsHost)
                connectionText.text =
                    "Hosting | Port " + port;
            else
                connectionText.text = "Connected to host";
        }

        UpdatePlayerCount();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (networkManager == null)
            return;

        // If the local client disconnected, return to the menu.
        if (clientId == networkManager.LocalClientId)
        {
            ShowJoinPanel();

            statusText.text =
                "Disconnected. Check the host and try again.";
        }

        UpdatePlayerCount();
    }

    private void OnTransportFailure()
    {
        ShowJoinPanel();

        statusText.text =
            "Network error. Check IP, port, and firewall.";
    }

    private void UpdatePlayerCount()
    {
        if (networkManager == null ||
            !networkManager.IsListening)
        {
            playerCountText.text = "Players: 0";
            return;
        }

        playerCountText.text =
            "Players: " +
            networkManager.ConnectedClientsIds.Count;
    }

    private void ShowNetworkHUD()
    {
        joinPanel.SetActive(false);
        networkHUD.SetActive(true);
        UpdatePlayerCount();
    }

    private void ShowJoinPanel()
    {
        joinPanel.SetActive(true);
        networkHUD.SetActive(false);
    }

    public void LeaveGame()
    {
        if (networkManager != null &&
            networkManager.IsListening)
        {
            networkManager.Shutdown();
        }

        ShowJoinPanel();
        statusText.text = "You left the session.";
    }

    private void OnEscapePressed(InputAction.CallbackContext context)
    {
        if (networkManager != null &&
            networkManager.IsListening)
        {
            LeaveGame();
        }
    }

    private void OnDestroy()
    {
        escapeAction.performed -= OnEscapePressed;
        escapeAction.Disable();
        escapeAction.Dispose();

        if (networkManager == null)
            return;

        networkManager.OnClientConnectedCallback -=
            OnClientConnected;

        networkManager.OnClientDisconnectCallback -=
            OnClientDisconnected;

        networkManager.OnTransportFailure -=
            OnTransportFailure;
    }
}
