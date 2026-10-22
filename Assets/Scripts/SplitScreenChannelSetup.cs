using NUnit.Framework.Internal.Commands;
using Unity.Cinemachine;
using UnityEngine;

public class SplitScreenChannelSetup : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] CinemachineBrain localBrain;
    [SerializeField] CinemachineCamera localVirtualCamera;

    private PlayerInputScript playerInputScript;

    private void Awake()
    {
        if (playerInputScript == null)
            playerInputScript = GetComponent<PlayerInputScript>();

        ConfigureCinemachineChannels(playerInputScript.GetPlayerIndex() - 1);
    }
    void ConfigureCinemachineChannels(int playerIndex)
    {
        if (localBrain == null || localVirtualCamera == null)
        {
            Debug.LogError("Missing Cinemachine components on prefab elements.", this);
            return;
        }
        OutputChannels targetChannel = (OutputChannels)(1 << playerIndex);

        localVirtualCamera.OutputChannel = targetChannel;

        localBrain.ChannelMask = targetChannel;
    }
}
