using UnityEngine;

public class Fish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Görev yöneticisini bul ve görevi bitir
            FindObjectOfType<MissionManager>().CompleteMission();

            // Bu objeyi yok et veya etkisiz hale getir
            gameObject.SetActive(false);
        }
    }
}
