using UnityEngine;
using TMPro;

public class PenguinTitleAnimation : MonoBehaviour
{
    [Header("Referans")]
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Üşüme (Shiver) Ayarları")]
    [Range(0f, 10f)] public float shiverAmount = 0.8f; // Titreme miktarı
    [Range(0f, 100f)] public float shiverSpeed = 50f;  // Titreme hızı

    [Header("Parlama (Glow) Ayarları")]
    [Range(0f, 1f)] public float pulseSpeed = 2f;    // Parlama hızı
    [Range(0f, 0.5f)] public float glowMin = 0.1f;    // En az parlama
    [Range(0.5f, 1f)] public float glowMax = 0.4f;    // En çok parlama

    private Material textMaterial;

    void Start()
    {
        if (titleText != null)
        {
            // Yazının materyalini alıyoruz (Glow efekti için)
            textMaterial = titleText.fontMaterial;
        }
    }

    void LateUpdate()
    {
        if (titleText == null) return;

        ApplyShiverEffect();
        ApplyGlowPulse();
    }

    // Harf harf üşüme/titreme animasyonu
    void ApplyShiverEffect()
    {
        titleText.ForceMeshUpdate();
        TMP_TextInfo textInfo = titleText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;
            Vector3[] verts = textInfo.meshInfo[matIndex].vertices;

            // Dişleri birbirine vuran bir penguen gibi hızlı titreme
            float offset = Mathf.Sin(Time.time * shiverSpeed + i) * shiverAmount;

            for (int j = 0; j < 4; j++)
            {
                verts[vertIndex + j] += new Vector3(offset, offset, 0);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            titleText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    // Yazının yavaşça parlayıp sönmesi (Büyülü buz hissi)
    void ApplyGlowPulse()
    {
        float glowPower = Mathf.Lerp(glowMin, glowMax, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        textMaterial.SetFloat(ShaderUtilities.ID_GlowPower, glowPower);
    }
}