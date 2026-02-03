using UnityEngine;
using TMPro;
using System.Collections;

public class DaySystem : MonoBehaviour
{
    [Header("UI Elemanlarý")]
    public TextMeshProUGUI dayText;
    public CanvasGroup dayTextCanvasGroup;

    [Header("Zaman Ayarlarý")]
    public float dayDurationInSeconds = 600f;
    public float textDisplayDuration = 5f;

    [Header("Iþýk ve Atmosfer")]
    public Light sunLight;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;
    public Material skyboxMaterial;

    // SÝSÝ KAPATTIÐIMIZ ÝÇÝN CURVE VE SÝS AYARLARINI SÝLDÝK/PASÝFE ALDIK

    private float timer = 0f;
    private int currentDay = 1;
    private bool isTransitioning = false;

    void Start()
    {
        dayTextCanvasGroup.alpha = 0;

        if (sunLight != null)
        {
            RenderSettings.sun = sunLight;
            sunLight.type = LightType.Directional;
        }

        // BURADAN RenderSettings.fog = true; SATIRINI SÝLDÝK
        RenderSettings.fog = false; // Garanti olsun diye kapattýk

        UpdateDayUI();
        UpdateAtmosphere();
        StartCoroutine(FadeInDayText());
    }

    void Update()
    {
        if (!isTransitioning)
        {
            timer += Time.deltaTime;
            UpdateAtmosphere();

            if (timer >= dayDurationInSeconds)
            {
                StartNextDay();
            }
        }
    }

    void UpdateAtmosphere()
    {
        if (sunLight == null) return;

        float timePercent = timer / dayDurationInSeconds;

        // 1. GÜNEÞ ROTASYONU VE RENGÝ
        float sunAngle = Mathf.Lerp(-10f, 190f, timePercent);
        sunLight.transform.localRotation = Quaternion.Euler(sunAngle, 170f, 0);
        sunLight.color = sunColor.Evaluate(timePercent);

        float currentSunIntensity = sunIntensity.Evaluate(timePercent);
        sunLight.intensity = currentSunIntensity;

        // 2. AMBIENT LIGHT (Zifiri karanlýk kilidi)
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;

        // Gece aydýnlýðý için bu deðerleri yüksek tuttum (0.4f)
        RenderSettings.ambientIntensity = Mathf.Clamp(currentSunIntensity, 0.4f, 1.5f);
        RenderSettings.reflectionIntensity = Mathf.Max(currentSunIntensity, 0.4f);

        // 3. SKYBOX
        if (skyboxMaterial != null)
        {
            float skyboxExposure = Mathf.Max(currentSunIntensity, 0.2f);
            skyboxMaterial.SetFloat("_Exposure", skyboxExposure);
        }

        // 4. SÝS AYARLARI SÝLÝNDÝ
        // Artýk sis pengueni kapatmayacak.

        // KRÝTÝK: Ortam aydýnlatmasýný zorla güncelle
        if (Time.frameCount % 10 == 0)
        {
            DynamicGI.UpdateEnvironment();
        }
    }

    public void StartNextDay()
    {
        currentDay++;
        timer = 0;
        UpdateDayUI();
        StopCoroutine("FadeInDayText");
        StartCoroutine(FadeInDayText());
    }

    IEnumerator FadeInDayText()
    {
        dayTextCanvasGroup.alpha = 0;
        while (dayTextCanvasGroup.alpha < 1) { dayTextCanvasGroup.alpha += Time.deltaTime * 2f; yield return null; }
        yield return new WaitForSeconds(textDisplayDuration);
        while (dayTextCanvasGroup.alpha > 0) { dayTextCanvasGroup.alpha -= Time.deltaTime * 1f; yield return null; }
    }

    void UpdateDayUI() { if (dayText != null) dayText.text = "Day " + currentDay; }
}