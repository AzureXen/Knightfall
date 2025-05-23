using UnityEngine;

public class PlayerBow : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    public GameObject bullet;
    public Transform bow;
    public float cooldownTimer;
    public float cooldown = 1f;

    public GameObject player;
    private PlayerSFX playerSFX;
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        playerSFX = player.GetComponent<PlayerSFX>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 playerToMouse = mousePos - transform.position;

        float rotateZ = Mathf.Atan2(playerToMouse.y, playerToMouse.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0,0,rotateZ);

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Clamp(cooldownTimer, 0 , cooldown);
        }

        if (Input.GetMouseButton(0) && cooldownTimer == 0) Fire();
    }
    void Fire()
    {
        playerSFX.playBowShoot();
        cooldownTimer = cooldown;
        GameObject instance = Instantiate(bullet, bow.position, Quaternion.identity);
        instance.GetComponent<PlayerBullet>().playerSFX = playerSFX;
    }
}
