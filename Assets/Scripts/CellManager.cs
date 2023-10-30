using UnityEngine;
using UnityEngine.UI;

using static Constants;

public class CellManager : MonoBehaviour
{
    #region Singleton
    
    public static CellManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        this.Initialize();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    #endregion

    private readonly RectTransform[] cells = new RectTransform[CellPerLine * CellPerLine];

    public GameObject cellSource;

    public Color oddCellColor;
    public Color evenCellColor;
    
    private void Initialize()
    {
        var rectTransform = this.GetComponent<RectTransform>();
        var boardSizeDelta = rectTransform.sizeDelta;
        var cellSize = boardSizeDelta.x / CellPerLine;
        var cellSizeHalf = cellSize * 0.5f;
        
        var cellSizeDelta = new Vector2(cellSize, cellSize);
        var origin = new Vector3(-boardSizeDelta.x * 0.5f + cellSizeHalf, boardSizeDelta.x * 0.5f - cellSizeHalf, 0f);
        
        for (var y = 0; y < CellPerLine; y++)
        {
            for (var x = 0; x < CellPerLine; x++)
            {
                var cell = Instantiate(this.cellSource, this.transform).GetComponent<RectTransform>();
                cell.gameObject.SetActive(true);
                cell.sizeDelta = cellSizeDelta;
                cell.localPosition = new Vector3(origin.x + cellSize * x, origin.y - cellSize * y);
                cell.GetComponent<Image>().color = (x + y) % 2 == 0
                    ? this.evenCellColor
                    : this.oddCellColor;

                this.cells[ToIndex(x, y)] = cell;
            }
        }
    }

    public Vector3 GetPosition(int x, int y) => this.cells[ToIndex(x, y)].position;
}