using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    public void GoToMain()
    {
        SceneManager.LoadScene(1);
    }
}
