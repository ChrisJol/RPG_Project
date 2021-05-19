using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] float runSpeedModifier = 1f;
        [SerializeField] float attackDelay = 1.5f;
        [SerializeField] Transform rightHand = null;
        [SerializeField] Transform leftHand = null;
        [SerializeField] WeaponConfig defaultWeapon = null;

        Health target;
        float attackTime = 0f;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        private void Awake()
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private void Start() {
            currentWeapon.ForceInit();
            attackTime = -attackDelay; 
        }

        private void Update()
        {
            if(target == null || target.IsDead()) return;

            if(!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, runSpeedModifier);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehavior();
            }
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            currentWeaponConfig = weaponConfig;
            currentWeapon.value = AttachWeapon(weaponConfig);
        }

        public Health GetTarget()
        {
            return target;
        }

        public Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHand, leftHand, animator);
        }

        private void AttackBehavior()
        {
            transform.LookAt(target.transform);
            if(attackTime + attackDelay <= Time.time)
            {
                TriggerAttack();
                attackTime = Time.time;
            }
        }

        private void TriggerAttack()
        {
            //This will triger the Hit() event
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        //Animation Event
        void Hit()
        {
            if(target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if(currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            if(currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHand, leftHand, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= currentWeaponConfig.GetAttackRange();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if(combatTarget == null) return false;

            Health healthTarget = combatTarget.GetComponent<Health>();

            return healthTarget != null && !healthTarget.IsDead();
        }

        public void Cancel()
        {
            target = null;
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
            GetComponent<Mover>().Cancel();
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentBonus();
            }
        }

        public void RestoreState(object state)
        {
            string weaponName = (string) state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
