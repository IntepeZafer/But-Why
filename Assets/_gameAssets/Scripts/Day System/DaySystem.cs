using UnityEngine;
using TMPro;
using System.Collections;

public class DaySystem : MonoBehaviour
{
    [Header("UI Elemanlarý")]
    public TextMeshProUGUI dayText;
    public CanvasGroup dayTextCanvasGroup;

    [Header("Zaman Ayarlarý")]
    public float dayDurationInSeconds = 600f; // 10 Dakika - Gerçekçi hýz
    public float textDisplayDuration = 5f;

    [Header("Iþýk ve Atmosfer")]
    public Light sunLight;
    public Gradient sunColor;
    public AnimationCurve sunIntensity; // Gündüz yüksek (1.5), Gece sýfýr (0) olmalý!
    [Tooltip("Dengeli sis için 0.001 (gündüz) ile 0.03 (gece) arasý deðerler kullanýn.")]
    public AnimationCurve fogDensityCurve;
    public Material skyboxMaterial;

    private float timer = 0f;
    private int currentDay = 1;
    private bool isTransitioning = false;

    void Start()
    {
        dayTextCanvasGroup.alpha = 0;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;

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
            if (timer >= dayDurationInSeconds) StartNextDay();
        }
    }

    void UpdateAtmosphere()
    {
        if (sunLight == null) return;
        float timePercent = timer / dayDurationInSeconds;

        // 1. GÜNEÞ ROTASYONU
        float sunAngle = Mathf.Lerp(-10f, 190f, timePercent);
        sunLight.transform.localRotation = Quaternion.Euler(sunAngle, 170f, 0);
        sunLight.color = sunColor.Evaluate(timePercent);

        // GÜNDÜZ PARLAKLIÐI VE GECE KARANLIÐI BURADAN GELÝYOR
        float currentSunIntensity = sunIntensity.Evaluate(timePercent);
        sunLight.intensity = currentSunIntensity;

        // Ortam Aydýnlatmasý ve Yansýmalarý Güneþle Baðla (ZÝFÝRÝ KARANLIK ÝÇÝN KRÝTÝK)
        RenderSettings.ambientIntensity = currentSunIntensity;
        RenderSettings.reflectionIntensity = currentSunIntensity;

        // 2. SKYBOX (Gece tamamen karanlýk olmasýný saðlar)
        if (skyboxMaterial != null)
        {
            float skyboxExposure = Mathf.Max(currentSunIntensity, 0.01f); // Min 0.01f, tam siyah olmasýn
            skyboxMaterial.SetFloat("_Exposure", skyboxExposure);
        }

        // 3. SÝS AYARLARI (Gündüz az, Gece çok)
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, sunLight.color * 0.2f, Time.deltaTime);
        RenderSettings.fogDensity = Mathf.Clamp(fogDensityCurve.Evaluate(timePercent), 0.001f, 0.03f); // Min 0.001f, Max 0.03f

        // Ortamdaki ýþýklandýrma güncellemelerinin hemen yansýmasý için (Çok önemli!)
        DynamicGI.UpdateEnvironment();

        // 4. KAR VE BUZ TETÝKLEME (GlobalSnowFrostManager'a gece bonusu gönder)
        var gs = FindObjectOfType<GlobalSnowFrostManager>();
        if (gs != null)
        {
            float nightBonusFactor = timePercent > 0.7f ? (timePercent - 0.7f) / 0.3f : 0;
            gs.ApplyNightBonus(nightBonusFactor);
        }
    }

    public void StartNextDay()
    {
        if (isTransitioning) return;

        currentDay++;
        timer = 0;
        UpdateDayUI();
        UpdateAtmosphere(); // Yeni günün atmosferini ayarla

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

    void UpdateDayUI() { dayText.text = "Day " + currentDay; }
}