using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health health = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;

        void Update()
        {
            float healthFraction = health.GetFraction();
            
            if(Mathf.Approximately(healthFraction, 0)
            || Mathf.Approximately(healthFraction, 1))
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            foreground.localScale = new Vector3(healthFraction, 1f, 1f);
        }
    }
}
