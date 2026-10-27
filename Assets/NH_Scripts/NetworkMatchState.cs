using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.SceneManagement;

public class NetworkMatchState : MonoBehaviour
{
    public NetworkVariable<int> PlayersRemaining =
        new NetworkVariable<int>(0);

    public NetworkVariable<bool> MatchOver =
        new NetworkVariable<bool>(false);

    public NetworkVariable<FixedString64Bytes> WinnerName =
        new NetworkVariable<FixedString64Bytes>(
            new FixedString64Bytes("")
        );

    private DirectorAI director;

    public override void OnNetworkSpawn()
    {
        director = FindObjectOfType<DirectorAI>();

        if (IsServer)
        {
            PlayersRemaining.Value = 0;
            MatchOver.Value = false;
            WinnerName.Value = new FixedString64Bytes("");
        }
    }

    private void Update()
    {
        // Only the host/server updates the shared match state.
        if (!IsServer)
            return;

        if (director == null)
        {
            director = FindObjectOfType<DirectorAI>();
            return;
        }

        PlayersRemaining.Value = director.LivingPlayers;

        if (director.MatchEnded && !MatchOver.Value)
        {
            MatchOver.Value = true;

            WinnerName.Value = new FixedString64Bytes(
                director.WinnerName
            );
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RestartMatchServerRpc()
    {
        if (!IsServer || NetworkManager.Singleton == null)
            return;

        string sceneName = SceneManager.GetActiveScene().name;

        NetworkManager.Singleton.SceneManager.LoadScene(
            sceneName,
            LoadSceneMode.Single
        );
    }
}