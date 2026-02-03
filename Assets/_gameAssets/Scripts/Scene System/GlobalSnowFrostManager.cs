using UnityEngine;
using GlobalSnowEffect;

public class GlobalSnowFrostManager : MonoBehaviour
{
    private GlobalSnow snow;

    [Header("Kar Þiddeti Dinamiði")]
    public float baseSnowIntensity = 0.1f; // Gündüz kar miktarý (0.05-0.4 arasý deðiþir)
    public float nightSnowMultiplier = 0.6f; // Gece karýn ne kadar artacaðý (bonus ile çarpýlýr)
    private float nightBonusApplied = 0f; // DaySystem'den gelen gece bonusu

    [Header("Sýnýrlar")]
    public float maxTotalSnow = 0.9f; // Maksimum kar yaðýþý (çok yoðun fýrtýna)
    public float minTotalSnow = 0.01f; // Minimum kar (neredeyse hiç kar yok)

    [Header("Rastgele Deðiþim")]
    public float changeInterval = 45f; // Hava durumu her X saniyede bir deðiþsin mi?
    private float nextChangeTime;

    void Start()
    {
        snow = GlobalSnow.instance;
        ChangeWeather(); // Baþlangýçta havayý ayarla
    }

    void Update()
    {
        if (snow == null) return;

        if (Time.time >= nextChangeTime) ChangeWeather();

        // Toplam kar þiddeti = Gündüz temel + Gece bonusu (Sýnýrlar içinde)
        float totalSnowIntensity = Mathf.Clamp(baseSnowIntensity + (nightBonusApplied * nightSnowMultiplier), minTotalSnow, maxTotalSnow);

        // Deðerleri yumuþak bir þekilde uygula (Atmosferik geçiþ)
        snow.snowfallIntensity = Mathf.Lerp(snow.snowfallIntensity, totalSnowIntensity, Time.deltaTime * 0.15f);
        snow.cameraFrostIntensity = Mathf.Lerp(snow.cameraFrostIntensity, totalSnowIntensity * 0.6f, Time.deltaTime * 0.15f);
        snow.cameraFrostSpread = Mathf.Lerp(snow.cameraFrostSpread, totalSnowIntensity * 2.5f, Time.deltaTime * 0.15f);
    }

    // Gündüz kar yaðýþýný rastgele belirleyen fonksiyon
    void ChangeWeather()
    {
        float chance = Random.value;
        if (chance < 0.4f) baseSnowIntensity = minTotalSnow; // %40 ihtimalle hava açýk
        else baseSnowIntensity = Random.Range(0.1f, 0.4f);       // %60 ihtimalle hafif/orta kar

        nextChangeTime = Time.time + changeInterval;
        Debug.Log("Hava durumu deðiþti, gündüz kar þiddeti: " + baseSnowIntensity);
    }

    // DaySystem'den gelen gece bonusunu uygular
    public void ApplyNightBonus(float percent)
    {
        nightBonusApplied = percent;
    }
}