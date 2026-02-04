using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yüklemek için
using System.Collections;

public class MenuController : MonoBehaviour
{
    [Header("Sahne Ayarı")]
    // Buraya kod içinde sahne adını yazıyoruz
    [SerializeField] private string targetSceneName = "Game"; 

    [Header("Geçiş Efekti (Opsiyonel)")]
    [SerializeField] private CanvasGroup fadePanel; 
    [SerializeField] private float fadeDuration = 1.0f;

    // Start butonuna bu fonksiyonu bağlayacaksın
    public void StartGame()
    {
        // Kod direkt buradaki "targetSceneName" değişkenini kullanır
        StartCoroutine(TransitionRoutine());
    }

    IEnumerator TransitionRoutine()
    {
        // 1. Varsa kararma/beyazlama efektini oynat
        if (fadePanel != null)
        {
            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadePanel.alpha = timer / fadeDuration;
                yield return null;
            }
        }

        // 2. Kodun en başında tanımladığımız sahneyi yükle
        SceneManager.LoadScene(targetSceneName);
    }

    // Çıkış butonu için (Bunu da aynı scriptten yönetebilirsin)
    public void QuitGame()
    {
        Debug.Log("Oyundan çıkılıyor...");
        Application.Quit();
    }
}