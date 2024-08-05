using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NineQueen : MonoBehaviour
{
    private const int QUEEN_COUNT = 9;

    public GridLayoutGroup grid;
    public TextMeshProUGUI txtCurPage;

    public Button mBtnPrev;
    public Button mBtnNext;


    private Button[,] GridBtns = new Button[QUEEN_COUNT, QUEEN_COUNT];
    private int curShowPage = 0;

    private List<int[]> solutions = new List<int[]>();

    private void Awake()
    {
        mBtnPrev.onClick.AddListener(OnClickPrevPage);
        mBtnNext.onClick.AddListener(OnClickNextPage);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitGridButtons();

        GetSolutions(QUEEN_COUNT);

        ShowSolutionByIndex(curShowPage);
    }

    void InitGridButtons()
    {
        Button[] btns = grid.GetComponentsInChildren<Button>();

        for(int idx = 0; idx < btns.Length;idx++)
        {
            int x = idx / QUEEN_COUNT;
            int y = idx % QUEEN_COUNT;

            GridBtns[x, y] = btns[idx];
        }
    }

    List<int[]> GetSolutions(int count)
    {
        solutions = new List<int[]>();

        List<int> queenList = new List<int>();
        for (int idx = 0; idx < count; idx++)
        {
            queenList.Add(0);
        }

        PutQueen(count, queenList, 0);

        return solutions;
    }

    void PutQueen(int queenCount,List<int> queenList,int nextY)
    {
        for(queenList[nextY] = 0;queenList[nextY] < queenCount;queenList[nextY]++)
        {
            if(!CheckConflict(queenList,nextY))
            {
                if(nextY + 1 < QUEEN_COUNT)
                {
                    PutQueen(queenCount, queenList, nextY + 1);
                }
                else
                {
                    solutions.Add(queenList.ToArray());
                }
            }
        }
    }

    bool CheckConflict(List<int> queenList,int nextY)
    {
        for(int positionY = 0; positionY < nextY;positionY++)
        {
            int x = Mathf.Abs(queenList[positionY] - queenList[nextY]);
            int y = Mathf.Abs(positionY - nextY);
            if (x == y || queenList[positionY] == queenList[nextY])
            {
                return true;
            }
        }
        return false;
    }

    void ShowSolutionByIndex(int page)
    {
        if (page < 0 || page > solutions.Count) return;

        txtCurPage.text = string.Format("Current Page Is : {0}", page);

        int[] solution = solutions[page];
        int queenIndex = 0;

        for(int i = 0; i < QUEEN_COUNT;i++)
        {
            for(int idx = 0; idx < QUEEN_COUNT; idx++)
            {
                if (GridBtns[i, idx] == null) continue;

                if(i== queenIndex && idx == solution[queenIndex])
                {
                    GridBtns[i, idx].GetComponentInChildren<TextMeshProUGUI>().text = "x";
                }
                else
                {
                    GridBtns[i, idx].GetComponentInChildren<TextMeshProUGUI>().text = "";
                }
            }
            queenIndex++;
        }
    }

    private void OnClickPrevPage()
    {
        curShowPage = Mathf.Max(0, --curShowPage);
        ShowSolutionByIndex(curShowPage);
    }

    public void OnClickNextPage()
    {
        curShowPage = Mathf.Min(solutions.Count - 1, ++curShowPage);
        ShowSolutionByIndex(curShowPage);
    }
}
