using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SetPanel :View
{

    Button btn_close;
    Slider slider_Sound;
    Slider slider_Music;
    

    private void Start()
    {
        btn_close = transform.Find("bg/Button").GetComponent<Button>();
        btn_close.onClick.AddListener(OnBtnDownClick);
        slider_Sound = transform.Find("Sound/Slider").GetComponent<Slider>();
        slider_Music = transform.Find("Music/Slider").GetComponent<Slider>();
        slider_Sound.onValueChanged.AddListener(OnSoundValueChanged);
        slider_Music.onValueChanged.AddListener(OnMusicValueChanged);
    }

    //关闭按钮
    public void OnBtnDownClick()
    {
        //当点界面隐藏
        Hide();
    }

    //改变音效大小
    public void OnSoundValueChanged(float f)
    {
        //修改音效大小
        AudioManager._instance.OnSoundVolumeChanged(f);
        //保存当前修改
        PlayerPrefs.SetFloat(Const.Sound,f);
    }

    //改变音乐大小
    public void OnMusicValueChanged(float f)
    {
        //修改音乐大小
        AudioManager._instance.OnMusicVolumeChanged(f);
        //保存当前修改
        PlayerPrefs.SetFloat(Const.Music, f);
        PlayerPrefs.SetFloat(Const.Music, f);
    }

    public override void Show()
    {
        base.Show();
        //对界面进行初始化
        slider_Sound.value = PlayerPrefs.GetFloat(Const.Sound,0);
        slider_Music.value = PlayerPrefs.GetFloat(Const.Music,0);
    }
}
