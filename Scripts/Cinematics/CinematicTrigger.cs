using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        bool triggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if(!triggered && other.tag == "Player")
            {
                triggered = true;
                GetComponent<PlayableDirector>().Play();
                FindObjectOfType<SavingWrapper>().Save();
            }
        }

        public object CaptureState()
        {
            return triggered;
        }

        public void RestoreState(object state)
        {
            triggered = (bool)state;
        }
    }
}
