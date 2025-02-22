using UnityEngine;

public class HitBoxVisualAnimator : MonoBehaviour
{
    Animator am;
    [SerializeField] private PlayerSword ps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        am = GetComponent<Animator>();
        if(ps == null) ps = FindFirstObjectByType<PlayerSword>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ps == null) { Debug.Log("Object not detected"); return; }

        if (ps.isAttacking)
        {
            am.SetBool("Swing", true);
        }
        else
        {
            am.SetBool("Swing", false);
        }
    }
}
