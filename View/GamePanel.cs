using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GamePanel : MonoBehaviour
{

    #region UI控件 
    Text text_Score;      //分数
    Text text_BestScore;  //最高分
    Button btn_LastStep;  //上一步
    Button btn_Restart;   //重新开始
    Button btn_ExitStep;  //下一步

    WinPanel winPanel; //赢的界面
    LosePanel losePanel; //输的界面
    #endregion

    #region 属性 变量
    Transform gridParents;       //格子父物体
    Dictionary<int, int> grid_config = new Dictionary<int, int>() { { 4, 100 }, { 5, 80 }, { 6, 65 } };

    private int row;       //行
    private int column;    //列
    public MyGrid[][] grids = null;  //所有的格子
    public List<MyGrid> canCreateNumberGrid = new List<MyGrid>();  // 可以创建数字的格子

    GameObject gridPrefab;
    GameObject numberPrefab;

    private Vector3 pointerDownPos, pointerUpPos;

    private bool isNeedCreateNumber = false;

    public int currentScore;
    public StepModel lastStepModel;

    AudioClip bgClip;

    #endregion

    #region 游戏周期


    private void Awake()
    {
        bgClip = Resources.Load<AudioClip>("Water");
        text_Score = transform.Find("Score/Text (1)").GetComponent<Text>();
        text_BestScore = transform.Find("BestScore/Text (1)").GetComponent<Text>();
        btn_LastStep =transform.Find("Btn_Last").GetComponent<Button>();
        btn_LastStep.onClick.AddListener(OnLastClick);
        btn_Restart = transform.Find("Btn_Restart").GetComponent<Button>();
        btn_Restart.onClick.AddListener(RestartGame);
        btn_ExitStep = transform.Find("Btn_Exit").GetComponent<Button>();
        btn_ExitStep.onClick.AddListener(ExitGame);
        winPanel = GameObject.Find("WinPanel").GetComponent<WinPanel>();
        losePanel = GameObject.Find("LosePanel").GetComponent<LosePanel>();
        gridParents = transform.Find("Grid");
        gridPrefab = Resources.Load<GameObject>("Prefabs/Item");
        numberPrefab = Resources.Load<GameObject>("Prefabs/Number");

        //初始化界面信息
        InitPanelMessage();

        //初始化格子
        InitGrid();
        //创建第一个数字
        CreateNumber();
    }

    #endregion


    #region 游戏逻辑

    //初始化格子
    public void InitGrid()
    {


        //获取格子数量
        int gridNum = PlayerPrefs.GetInt(Const.GameModel, 4);
        GridLayoutGroup gridLayoutGroup = transform.Find("Grid").GetComponent<GridLayoutGroup>();
        gridLayoutGroup.constraintCount = gridNum;
        gridLayoutGroup.cellSize = new Vector2(grid_config[gridNum], grid_config[gridNum]);

        //初始化格子的数量
        grids = new MyGrid[gridNum][];

        //创建格子
        row = gridNum;
        column = gridNum;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                //创建 i j 格子
                if (grids[i] == null)
                {
                    grids[i] = new MyGrid[gridNum];
                }
                grids[i][j] = CreateGrid();
            }
        }
    }

    //创建格子
    public MyGrid CreateGrid()
    {
        //实例化格子预制体
        GameObject gameObject = GameObject.Instantiate(gridPrefab, gridParents);

        return gameObject.GetComponent<MyGrid>();
    }

    //创建数字
    public void CreateNumber()
    {
        //找到这个数字所在的格子
        canCreateNumberGrid.Clear();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                //判断格子里是否有数字，如果没有数字，就将其加入到没有数字的集合中去
                if (!grids[i][j].IsHaveNumber())
                {
                    //判断这个格子里有没有数字
                    //如果没有数字
                    canCreateNumberGrid.Add(grids[i][j]);
                }
            }
        }

        //如果可创建数字的格子为0，就不创建数字了
        if (canCreateNumberGrid.Count == 0)
        {
            return;
        }

        //随机一个格子
        int index = Random.Range(0, canCreateNumberGrid.Count);

        //在随机的格子里创建数字
        GameObject gameObj = GameObject.Instantiate(numberPrefab, canCreateNumberGrid[index].transform);
        gameObj.GetComponent<Number>().Init(canCreateNumberGrid[index]);

        //创建数字 将数字放入

    }

    public void CreateNumber( MyGrid myGrid , int number )
    {
        GameObject gameObj
            = GameObject.Instantiate(numberPrefab, myGrid.transform);
        gameObj.GetComponent<Number>().Init(myGrid);
        gameObj.GetComponent<Number>().SetNumber(number);
    }

    //计算移动的类型
    public MoveType CaculateMoveType()
    {
        if (Mathf.Abs(pointerUpPos.x - pointerDownPos.x) > Mathf.Abs(pointerDownPos.y - pointerUpPos.y))
        {
            //左右移动
            if (pointerUpPos.x - pointerDownPos.x > 0)
            {
                //向右移动
                //Debug.Log("向右移动");
                return MoveType.RIGHT;
            }
            else
            {
                //向左移动
                //Debug.Log("向左移动");
                return MoveType.LEFT;
            }
        }
        else
        {
            //上下移动
            if (pointerUpPos.y - pointerDownPos.y > 0)
            {
                //向上移动
                //Debug.Log("向上移动");
                return MoveType.UP;
            }
            else
            {
                //向下移动
                //Debug.Log("向下移动");
                return MoveType.DOWN;
            }
        }
    }

    //移动数字
    public void MoveNumber(MoveType moveType)  //参数知晓移动类型
    {
        switch (moveType)
        {
            case MoveType.RIGHT:

                for (int i = 0; i < row; i++)
                {
                    for (int j = column - 2; j >= 0; j--)
                    {
                        //判断格子是否有数字  有数字就进行 接下来循环（格子里是否有数字/有数字是否要合并格子并销毁数字）
                        if (grids[i][j].IsHaveNumber())
                        {

                            Number number = grids[i][j].GetNumber();

                            //Debug.Log("坐标：" + i + "," + j);
                            for (int m = j + 1; m < column; m++)
                            {
                                Number targetNumber = null;
                                if (grids[i][m].IsHaveNumber())
                                {
                                    targetNumber = grids[i][m].GetNumber();
                                }

                                HandelNumber(number, targetNumber, grids[i][m]);

                                if (targetNumber != null)   //如果没有数字跳出循环
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
            case MoveType.LEFT:

                for (int i = 0; i < row; i++)
                {
                    for (int j = 1; j < column; j++)
                    {
                        //判断格子是否有数字  有数字就进行 接下来循环（格子里是否有数字/有数字是否要合并格子并销毁数字）
                        if (grids[i][j].IsHaveNumber())
                        {

                            Number number = grids[i][j].GetNumber();

                            //Debug.Log("坐标：" + i + "," + j);
                            for (int m = j - 1; m >= 0; m--)
                            {
                                Number targetNumber = null;
                                if (grids[i][m].IsHaveNumber())
                                {
                                    targetNumber = grids[i][m].GetNumber();
                                }

                                HandelNumber(number, targetNumber, grids[i][m]);

                                if (targetNumber != null)   //如果没有数字跳出循环
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
            case MoveType.UP:

                for (int j = 0; j < column; j++)
                {
                    for (int i = 0; i < row; i++)
                    {
                        //判断格子是否有数字  有数字就进行循环
                        if (grids[i][j].IsHaveNumber())
                        {

                            Number number = grids[i][j].GetNumber();

                            //Debug.Log("坐标：" + i + "," + j);

                            for (int m = i - 1; m >= 0; m--)
                            {

                                Number targetNumber = null;
                                if (grids[m][j].IsHaveNumber())
                                {
                                    targetNumber = grids[m][j].GetNumber();
                                }

                                HandelNumber(number, targetNumber, grids[m][j]);

                                if (targetNumber != null)   //如果没有数字跳出循环
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                break;
            case MoveType.DOWN:

                for (int j = 0; j < column; j++)
                {
                    for (int i = row - 2; i >= 0; i--)
                    {
                        //判断格子是否有数字  有数字就进行 接下来循环（格子里是否有数字/有数字是否要合并格子并销毁数字）
                        if (grids[i][j].IsHaveNumber())
                        {

                            Number number = grids[i][j].GetNumber();

                            //Debug.Log("坐标：" + i + "," + j);
                            for (int m = i + 1; m < row; m++)
                            {
                                Number targetNumber = null;
                                if (grids[m][j].IsHaveNumber())
                                {
                                    targetNumber = grids[m][j].GetNumber();
                                }
                                HandelNumber(number, targetNumber, grids[m][j]);

                                if (targetNumber != null)   //如果没有数字跳出循环
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
        }
    }

    //处理数字
    public void HandelNumber(Number current, Number target, MyGrid targetGrid)
    {
        if (target != null)
        {
            //判断 是否可以合并
            if (current.IsMerge(target))
            {
                target.Merge();

                //销毁当前数字
                current.GetGrid().SetNumber(null);
                //GameObject.Destroy(current.gameObject);
                current.DestroyOnEnd(target.GetGrid());
                isNeedCreateNumber = true;
            }
        }
        else
        {
            //没有数字
            current.MoveToGrid(targetGrid);
            isNeedCreateNumber = true;
        }
    }

    //重置数字的状态
    public void ResetNumberStatus()
    {
        //遍历所有数字
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (grids[i][j].IsHaveNumber())
                {
                    grids[i][j].GetNumber().status = NumberStatus.Normal;
                }
            }
        }
    }

    //判断游戏是否失败
    public bool IsGameLose()
    {
        //判断格子是否满了
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (!grids[i][j].IsHaveNumber())
                {
                    return false;
                }
            }
        }
        //判断有没有数字能够合并
        for (int i = 0; i < row; i += 2)
        {
            for (int j = 0; j < column; j ++)
            {
                MyGrid up = IsHaveGrid(i - 1, j) ? grids[i - 1][j] : null;
                MyGrid down = IsHaveGrid(i + 1, j) ? grids[i + 1][j] : null;
                MyGrid left = IsHaveGrid(i, j - 1) ? grids[i][j - 1] : null;
                MyGrid right = IsHaveGrid(i, j + 1) ? grids[i][j + 1] : null;

                if (up != null)
                {
                    if (grids[i][j].GetNumber().IsMerge(up.GetNumber()))
                    {
                        return false;
                    }
                }

                if (down != null)
                {
                    if (grids[i][j].GetNumber().IsMerge(down.GetNumber()))
                    {
                        return false;
                    }
                }

                if (left != null)
                {
                    if (grids[i][j].GetNumber().IsMerge(left.GetNumber()))
                    {
                        return false;
                    }
                }

                if (right != null)
                {
                    if (grids[i][j].GetNumber().IsMerge(right.GetNumber()))
                    {
                        return false;
                    }
                }
            }
        }
        return true; //游戏失败
    }
    
    //判断是否有空格子
    public bool IsHaveGrid(int i, int j)
    {
        if (i >= 0 && i<row && j>= 0 && j<column)
        {
            return true;
        }
        return false;
    }

    #endregion

    #region 事件监听

    //鼠标点击
    public void OnPointerDown()
    {
        pointerDownPos = Input.mousePosition;
    }

    //鼠标抬起
    public void OnPointerUp()
    {
        pointerUpPos = Input.mousePosition;

        //判断 移动距离小时是无效操作
        if (Vector3.Distance(pointerDownPos, pointerUpPos) < 100)
        {
            Debug.Log("无效操作");
            return;
        }

        //保存数据
        lastStepModel.UpdateData(this.currentScore, PlayerPrefs.GetInt(Const.BestScore, 0), grids);
        btn_LastStep.interactable = true;

        //计算移动类型
        MoveType moveType = CaculateMoveType();
        Debug.Log("移动类型：" + moveType);
        MoveNumber(moveType);

        //产生数字
        if (isNeedCreateNumber)
        {
            CreateNumber();
        }

        //把所有数字的状态恢复成正常状态
        ResetNumberStatus();
        isNeedCreateNumber = false;

        //判断游戏是否结束
        if (IsGameLose())
        {
            GameLose();
        }


    }

    //上一步
    public void OnLastClick()
    {
        BackToLastStep();
        btn_LastStep.interactable = false;
    }
    #endregion

    #region 界面更新

    //初始化界面信息
    public void InitPanelMessage()
    {
        this.text_BestScore.text = PlayerPrefs.GetInt(Const.BestScore, 0).ToString();
        lastStepModel = new StepModel();
        btn_LastStep.interactable = false;

        //播放音乐
        AudioManager._instance.PlayMusic(bgClip);
    }

    //分数增加
    public void AddScore( int score )
    {
        currentScore += score ;
        UpdateScore(currentScore);

        //判断当前分数是否是最高分
        if (currentScore > PlayerPrefs.GetInt(Const.BestScore,0))
        {
            PlayerPrefs.SetInt(Const.BestScore, currentScore);
            UpdateBestScore(currentScore);
        }
    }

    //更新分数
    public void UpdateScore( int score )
    {
        this.text_Score.text = score.ToString();
    }

    //重置分数
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScore(currentScore);
    }

    //最高分
    public void UpdateBestScore( int bestScore )
    {
        this.text_BestScore.text = bestScore.ToString();
    }

    #endregion

    #region 游戏流程

    //返回到上一步
    public void BackToLastStep()
    {
        //分数
        currentScore = lastStepModel.score;
        UpdateScore(lastStepModel.score);

        PlayerPrefs.SetInt(Const.BestScore, lastStepModel.bestScore);
        UpdateBestScore(lastStepModel.bestScore);

        //数字
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (lastStepModel.numbers[i][j] == 0 && grids[i][j].IsHaveNumber())
                {
                    if (grids[i][j].IsHaveNumber())
                    {
                        GameObject.Destroy(grids[i][j].GetNumber().gameObject);
                        grids[i][j].SetNumber(null);
                    }
                }
                else if (lastStepModel.numbers[i][j] != 0 )
                { 
                    if (grids[i][j].IsHaveNumber())
                    {
                        //修改数字
                        grids[i][j].GetNumber().SetNumber(lastStepModel.numbers[i][j]);
                    }
                    else
                    {
                        //创建数字
                        CreateNumber(grids[i][j],lastStepModel.numbers[i][j]);
                    }
                }
                
            }
        }
    }

    //重新开始
    public void RestartGame()
    {
        //数据清空
        btn_LastStep.interactable = false;
        //清空分数
        ResetScore();
        //清空数字
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (grids[i][j].GetNumber() != null)
                {
                    GameObject.Destroy(grids[i][j].GetNumber().gameObject);
                }
                grids[i][j].SetNumber(null);
            }
        }

        // 创建一个数字
        CreateNumber();
    }

    //退出游戏
    public void ExitGame()
    {
        SceneManager.LoadSceneAsync(0);
    }
    
    //游戏胜利
    public void GameWin()
    {
        Debug.Log("游戏胜利！");
        winPanel.Show();
    }

    //游戏失败
    public void GameLose()
    {
        Debug.Log("游戏失败！");
        losePanel.Show();
    }
    #endregion


}
