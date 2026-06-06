using System;

using UnityEngine;

namespace Assets._GAME.Scripts.Hospital
{
    public enum OrganType
    {
        Brains,
        Heart,
        Intestines,
        Lungs,
    }

    public class Organ : MonoBehaviour
    {
        public OrganType type;
    }
}
