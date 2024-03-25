using UnityEngine;

public class DungeonSceneUIEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject PausedPanel;

    private bool isGamePause;
    private void Start()
    {
        InputManager.Instance.OnPauseAction += Pause;
        isGamePause = false;
        PausedPanel.SetActive(isGamePause);
    }
    /*
    private void OnDestroy()
    {
        InputManager.Instance.OnPauseAction -= Pause;
    }
    */

    private void Pause(object sender, System.EventArgs e)
    {
        isGamePause = !isGamePause;
        Time.timeScale = isGamePause ? 0f : 1f;
        PausedPanel.SetActive(isGamePause);

        Cursor.lockState = isGamePause ? CursorLockMode.None: CursorLockMode.Locked;
    }
    public void OnRestart_button_Click()
    {
        Time.timeScale = 1f;
        Loader.Instance.Load(Loader.Scene.DungeonScene, true);
    }
    public void OnMenu_button_Click()
    {
        Time.timeScale = 1f;
        Loader.Instance.Load(Loader.Scene.MenuScene, true);
    }
    public void OnLeaveGame_button_Click()
    {
        Application.Quit();
    }
    
}
