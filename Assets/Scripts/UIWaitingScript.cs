using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIWaitingScript : MonoBehaviour
{
    [SerializeField] GameObject joinPanel;
    [SerializeField] GameObject waitingPanel;

    private void Awake()
    {
        joinPanel.SetActive(true);
        waitingPanel.SetActive(false);
    }
    public void EnableWaitingPanel()
    {
        joinPanel.SetActive(false);
        waitingPanel.SetActive(true);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
