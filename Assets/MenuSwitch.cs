using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSwitch : MonoBehaviour
{

    public void OnButtonClick()
    {
        SceneManager.LoadScene(sceneName:"TestScene");
        //This will be different in the final game
    }
}
