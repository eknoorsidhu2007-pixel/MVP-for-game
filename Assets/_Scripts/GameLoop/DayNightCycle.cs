using Mirror;
using UnityEngine;

public class DayNightCycle : NetworkBehaviour
{
    [Header("Lighting")]
    public Light sunLight;
    public float dayIntensity = 1.2f;
    public float nightIntensity = 0.1f;
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.1f, 0.1f, 0.3f);

    [Header("Ambient")]
    public Color dayAmbient = new Color(0.4f, 0.4f, 0.4f);
    public Color nightAmbient = new Color(0.05f, 0.05f, 0.1f);

    [Header("Headlights")]
    public Light[] truckHeadlights;

    private void Update()
    {
        if (GameManager.Instance == null) return;

        float cycleTime = GameManager.Instance.GetCycleProgress();
        float dayDuration = GameManager.Instance.dayDuration;
        float nightDuration = GameManager.Instance.nightDuration;
        float totalCycle = dayDuration + nightDuration;

        bool isNight = GameManager.Instance.IsNight();

        // Rotate sun
        float sunAngle = (cycleTime / totalCycle) * 360f - 90f;
        sunLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

        // Light intensity and color
        if (!isNight)
        {
            float t = cycleTime / dayDuration;
            sunLight.intensity = Mathf.Lerp(0.2f, dayIntensity, Mathf.Sin(t * Mathf.PI));
            sunLight.color = dayColor;
            RenderSettings.ambientLight = dayAmbient;
            SetHeadlights(false);
        }
        else
        {
            float nightTime = cycleTime - dayDuration;
            float t = nightTime / nightDuration;
            sunLight.intensity = nightIntensity;
            sunLight.color = nightColor;
            RenderSettings.ambientLight = nightAmbient;
            SetHeadlights(true);
        }
    }

    private void SetHeadlights(bool on)
    {
        if (truckHeadlights == null) return;
        foreach (Light l in truckHeadlights)
        {
            if (l != null) l.enabled = on;
        }
    }
}