using UnityEngine;
using RPG.Attributes;
using RPG.Control;
using UnityEngine.InputSystem;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!callingController.GetComponent<Fighter>().CanAttack(gameObject))
            {
                return false;
            }

            if (Mouse.current.press.isPressed)
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }

            return true;
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        }
    }
}