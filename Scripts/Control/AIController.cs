using UnityEngine;
using RPG.Combat;
using RPG.Attributes;
using RPG.Movement;
using RPG.Core;
using GameDevTV.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float agroRange = 5f;
        [SerializeField] float suspicionDuration = 2f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = .5f;
        [SerializeField] float wayPointDwellTime = 3f;
        [Range(0,1)] [SerializeField] float patrolSpeedModifier = 0.2f;

        GameObject player;
        Fighter fighter;
        Health health;
        Mover mover;

        LazyValue<Vector3> guardLocation;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindGameObjectWithTag("Player");
            guardLocation = new LazyValue<Vector3>(GetGuardLocation);
        }

        private void Start()
        {
            guardLocation.ForceInit();    
        }

        private void Update()
        {
            if (health.IsDead()) return;

            if (InAttackRange() && fighter.CanAttack(player))
            {
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer < suspicionDuration)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }

            UpdateTimers();
        }

        private Vector3 GetGuardLocation()
        {
            return transform.position;
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = guardLocation.value;

            if(patrolPath != null)
            {
                if(AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0f;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }

            if(timeSinceArrivedAtWaypoint > wayPointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedModifier);
            }
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());

            return distanceToWaypoint <= waypointTolerance;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
        }

        private bool InAttackRange()
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            return distance <= agroRange;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, agroRange);    
        }
    }
}
