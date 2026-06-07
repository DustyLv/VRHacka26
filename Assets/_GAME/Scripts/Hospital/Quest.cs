using System;

using UnityEngine;

namespace Assets._GAME.Scripts.Hospital
{
    [Serializable]
    public struct Quest
    {
        public OrganType type;
        public int got;
        public int needed;

        public Quest(OrganType type, int needed)
        {
            got = 0;
            this.type = type;
            this.needed = needed;
        }

        public bool Collect(OrganType type)
        {
            if (type != this.type) return false;
            got++;
            Debug.Log($"{got}/{needed} {type}");
            Controller.instance.questText.text = $"{got}/{needed} {type}";
            return true;
        }

        public readonly bool IsDone() => got >= needed;
    }
}
