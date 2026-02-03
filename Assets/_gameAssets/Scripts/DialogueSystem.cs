using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class DialoguePart
{
    public string partName;
    [TextArea(3, 10)] public string[] dialogueLines;
}

public class DialogueSystem : MonoBehaviour
{
    public List<DialoguePart> dialogueParts;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private GameObject subtitlePanel;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float autoHideDelay = 5.0f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> penguinSounds;

    private Queue<string> sentenceQueue = new Queue<string>();
    private Coroutine activeRoutine;
    private bool isTextAnimating = false;

    void Start() { subtitlePanel.SetActive(false); }

    public void TriggerDialoguePart(int index)
    {
        if (index >= dialogueParts.Count) return;
        sentenceQueue.Clear();
        foreach (string line in dialogueParts[index].dialogueLines) sentenceQueue.Enqueue(line);
        subtitlePanel.SetActive(true);
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentenceQueue.Count == 0) { subtitlePanel.SetActive(false); return; }
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(TypeSentence(sentenceQueue.Dequeue()));
    }

    IEnumerator TypeSentence(string text)
    {
        isTextAnimating = true;
        subtitleText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            subtitleText.text += letter;
            if (audioSource && penguinSounds.Count > 0 && letter != ' ')
                audioSource.PlayOneShot(penguinSounds[Random.Range(0, penguinSounds.Count)]);
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(autoHideDelay);
        DisplayNextSentence();
    }
}