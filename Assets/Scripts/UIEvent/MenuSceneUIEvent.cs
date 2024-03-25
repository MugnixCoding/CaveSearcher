using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneUIEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject ExplainPanel;
    public void StartGameBtnClick()
    {
        Loader.Instance.Load(Loader.Scene.DungeonScene);
    }
    public void OpenExplainPanel()
    {
        ExplainPanel.SetActive(true);
    }
    public void CloseExplainPanel()
    {
        ExplainPanel.SetActive(false);
    }
    public void LeaveGame()
    {
        Application.Quit();
    }
}
