using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuitManager : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #endif
    }
}