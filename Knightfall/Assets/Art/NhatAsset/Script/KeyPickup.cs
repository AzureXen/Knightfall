using UnityEngine;
using UnityEngine.Tilemaps;

public class KeyPickup : MonoBehaviour
{
    public string doorTilemapName = "SecretDoorTilemap"; 
    public AudioClip unlockSFX;
    public string unlockMessage = "Đã mở khóa phòng bí mật";

    private Tilemap doorTilemap;

    private void Start()
    {
        // 🔍 Tự tìm Tilemap trong Scene theo tên
        GameObject obj = GameObject.Find(doorTilemapName);
        if (obj != null)
        {
            doorTilemap = obj.GetComponent<Tilemap>();
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Tilemap cửa bí mật trong scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && doorTilemap != null)
        {
            Destroy(doorTilemap.gameObject); // ❌ Xóa cả GameObject chứa Tilemap

            AudioSource.PlayClipAtPoint(unlockSFX, transform.position);
            Debug.Log(unlockMessage);

            Destroy(gameObject); // ❌ Xoá key
        }
    }
}
