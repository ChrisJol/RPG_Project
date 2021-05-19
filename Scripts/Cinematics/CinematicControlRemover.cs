using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject player;
        PlayableDirector playableDirector;

        private void Awake()
        {
            playableDirector = GetComponent<PlayableDirector>();
            player = GameObject.FindWithTag("Player");
        }

        private void OnEnable()
        {
            playableDirector.played += DisableControl;
            playableDirector.stopped += EnableControl;   
        }

        private void OnDisable()
        {
            playableDirector.played += DisableControl;
            playableDirector.stopped += EnableControl;
        }

        void DisableControl(PlayableDirector pd)
        {
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControl(PlayableDirector pd)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
