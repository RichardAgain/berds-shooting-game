using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BullerScript : MonoBehaviour
{

    [Range (1, 10)]
    [SerializeField] private float speed = 10f;

    [Range (1, 10)]
    [SerializeField] private float lifeSpan = 5f;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeSpan);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = transform.up * speed;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<Health>().takeDamage(1);
            Destroy(gameObject);
        }
    }

}
