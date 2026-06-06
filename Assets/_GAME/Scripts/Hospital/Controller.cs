using System;

using UnityEngine;

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
        }

        public bool Collect(OrganType type)
        {
            if (!current.Collect(type)) return false;
            if (!current.IsDone()) return false;
            questsDone++;
            GenerateQuest();
            return true;
        }
    }
}
