using UnityEngine;

public partial class GameManager
{
    [SerializeField] private GameObject gameOverPanel;

    private bool isPlaying;
    
    private void Awake()
    {
        this.Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.Exit();
        }
            
        this.UpdateInput();
    }
    
    public void Initialize()
    {
        this.gameOverPanel.SetActive(false);
        this.isPlaying = true;
            
        for (var i = 0; i < Constants.CellPerLine * Constants.CellPerLine; i++)
        {
            if (!this.tiles[i]) continue;
            Destroy(this.tiles[i].gameObject);
            this.tiles[i] = null;
        }
            
        for (var i = 0; i < this.spawnOnInit; i++)
        {
            this.Spawn();
        }
    }

    public void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}