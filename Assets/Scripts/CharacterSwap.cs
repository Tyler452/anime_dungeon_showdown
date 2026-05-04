using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSwap : MonoBehaviour
{
    [Header("Characters")]
    public GameObject characterA;
    public GameObject characterB;

    [Header("Settings")]
    public KeyCode swapKey = KeyCode.F;
    public bool startWithA = true;

    private GameObject activeCharacter;
    private Camera mainCamera;
    private CameraFollow camFollow;

    void Start()
    {
        activeCharacter = startWithA ? characterA : characterB;

        if (characterA != null && characterB != null)
        {
            characterA.SetActive(startWithA);
            characterB.SetActive(!startWithA);
        }

        mainCamera = Camera.main;
        if (mainCamera != null)
            camFollow = mainCamera.GetComponent<CameraFollow>();

        if (camFollow != null && activeCharacter != null)
        {
            camFollow.SetTarget(activeCharacter.transform);
        }
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
                SwapCharacter();
        }
        else if (Input.GetKeyDown(swapKey))
        {
            SwapCharacter();
        }
    }

    void SwapCharacter()
    {
        if (characterA == null || characterB == null)
        {
            Debug.LogWarning("Character references missing!");
            return;
        }

        GameObject nextCharacter = (activeCharacter == characterA) ? characterB : characterA;

        Vector3 currentPos = activeCharacter.transform.position;
        Quaternion currentRot = activeCharacter.transform.rotation;

        activeCharacter.SetActive(false);
        nextCharacter.SetActive(true);
        nextCharacter.transform.SetPositionAndRotation(currentPos, currentRot);

        activeCharacter = nextCharacter;

        if (camFollow != null)
            camFollow.SetTarget(activeCharacter.transform);

        Debug.Log("Swapped to: " + activeCharacter.name);
    }
}