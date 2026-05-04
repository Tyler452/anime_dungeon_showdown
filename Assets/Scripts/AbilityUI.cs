using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    public Image cooldownFill;
    public float cooldownTime = 5f;

    private float cooldownTimer = 0f;
    private bool isCoolingDown = false;

    void Start()
    {
        cooldownFill.fillAmount = 1f; // starts READY (full bar)
    }

    void Update()
    {
        if (!isCoolingDown) return;

        cooldownTimer -= Time.deltaTime;

        float progress = 1f - (cooldownTimer / cooldownTime);
        cooldownFill.fillAmount = progress;

        if (cooldownTimer <= 0f)
        {
            isCoolingDown = false;
            cooldownFill.fillAmount = 1f; // back to FULL (ready)
        }
    }

    public void UseAbility()
    {
        if (isCoolingDown) return;

        isCoolingDown = true;
        cooldownTimer = cooldownTime;

        cooldownFill.fillAmount = 0f; // instantly empty when used
    }

    public bool IsReady()
    {
        return !isCoolingDown;
    }
}