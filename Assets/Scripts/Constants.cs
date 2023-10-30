public static class Constants
{
    public const int CellPerLine = 4;
    
    public static int ToIndex(int x, int y) => y * CellPerLine + x;
    public static (int x, int y) ToPoint(int idx) => (idx % CellPerLine, idx / CellPerLine);
}