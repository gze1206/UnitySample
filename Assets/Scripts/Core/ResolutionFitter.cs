using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ResolutionFitter : MonoBehaviour
{
    public int targetAspectWidth;
    public int targetAspectHeight;
    
    private void Awake()
    {
        var cam = GetComponent<Camera>();
        var aspectRatio = Screen.width / (float)Screen.height;
        var targetRatio = this.targetAspectWidth / (float)this.targetAspectHeight;
        cam.orthographicSize *= targetRatio / aspectRatio;
    }
}
