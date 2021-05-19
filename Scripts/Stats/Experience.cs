using RPG.Saving;
using UnityEngine;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0f;

        //public delegate void ExperienceGainedDelegate();
        public event Action onExperiencedGained;

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperiencedGained();
        }

        public float GetExperience()
        {
            return experiencePoints;
        }

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float) state;
        }
    }
}
