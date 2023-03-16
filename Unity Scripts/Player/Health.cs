using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    public float currentHealth {get; private set;}
    private Animator anim;
    private bool dead;
    [SerializeField] private AudioSource DeathSoundEffect;
    [SerializeField] private AudioSource HurtSoundEffect;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
    }
    public void TakeDamage(float _damage)
    {

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            HurtSoundEffect.Play();
            anim.SetTrigger("Hurt");
        }
        else
        {
            if (!dead)
            {
                DeathSoundEffect.Play();
                anim.SetTrigger("Death");
                GetComponent<PlayerMovement>().enabled = false;
                dead = true;
            }
        }

    }

    public void AddHealth(float _value){
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    public void Respawn(){
        dead = false;
        AddHealth(startingHealth);
        anim.ResetTrigger("Death");
        anim.Play("PlayerIdle");

        GetComponent<PlayerMovement>().enabled = true;
    }

    public void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Fallpoint")){
                DeathSoundEffect.Play();
                anim.SetTrigger("Death");
                GetComponent<PlayerMovement>().enabled = false;
                dead = true;
        }
    }
}
