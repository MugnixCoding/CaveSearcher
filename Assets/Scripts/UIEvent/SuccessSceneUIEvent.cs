using UnityEngine;

public class SuccessSceneUIEvent : MonoBehaviour
{
    public void OnAgain_button_Click()
    {
        Loader.Instance.Load(Loader.Scene.DungeonScene,true);
    }
    public void OnMenu_button_Click()
    {
        Loader.Instance.Load(Loader.Scene.MenuScene, true);
    }
    public void OnEndGame_button_Click()
    {
        Application.Quit();
    }
}
