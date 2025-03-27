using System.Collections;
using UnityEngine;
using static ParticleManager;
using static UnityEngine.GraphicsBuffer;

public class ZombieManager : MonoBehaviour
{
    private int ZombieHP = 10;
    private Animator animator;
    private float attackRange = 4.0f;


    public ParticleSystem DamageParticleSystem;
    public Transform target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Die();
        Attack();
    }

    private void Attack()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= 4.0f) {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * 2.0f * Time.deltaTime;
            transform.LookAt(target.position);
            animator.SetTrigger("IsAttack");
        }
        
        if (distance > attackRange)
        {
            animator.SetTrigger("IsIdle");
        }
    }

    void Die()
    {
        if (ZombieHP <= 0)
        {
            //animator.SetTrigger("IsDie");
            Destroy(this.gameObject, 3.0f);
            ParticleManager.Instance.ParticlePlay(ParticleType.DamageExplosion, transform.position, Vector3.one);
            ParticleSystem particle = Instantiate(DamageParticleSystem, transform.position, Quaternion.identity);
            DamageParticleSystem.Play();
        }
    }

    public void TakeDamage(int damage)
    {
        ZombieHP -= damage;
        Debug.Log("Killed!");
    }
}
