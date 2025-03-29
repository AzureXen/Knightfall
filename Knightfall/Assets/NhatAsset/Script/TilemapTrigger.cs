using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapTrigger : MonoBehaviour
{
    public Tilemap triggerTilemap; // Tilemap chứa 5 ô xanh
    public GameObject banditPrefab;
    public Transform spawnPoint;
    public AudioClip spawnSound;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            Vector3 worldPos = collision.transform.position;
            Vector3Int cellPos = triggerTilemap.WorldToCell(worldPos);

            if (triggerTilemap.HasTile(cellPos)) // Nếu player đang đứng trên 1 trong 5 tile
            {
                triggered = true;

                // Xóa toàn bộ tile trigger nếu muốn
                triggerTilemap.ClearAllTiles();

                // Spawn Bandit
                if (banditPrefab && spawnPoint)
                    Instantiate(banditPrefab, spawnPoint.position, Quaternion.identity);

                if (spawnSound)
                    AudioSource.PlayClipAtPoint(spawnSound, transform.position);

                Debug.Log("🔥 Boss được triệu hồi!");
            }
        }
    }
}
