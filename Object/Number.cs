using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Number : MonoBehaviour
{
    Image bg;
    Text number_text;
    MyGrid inGrid;    //这个数字所在的格子

    public NumberStatus status { get; set; }
    
    private float spawnScaleTime = 1;
    private bool isPlayingSpawnAnim = false;

    private float mergeSceleTime = 1;
    private float mergeSceleTimeBack = 1;
    private bool isPlayingMergeAnim = false;

    private float movePosTime = 1;
    private bool isMoving = false;
    private bool isDestroyOnMoveEnd = false; 
    private Vector3 startMove, endMovePos;

    public Color[] bg_colors;
    public List<int> number_index;

    AudioClip mergeClip;

    private void Awake()
    {
        bg = GetComponent<Image>();
        number_text = transform.Find("Text").GetComponent<Text>();
    }

    private void Start()
    {
        mergeClip = Resources.Load<AudioClip>("AudioOfMerge");

    }

    private void Update()
    {
        //创建动画——时变大动画
        if (isPlayingSpawnAnim)
        {
            if (spawnScaleTime <= 1)
            {
                spawnScaleTime += Time.deltaTime * 4;
                transform.localScale = Vector3.Lerp(Vector3.one * 0.3f, Vector3.one, spawnScaleTime);
            }
        }
        else
        {
            isPlayingSpawnAnim = false;
        }

        //合并动画
        if (isPlayingMergeAnim)
        {
            //销毁时先变大在缩小——变大过程
            if (mergeSceleTime <= 1 && mergeSceleTimeBack == 0)
            {
                mergeSceleTime += Time.deltaTime * 4;
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, mergeSceleTime);
            }
            //销毁时先变大在缩小——缩小过程
            if (mergeSceleTime >= 1 && mergeSceleTimeBack <= 1)
            {
                mergeSceleTime += Time.deltaTime * 4;
                transform.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.one, mergeSceleTime);
            }
            if (mergeSceleTime >= 1 && mergeSceleTimeBack >=1 )
            {
                isPlayingMergeAnim = false;
            }
        }

        //移动动画
        if ( isMoving )
        {
            movePosTime += Time.deltaTime * 4;
            transform.localPosition = Vector3.Lerp(startMove, Vector3.zero, movePosTime);
            if (movePosTime >= 1)
            {
                isMoving = false;
                if (isDestroyOnMoveEnd)
                {
                    GameObject.Destroy(gameObject);
                }
            }
        }

    } 



    //初始化
    public void Init( MyGrid myGrid )
    {
        myGrid.SetNumber(this);
        // 设置所在的格子
        this.SetGrid(myGrid);
        //初始化数字
        this.SetNumber(2);
        status = NumberStatus.Normal;

        PlaySpawnAnim();  //播放动画
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
        
        this.bg.color = this.bg_colors[number_index.IndexOf(number)];
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
        //transform.localPosition = Vector3.zero;    //局部坐标设置为0，0
        startMove = transform.localPosition;
        //endMovePos = myGrid.transform.position;

        movePosTime = 0; 
        isMoving = true;

        this.GetGrid().SetNumber(null);

        //设置格子
        myGrid.SetNumber(this);
        this.SetGrid(myGrid);
    }

    //合并数字
    public void Merge()
    {
        GamePanel gamePanel = GameObject.Find("Canvas/GamePanel").GetComponent<GamePanel>();
        gamePanel.AddScore(this.GetNumber());

        int number = this.GetNumber() * 2;
        this.SetNumber(number);
        if (number == 2048)
        {
            //游戏胜利
            gamePanel.GameWin();
        }

        status = NumberStatus.NotMerge;
        //播放合并动画
        PlayMergeAnim();

        //播放音效
        AudioManager._instance.PlaySound(mergeClip);

    }

    //判断能不能合并
    public bool IsMerge( Number number)
    {
        if (this.GetNumber() == number.GetNumber() && number.status == NumberStatus.Normal)
        {
            return true;
        }
        return false;
    }

    //播放创建动画
    public void PlaySpawnAnim()
    {
        spawnScaleTime = 0;
        isPlayingSpawnAnim = true;
    }

    //播放合并的动画
    public void PlayMergeAnim()
    {
        mergeSceleTime = 0;
        mergeSceleTimeBack = 0;
        isPlayingMergeAnim = true;
    }

    //移动后进行销毁
    public void DestroyOnEnd( MyGrid myGrid)
    {
        transform.SetParent(myGrid.transform);
        startMove = transform.localPosition;
        movePosTime = 0;
        isMoving = true;
        isDestroyOnMoveEnd = true;
    }
}
