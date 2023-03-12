using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationGridLayout : LayoutGroup
{
    [System.Serializable]
    public class NavigationGridConnections
    {
        public NavigationGridLayout rightNavigationGrid, leftNavigationGrid, upNavigationGrid, downNavigationGrid;
    }

    public enum FitType { UNIFORM, WIDTH, HEIGHT, FIXED_ALL, FIXED_ROWS, FIXED_COLUMNS, /*FIXED_FIT,*/ NONE }
    public enum NavigationMode { NONE, EVERYTHING, HORIZONTAL, VERTICAL, AUTOMATIC, EXPLICIT, BY_NAVIGATION_GRID, IGNORE }

    public FitType fitType;
    public NavigationMode navigationMode = NavigationMode.BY_NAVIGATION_GRID;
    public int rows, columns;
    public Vector2 cellSize;
    public Vector2 spacing;

    public bool
        keepRowsAndColsValue = false;
    bool
        fitX = false,
        fitY = false;

    [Header("Connect with Other Grids")]
    public NavigationGridConnections navConnections;


    internal int NumOfGridElements { get { return transform.childCount; } }
    internal int LastRowNumOfGridElementsDiff = 0;


    protected override void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        LastRowNumOfGridElementsDiff = (rows * columns) - transform.childCount;

        for (int i = 0; i < transform.childCount; i++)
        {
            NavigationGridChild navChild = transform.GetChild(i).GetComponent<NavigationGridChild>();
            if (navChild == null)
            {
                Debug.LogWarning("NavigationGridChild not found on iteration " + i);
                continue;
            }
            int navChildActualCols = 0, navChildActualRows = 0, tmpIdx = i;
            while (tmpIdx >= columns) { tmpIdx -= columns; navChildActualRows++; }
            navChildActualCols = tmpIdx;
            Vector2Int navChildId = new Vector2Int(navChildActualCols, navChildActualRows);
            navChild.Initialize(this, navChildId);
        }

    }


    public Transform GetNavigationChildById(Vector2Int _navChildId)
    {
        if (_navChildId.x < 0 || _navChildId.y < 0 || _navChildId.x >= columns || _navChildId.y >= rows)
        {
            Debug.LogWarning("NavChildId was out of range. Make sure to tick KeepRowsAndColsValue if you if you want to have fixed values.");
            return null;
        }

        int actualIdx = _navChildId.x + _navChildId.y * columns;
        if (actualIdx >= NumOfGridElements) return null;

        return transform.GetChild(actualIdx);
    }


    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (fitType == FitType.NONE) return;

        if (fitType == FitType.UNIFORM || fitType == FitType.WIDTH || fitType == FitType.HEIGHT)
        {
            fitX = fitY = true;
            float sqrRt = Mathf.Sqrt(transform.childCount);
            if (!keepRowsAndColsValue)
            {
                rows = Mathf.CeilToInt(sqrRt);
                columns = Mathf.CeilToInt(sqrRt);
            }
        }

        if(fitType == FitType.WIDTH || fitType == FitType.FIXED_COLUMNS || fitType == FitType.FIXED_ALL /*|| fitType == FitType.FIXED_FIT*/)
        {
            if (!keepRowsAndColsValue) rows = Mathf.CeilToInt(transform.childCount / (float)columns);
        }
        if(fitType == FitType.HEIGHT || fitType == FitType.FIXED_ROWS || fitType == FitType.FIXED_ALL /*|| fitType == FitType.FIXED_FIT*/)
        {
            if (!keepRowsAndColsValue) columns = Mathf.CeilToInt(transform.childCount / (float)rows);
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = (parentWidth / (float)columns) - ((spacing.x / (float)columns) * 2) - (padding.left / (float)columns) - (padding.right / (float)columns);
        float cellHeight = (parentHeight / (float)rows) - ((spacing.y / (float)rows) * 2) - (padding.top / (float)rows) - (padding.bottom / (float)rows);

        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;


        for (int i = 0; i < rectChildren.Count; i++)
        {
            int rowCount = i / columns;
            int columnCount = i % columns;

            RectTransform item = rectChildren[i];
            float posX = (cellSize.x * columnCount) + (spacing.x * columnCount);
            float posY = (cellSize.y * rowCount) + (spacing.y * rowCount);

            SetChildAlongAxis(item, 0, posX, cellSize.x);
            SetChildAlongAxis(item, 1, posY, cellSize.y);

        }


    }

    public override void CalculateLayoutInputVertical()
    {
        //throw new System.NotImplementedException();
    }

    public override void SetLayoutHorizontal()
    {
        //throw new System.NotImplementedException();
    }

    public override void SetLayoutVertical()
    {
        //throw new System.NotImplementedException();
    }
}
