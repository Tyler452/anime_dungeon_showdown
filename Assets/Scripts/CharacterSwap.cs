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
        // Pick which character starts
        activeCharacter = startWithA ? characterA : characterB;

        // Enable one and disable the other
        if (characterA != null && characterB != null)
        {
            characterA.SetActive(startWithA);
            characterB.SetActive(!startWithA);
        }

        // Get camera + follow script
        mainCamera = Camera.main;
        if (mainCamera != null)
            camFollow = mainCamera.GetComponent<CameraFollow>();

        // --- tell camera who to follow right away ---
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

        // Figure out who’s next
        GameObject nextCharacter = (activeCharacter == characterA) ? characterB : characterA;

        // Keep position/rotation if you want a true swap
        Vector3 currentPos = activeCharacter.transform.position;
        Quaternion currentRot = activeCharacter.transform.rotation;

        activeCharacter.SetActive(false);
        nextCharacter.SetActive(true);
        nextCharacter.transform.SetPositionAndRotation(currentPos, currentRot);

        // Update the active one
        activeCharacter = nextCharacter;

        // --- update the camera target immediately ---
        if (camFollow != null)
            camFollow.SetTarget(activeCharacter.transform);

        Debug.Log("Swapped to: " + activeCharacter.name);
    }
}