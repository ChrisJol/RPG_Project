using System;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            Text UIText = GetComponent<Text>();

            if(fighter.GetTarget() == null)
            {
                UIText.text = "Enemy Health: N/A";
            }
            else
            {
                Health health = fighter.GetTarget();
                GetComponent<Text>().text = String.Format("Enemy Health: {0:0}/{1:0}", health.GetHealth(), health.GetMaxHealth());
            }
        }
    }
}
