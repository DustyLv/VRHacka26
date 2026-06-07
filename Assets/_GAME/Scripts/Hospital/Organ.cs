using System;

using UnityEngine;

namespace Assets._GAME.Scripts.Hospital
{
    public enum OrganType
    {
        Brains = 0,
        Heart = 1,
        Intestines = 2,
        Lungs = 3,
    }

    public class Organ : MonoBehaviour
    {
        public OrganType type;
    }
}
