using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepModel
{

    public int score;        //分数
    public int bestScore;    //最高分

    public int[][] numbers;   //二维数组保存对应的数字

    //更新数据
    public void UpdateData( int score, int bestScore, MyGrid[][] grids )
    {
        this.score = score;
        this.bestScore = bestScore;

        if (numbers == null)
        {
            numbers = new int[grids.Length][];
        }

        for (int i = 0; i < grids.Length; i++)
        {
            for (int j = 0; j < grids[i].Length; j++)
            {
                if (numbers[i] == null)
                {
                    numbers[i] = new int[grids[i].Length];
                }
                numbers[i][j] = grids[i][j].IsHaveNumber() ? grids[i][j].GetNumber().GetNumber():0;
            }
        }
    }
}
