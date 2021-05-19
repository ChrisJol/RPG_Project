using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 10f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifetime = 10f;
        [SerializeField] float lifeAfterImpact = 1f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] UnityEvent onHit;

        Health target = null;
        GameObject instigator = null;
        float damage = 0f;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if(target == null) return;
            if(isHoming && !target.IsDead()) 
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health enemyTarget, GameObject instigator, float damage)
        {
            this.target = enemyTarget;
            this.instigator = instigator;
            this.damage = damage;

            Destroy(gameObject, maxLifetime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if(targetCapsule == null) 
            {
                return target.transform.position;
            }

            return target.transform.position + Vector3.up * targetCapsule.height / 1.5f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<Health>() != target) return;
            if(target.IsDead()) return;

            speed = 0f;
            onHit.Invoke();
            
            if(hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            target.TakeDamage(instigator, damage);
            foreach(GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
