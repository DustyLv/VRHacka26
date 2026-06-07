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
        private Dictionary<int, List<int>> connections;
        private int previousPoint;
        private int targetPoint;

        public List<Transform> points;
        public List<Pair> lines;
        public float closeEnought = 5;
        public float acceleration = 10;
        public Rigidbody body;

        private void BuildConnections()
        {
            connections = new();
            foreach (Pair line in lines)
            {
                Debug.DrawLine(points[line.Item1 - 1].position, points[line.Item2 - 1].position, Color.red, float.MaxValue);
                if (connections.TryGetValue(line.Item1, out List<int> value1)) value1.Add(line.Item2);
                else connections[line.Item1] = new() { line.Item2 };
                if (connections.TryGetValue(line.Item2, out List<int> value2)) value2.Add(line.Item1);
                else connections[line.Item2] = new() { line.Item1 };
            }
            // foreach (KeyValuePair<int, List<int>> kvp in connections) Debug.Log($"{kvp.Key} => {string.Join(", ", kvp.Value)}");
        }

        private void ClosestPoint()
        {
            float closestDistance = float.MaxValue;
            Transform closestPoint = null;
            foreach (Transform point in points)
            {
                float distance = Vector3.Distance(point.transform.position, transform.position);
                if (distance > closestDistance) continue;
                closestDistance = distance;
                closestPoint = point;
            }
            targetPoint = points.IndexOf(closestPoint) + 1;
            Debug.Log($"{targetPoint} {closestPoint} {closestDistance}");
            Debug.DrawLine(transform.position, closestPoint.position, Color.green, float.MaxValue);
        }

        private void ConnectedPoint()
        {
            if (!connections.TryGetValue(targetPoint, out List<int> possibilities)) return;
            if (previousPoint > 0)
            {
                possibilities = new(possibilities);
                possibilities.Remove(previousPoint);
            }
            previousPoint = targetPoint;
            targetPoint = possibilities[UnityEngine.Random.Range(0, possibilities.Count)];
            Debug.DrawLine(transform.position, points[previousPoint - 1].position, Color.blue, float.MaxValue);
            Debug.DrawLine(transform.position, points[targetPoint - 1].position, Color.green, float.MaxValue);
        }

        private void NextTarget()
        {
            if (points.Count <= 0) return;
            if (targetPoint <= 0) ClosestPoint();
            else ConnectedPoint();
        }

        private void Start()
        {
            BuildConnections();
            NextTarget();
        }

        private bool TargetReached()
        {
            if (points.Count <= 0 || targetPoint <= 0) return false;
            Transform target = points[targetPoint - 1];
            return Vector3.Distance(target.transform.position, transform.position) <= closeEnought;
        }

        private void FixedUpdate()
        {
            if (points.Count <= 0 || targetPoint <= 0) return;
            if (TargetReached()) NextTarget();
            Transform target = points[targetPoint - 1];
            Vector3 force = target.position - transform.position;
            force.Normalize();
            body.AddForceAtPosition(force * acceleration, target.position, ForceMode.Impulse);
        }
    }
}
