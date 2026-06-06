using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets._GAME.Scripts.AI
{
    [Serializable]
    public struct Pair
    {
        public int Item1;
        public int Item2;
    }

    public class Controller : MonoBehaviour
    {
        private Dictionary<int, HashSet<int>> connections;

        public List<Transform> points;
        public List<Pair> lines;

        private void BuildConnections()
        {
            connections = new();
            foreach (Pair line in lines)
            {
                Debug.DrawLine(points[line.Item1 - 1].position, points[line.Item2 - 1].position, Color.red, float.MaxValue);
                if (connections.TryGetValue(line.Item1, out HashSet<int> value1)) value1.Add(line.Item2);
                else connections[line.Item1] = new() { line.Item2 };
                if (connections.TryGetValue(line.Item2, out HashSet<int> value2)) value2.Add(line.Item1);
                else connections[line.Item2] = new() { line.Item1 };
            }
            foreach (KeyValuePair<int, HashSet<int>> kvp in connections) Debug.Log($"{kvp.Key} => {string.Join(", ", kvp.Value)}");
        }

        private void Start()
        {
            BuildConnections();
        }
    }
}
