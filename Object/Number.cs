using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Number : MonoBehaviour
{
    Image bg;
    Text number_text;
    MyGrid inGrid;    //这个数字所在的格子

    private void Awake()
    {
        bg = GetComponent<Image>();
        number_text = transform.Find("Text").GetComponent<Text>();
    }


    //初始化
    public void Init( MyGrid myGrid )
    {
        myGrid.SetNumber(this);
        // 设置所在的格子
        this.SetGrid(myGrid);
        //初始化数字
        this.SetNumber(2);
    }


    //设置格子
    public void SetGrid(MyGrid myGrid)
    {
        this.inGrid = myGrid;
    }

    //获取格子
    public MyGrid GetGrid()
    {
        return this.inGrid;
    }

    //设置数字
    public void SetNumber(int number)
    {
        this.number_text.text = number.ToString();
    }

    //获取数字
    public int GetNumber()
    {
        return int.Parse(number_text.text);
    }

    //把数字移动到某一个格子的下面
    public void MoveToGrid( MyGrid myGrid)
    {
        transform.SetParent(myGrid.transform);     //设置父物体
        transform.localPosition = Vector3.zero;    //局部坐标设置为0，0

        this.GetGrid().SetNumber(null);

        //设置格子
        myGrid.SetNumber(this);
        this.SetGrid(myGrid);
    }

    //合并数字
    public void Merge()
    {
        this.SetNumber(this.GetNumber() * 2);
    }
}
