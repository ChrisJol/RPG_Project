using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] float fadeInTime = 1f;

        InputMaster inputActions;
        const string defaultSaveFile = "save";

        private void Awake() 
        {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
        }

#if UNITY_EDITOR
        private void OnEnable() {
            inputActions = new InputMaster();
            inputActions.Enable();
            inputActions.Player.Save.performed += _ => Save();
            inputActions.Player.Load.performed += _ => Load();
            inputActions.Player.Delete.performed += _ => Delete();
        }
        
        private void OnDisable()
        {
            inputActions.Disable();    
        }
#endif

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}
