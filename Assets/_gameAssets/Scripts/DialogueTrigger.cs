using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public int partIndex;
    public bool triggerOnce = true;
    private bool done = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !done)
        {
            FindObjectOfType<DialogueSystem>().TriggerDialoguePart(partIndex);
            if (triggerOnce) done = true;
        }
    }
}