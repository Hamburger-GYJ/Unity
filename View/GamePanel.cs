using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    
public class GamePanel : MonoBehaviour
{

    Text text_Score;      //分数
    Text text_BestScore;  //最高分
    Button btn_LastStep;  //上一步
    Button btn_Restart;   //重新开始
    Button btn_ExitStep;  //下一步

    Transform gridParents;       //格子父物体
    Dictionary<int, int> grid_config = new Dictionary<int, int>() { { 4, 100 }, { 5, 80 }, { 6, 65 } };

    private int row;       //行
    private int column;    //列
    public MyGrid[][] grids = null;  //所有的格子
    public List<MyGrid> canCreateNumberGrid = new List<MyGrid>();  // 可以创建数字的格子

    GameObject gridPrefab;
    GameObject numberPrefab;

    private Vector3 pointerDownPos, pointerUpPos;

    //一开始就初始化格子
    private void Awake()
    {
        gridParents = transform.Find("Grid");
        gridPrefab = Resources.Load<GameObject>("Prefabs/Item");
        numberPrefab = Resources.Load<GameObject>("Prefabs/Number");
        InitGrid();
        CreateNumber();
    }

    // Start is called before the first frame update
    void Start()
    {
        text_Score = transform.Find("Score/Text (1)").GetComponent<Text>();
        text_BestScore = transform.Find("BestScore/Text (1)").GetComponent<Text>();
        btn_LastStep = GetComponent<Button>();
        btn_Restart = GetComponent<Button>();
        btn_ExitStep = GetComponent<Button>();
    }

    //上一步
    public void OnLastClick()
    {

    }

    //重新开始
    public void OnRestartClick()
    {

    }

    //退出
    public void OnExitClick()
    {

    }

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
        GameObject gameObject =  GameObject.Instantiate(gridPrefab, gridParents);

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
                    canCreateNumberGrid.Add(grids[i][j]);
                }
            }
        }

        //如果可创建数字的格子为0，就不创建数字了
        if ( canCreateNumberGrid.Count == 0 )
        {
            return;
        }

        //随机一个格子
        int index = Random.Range(0, canCreateNumberGrid.Count);

        //在随机的格子里创建数字
        GameObject gameObj =  GameObject.Instantiate(numberPrefab, canCreateNumberGrid[index].transform);
        gameObj.GetComponent<Number>().Init(canCreateNumberGrid[index]);

        //创建数字 将数字放入
        
    }

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
        if (Vector3.Distance(pointerDownPos,pointerUpPos) < 100 )
        {
            Debug.Log("无效操作");
            return;
        }

        //计算移动类型
        MoveType moveType = CaculateMoveType();
        Debug.Log("移动类型：" + moveType);
        MoveNumber(moveType);

        //产生数字
        CreateNumber();
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
    public void MoveNumber( MoveType moveType)  //参数知晓移动类型
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
                    for (int j = 1; j < column; j++ )
                    {
                        //判断格子是否有数字  有数字就进行 接下来循环（格子里是否有数字/有数字是否要合并格子并销毁数字）
                        if (grids[i][j].IsHaveNumber())
                        {

                            Number number = grids[i][j].GetNumber();

                            //Debug.Log("坐标：" + i + "," + j);
                            for (int m = j-1; m >= 0; m--)
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

                            Debug.Log("坐标：" + i + "," + j);

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
                    for (int i = row-1; i >=0 ; i--)      
                    {
                        //判断格子是否有数字  有数字就进行 接下来循环（格子里是否有数字/有数字是否要合并格子并销毁数字）
                        if (grids[i][j].IsHaveNumber())
                        {

                            Number number = grids[i][j].GetNumber();

                            //Debug.Log("坐标：" + i + "," + j);
                            for (int m = i+1; m < row; m++)
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
    public void HandelNumber( Number current, Number target,MyGrid targetGrid)
    {
        if (target != null)
        { 
            //判断 是否可以合并
            if (current.GetNumber() == target.GetNumber())
            {
                target.Merge();

                //销毁当前数字
                current.GetGrid().SetNumber(null);
                GameObject.Destroy(current.gameObject);
            }
        }
        else
            {
            //没有数字
                current.MoveToGrid(targetGrid);
            }
    }
}
