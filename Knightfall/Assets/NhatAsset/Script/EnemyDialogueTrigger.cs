using UnityEngine;

public class EnemyDialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Setup")]
    public GameObject dialogueBoxPrefab;
    public string[] dialogueLinesOnHit;
    public string[] dialogueLinesOnDeath;
    public float dialogueDuration = 3f;

    private GameObject currentDialogueBox;

    public void TriggerHitDialogue()
    {
        ShowDialogue(dialogueLinesOnHit);
    }

    public void TriggerDeathDialogue()
    {
        ShowDialogue(dialogueLinesOnDeath);
    }

    private void ShowDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        // Xoá hộp thoại cũ nếu có
        if (currentDialogueBox != null)
        {
            Destroy(currentDialogueBox);
        }

        // Tạo mới
        Vector3 spawnPos = transform.position + new Vector3(0, 1.5f, 0);
        currentDialogueBox = Instantiate(dialogueBoxPrefab, spawnPos, Quaternion.identity);

        // Gắn vào Canvas
        currentDialogueBox.transform.SetParent(GameObject.Find("Canvas").transform, false);

        // Gắn theo enemy
        FollowTarget follow = currentDialogueBox.GetComponent<FollowTarget>();
        if (follow == null)
        {
            follow = currentDialogueBox.AddComponent<FollowTarget>();
        }
        follow.target = this.transform;

        // Gán thoại
        Dialogue dialogue = currentDialogueBox.GetComponent<Dialogue>();
        if (dialogue != null)
        {
            dialogue.lines = lines;
            dialogue.gameObject.SetActive(true);
        }

        // Tự xoá sau X giây (nếu không muốn thì bạn có thể bỏ dòng này)
        Destroy(currentDialogueBox, dialogueDuration);
    }
}
