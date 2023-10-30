using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    private static TileConfig config;
    
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    
    public int Value { get; private set; }

    private void Awake()
    {
        config ??= Resources.Load<TileConfig>("TileConfig");
        this.SetValue(2);
    }

    public void SetValue(int newValue)
    {
        var color = config.GetColor(newValue);
        
        this.Value = newValue;
        this.image.color = color.background;
        this.text.color = color.foreground;
        this.text.faceColor = color.foreground;
        this.text.text = newValue.ToString();
    }
}