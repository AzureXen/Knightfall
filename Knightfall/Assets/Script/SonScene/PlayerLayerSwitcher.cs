using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerLayerSwitcher : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayers; // Assign Ground and Floor1 layers in Inspector
    private SpriteRenderer spriteRenderer;
    private int defaultLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultLayer = gameObject.layer;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Tilemap>(out Tilemap tilemap))
        {
            // Change player's layer to match the Tilemap layer
            gameObject.layer = collision.gameObject.layer;

            // Optional: Update sorting layer for visual correctness
            spriteRenderer.sortingLayerID = tilemap.GetComponent<TilemapRenderer>().sortingLayerID;
            spriteRenderer.sortingOrder = tilemap.GetComponent<TilemapRenderer>().sortingOrder + 1; // Ensure player stays on top

            Debug.Log("Player changed to layer: " + LayerMask.LayerToName(gameObject.layer));
        }
    }

    //void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.TryGetComponent<Tilemap>(out Tilemap tilemap))
    //    {
    //        // Reset player layer to default when leaving the tilemap
    //        gameObject.layer = defaultLayer;
    //    }
    //}
}
