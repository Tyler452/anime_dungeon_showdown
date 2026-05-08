using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class AutoTile : MonoBehaviour
{
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();

        // Creates a unique material instance for this object
        Material mat = rend.material;

        Vector3 scale = transform.localScale;

        // Adjust these depending on wall direction
        mat.mainTextureScale = new Vector2(scale.x, scale.y);
    }
}