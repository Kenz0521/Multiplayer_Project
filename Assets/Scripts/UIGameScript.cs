using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameScript : MonoBehaviour
{
    [SerializeField] GameObject winningPanel;
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    public void PlayAgain()
    {
        winningPanel.SetActive(false);
        SceneManager.LoadScene(1);
    }
}
