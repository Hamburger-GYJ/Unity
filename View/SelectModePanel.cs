using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SelectModePanel : View,IPointerClickHandler
{

    GameObject backGround;

    private void Start()
    {
        backGround = transform.Find("BackGround").gameObject;
    }

    //点击选择模式
    public void OnSelectModeClick( int count )
    {
        //选择模式 保存到硬盘
        PlayerPrefs.SetInt(Const.GameModel, count);

        //跳转场景 到 游戏场景
        SceneManager.LoadSceneAsync(1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerEnter == backGround)
        {
            Hide();
        }
    }
}
