using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    [SerializeField] public int maxHealth = 5;
    public int currentHealth;

    [SerializeField] private float iframeDuration;
    [SerializeField] private int flashesNumber;

    private SpriteRenderer spr;

    private Animator animator;

    public bool isHurt = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    void Awake ()
    {
        spr = GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
    }

    public void takeDamage(int amount)
    {
        currentHealth -= amount;

        StartCoroutine(Invulnerability());

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(10, 11, true);
        isHurt = true;
        for (int i = 0; i < flashesNumber; i++)
        {
            spr.color = new Color(0, 0 , 0, 0.5f);
            yield return new WaitForSeconds(iframeDuration / (flashesNumber * 2));
            spr.color = Color.white;
            yield return new WaitForSeconds(iframeDuration / (flashesNumber * 2));
        }
        isHurt = false;

        Physics2D.IgnoreLayerCollision(10, 11, false);
    }
}
