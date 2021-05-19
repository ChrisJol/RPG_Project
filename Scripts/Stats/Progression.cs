using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression")]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<characterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, characterClass characterClass, int level)
        {
            BuildLookup();
            
            float[] levels = lookupTable[characterClass][stat];

            if(levels.Length < level) return 1f;

            return levels[level - 1];
        }

        public int GetLevels(Stat stat, characterClass characterClass)
        {
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookup()
        {
            if(lookupTable != null) return;

            lookupTable = new Dictionary<characterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                Dictionary<Stat, float[]> statLookupTable = new Dictionary<Stat, float[]>();

                foreach(ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public characterClass characterClass;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}