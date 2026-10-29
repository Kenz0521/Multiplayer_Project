using UnityEngine;

public class WaitingCameraScript : MonoBehaviour
{
    public void OnPlayerJoined()
    {
        this.gameObject.SetActive(false);
    }
}
