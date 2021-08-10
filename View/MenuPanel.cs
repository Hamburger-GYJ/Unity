using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{

    SelectModePanel selectModePanel;
    SetPanel setPanel;
    Button btn_StartGame;
    Button btn_Set;
    Button btn_ExitGame;

    AudioClip bgClip;

    
    private void Awake()
    {
        bgClip = Resources.Load<AudioClip>("Water");
    }

    private void Start()
    {

        AudioManager._instance.PlayMusic(bgClip);
        selectModePanel = GameObject.Find("Canvas/SelectModePanel").GetComponent<SelectModePanel>();
        selectModePanel.transform.gameObject.SetActive(false);
        setPanel = GameObject.Find("Canvas/SetPanel").GetComponent<SetPanel>();
        setPanel.transform.gameObject.SetActive(false);
        btn_StartGame = transform.Find("Btn_Grid/Btn_StartGame").GetComponent<Button>();
        btn_Set = transform.Find("Btn_Grid/Btn_Set").GetComponent<Button>();
        btn_ExitGame = transform.Find("Btn_Grid/Btn_ExitGame").GetComponent<Button>();
        btn_StartGame.onClick.AddListener(OnStartGameClick);
        btn_Set.onClick.AddListener(OnSetClick);
        btn_ExitGame.onClick.AddListener(OnExitGameClick);

    }

    //点击开始游戏
    public void OnStartGameClick()
    {
        //显示选择界面
        selectModePanel.Show();
    }

    //点击设置
    public void OnSetClick()
    {
        //显示设置界面
        setPanel.Show();
    }

    //点击退出游戏
    public void OnExitGameClick()
    {
        //退出游戏
        Application.Quit();
    }
}
