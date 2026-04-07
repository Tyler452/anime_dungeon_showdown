using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSwitch : MonoBehaviour {
    
    public GameObject OptionsHolder;

    public void OnButtonClick()
    {
        SceneManager.LoadScene(sceneName:"TestScene");
        //This will be different in the final game
    }

    public void SettingsMenuOn() {
        OptionsHolder.SetActive (true);
    }
    
    public void SettingsMenuOff() {
        OptionsHolder.SetActive (false);
    }
}
