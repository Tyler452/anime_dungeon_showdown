using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    public AbilityUI ability1;
    public AbilityUI ability2;
    public AbilityUI ability3;
    public AbilityUI ability4;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && ability1.IsReady())
            ability1.UseAbility();

        if (Input.GetKeyDown(KeyCode.R) && ability2.IsReady())
            ability2.UseAbility();
    }
}