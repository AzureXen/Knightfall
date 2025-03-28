using UnityEngine;

public class LetterPickup : MonoBehaviour
{
    public LetterCollectorCustom letterManager; 

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something touched the letter: " + other.gameObject.name); // Debugging
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collected the letter!"); // Debugging
            letterManager.CollectLetter(); 
            Destroy(gameObject); 
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
