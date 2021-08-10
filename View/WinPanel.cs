using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanel : View
{
    Button btn_GameAgain;
    Button btn_BackToHome;

    private void Start()
    {
        Hide();
        btn_BackToHome = transform.Find("Btn_BackToHome").GetComponent<Button>();
        btn_BackToHome.onClick.AddListener(OnExitClick);
        btn_GameAgain = transform.Find("Btn_GameAgain").GetComponent<Button>();
        btn_GameAgain.onClick.AddListener(OnRestartClick);
    }

    //重新开始的按钮点击事件
    public void OnRestartClick()
    {
        //调用 GamePanel 里面的重新开始
        //Debug.Log("重新开始");

        GameObject.Find("Canvas/GamePanel").GetComponent<GamePanel>().RestartGame();
        Hide();
    }

    //退出按钮的点击事件
    public void OnExitClick()
    {
        //退出到菜单场景
        //Debug.Log("退出");
        SceneManager.LoadSceneAsync(0);
    }
}
