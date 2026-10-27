using UnityEngine;
using Unity.Netcode;
using TMPro;
public class MatchUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private GameObject matchHUD;
    [SerializeField] private GameObject winnerPanel;

    [Header("HUD")]
    [SerializeField] private TMP_Text playersLeftText;

    [Header("Winner Screen")]
    [SerializeField] private TMP_Text winnerNameText;

    private NetworkMatchState matchState;

    private void Start()
    {
        matchHUD.SetActive(false);
        winnerPanel.SetActive(false);
    }

    private void Update()
    {
        if (matchState == null)
        {
            matchState = FindObjectOfType<NetworkMatchState>();
        }

        NetworkManager network = NetworkManager.Singleton;

        if (network == null || !network.IsListening ||
            matchState == null)
        {
            return;
        }

        joinPanel.SetActive(false);

        if (matchState.MatchOver.Value)
        {
            matchHUD.SetActive(false);
            winnerPanel.SetActive(true);

            string winner =
                matchState.WinnerName.Value.ToString();

            winnerNameText.text =
                winner == "No winner"
                ? winner
                : winner + " Wins!";
        }
        else
        {
            winnerPanel.SetActive(false);
            matchHUD.SetActive(true);

            playersLeftText.text =
                "Players Remaining: " +
                matchState.PlayersRemaining.Value;
        }
    }

    public void PlayAgain()
    {
        if (matchState == null)
            return;

        if (NetworkManager.Singleton == null ||
            !NetworkManager.Singleton.IsListening)
            return;

        matchState.RestartMatchServerRpc();
    }

    public void LeaveGame()
    {
        if (NetworkManager.Singleton != null &&
            NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        winnerPanel.SetActive(false);
        matchHUD.SetActive(false);
        joinPanel.SetActive(true);
    }
}
