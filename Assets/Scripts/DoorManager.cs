using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorManager : MonoBehaviour {
    public GameObject[] doors;
    public string[] Dungeons;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Door") {
            Debug.Log("Touched the door");
            int randomDungeon = Random.Range(0, Dungeons.Length);
            SceneManager.LoadScene(Dungeons[randomDungeon]);
        }
    }
}
