using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets._GAME.Scripts.Hospital
{
    public class Controller : MonoBehaviour
    {
        public Quest current;
        public int questsDone;
        public int neededBaseMin;
        public int neededBaseMax;
        public int neededScaleMin;
        public int neededScaleMax;
        public OrganType[] types = { OrganType.Brains, OrganType.Heart, OrganType.Intestines, OrganType.Lungs };
        public AudioSource car;
        public AudioClip[] audios;
        public TextMeshProUGUI questText;
        
        public static Controller instance;
        
        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            GenerateQuest();
        }

        public void GenerateQuest()
        {
            OrganType type = types[UnityEngine.Random.Range(0, types.Length)];
            int needed = UnityEngine.Random.Range(neededBaseMin, neededBaseMax);
            needed += UnityEngine.Random.Range(questsDone * neededScaleMin, questsDone * neededScaleMax);
            current = new Quest(type, needed);
            Debug.Log($"{needed} {type}");
            car.clip = audios[UnityEngine.Random.Range(0, audios.Length)];
            car.Play();
            questText.text = $"New task! <br> {needed} {type}";
        }

        public bool Collect(OrganType type)
        {
            if (!current.Collect(type)) return false;
            if (!current.IsDone()) return false;
            questsDone++;
            if (audios.Length > 0)
            {
                car.generator = audios[UnityEngine.Random.Range(0, audios.Length)];
                car.Play();
            }
            GenerateQuest();
            return true;
        }
    }
}
