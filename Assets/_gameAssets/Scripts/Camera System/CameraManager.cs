using UnityEngine;
using Unity.Cinemachine; // Bunu eklemeyi unutma!

public class CameraManager : MonoBehaviour
{
    [Header("FreeLook Kameralar")]
    public CinemachineCamera[] freeLookCams; // FreeLook kameralarýný buraya sürükle

    private int currentIndex = 0;
    private int priorityHigh = 20;
    private int priorityLow = 10;

    void Start()
    {
        // Baþlangýçta tüm kameralarý düþük önceliðe al, sadece ilkini yükselt
        SetAllPrioritiesLow();
        if (freeLookCams.Length > 0)
        {
            freeLookCams[0].Priority = priorityHigh;
        }
    }

    void Update()
    {
        // C tuþuna basýnca deðiþtir
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchFreeLookCamera();
        }
    }

    void SwitchFreeLookCamera()
    {
        // Mevcut kameranýn önceliðini düþür
        freeLookCams[currentIndex].Priority = priorityLow;

        // Bir sonraki kameraya geç
        currentIndex++;
        if (currentIndex >= freeLookCams.Length)
        {
            currentIndex = 0;
        }

        // Yeni kameranýn önceliðini yükselt
        freeLookCams[currentIndex].Priority = priorityHigh;

        Debug.Log("Kamera Deðiþti: " + freeLookCams[currentIndex].name);
    }

    void SetAllPrioritiesLow()
    {
        foreach (var cam in freeLookCams)
        {
            cam.Priority = priorityLow;
        }
    }
}
