using UnityEngine;

public class Ship : MonoBehaviour {
    [SerializeField]
    private int crew = 0;
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private Planet source;
    [SerializeField]
    private Planet target;

    private Rigidbody rb;
    private bool isAttacking = false;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        if (!isAttacking) return;
        
        Vector3 heading = target.transform.position - source.transform.position;
        rb.velocity = heading * speed;
        isAttacking = false;
    }

    public void Attack() {
        if (source && target && crew > 0) {
            isAttacking = true;
        }
    }
}
