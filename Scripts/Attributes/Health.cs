using GameDevTV.Utils;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> {}

        LazyValue<float> health;
        bool isDead = false;

        private void Awake()
        {
            health = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            health.ForceInit();
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;    
        }

        public bool IsDead(){
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            health.value = Mathf.Max(health.value - damage, 0);
            
            if(health.value <= 0)
            {
                AwardExperience(instigator);
                onDie.Invoke();
                Die();
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        public void Heal(float healthToRestore)
        {
            health.value = Mathf.Min(health.value + healthToRestore, GetMaxHealth());
        }

        public float GetHealth()
        {
            return health.value;
        }

        public float GetMaxHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetFraction()
        {
            return (health.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        private float GetInitialHealth()
        {
            float healthStat = GetComponent<BaseStats>().GetStat(Stat.Health);
            return healthStat;
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if(experience == null) return;

            float experienceReward = GetComponent<BaseStats>().GetStat(Stat.ExperienceReward);
            experience.GainExperience(experienceReward);
        }

        private void Die()
        {
            if(isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void RegenerateHealth()
        {
            health.value = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public object CaptureState()
        {
            return health.value;
        }

        public void RestoreState(object state)
        {
            health.value = (float)state;
            if(health.value <= 0) Die();
        }
    }
}
