using System;

using UnityEngine;

namespace Assets._GAME.Scripts.Car
{
    public class Controller : MonoBehaviour
    {
        public Suspension suspension;
        public Engine engine;
        public Steering steering;

        private void FixedUpdate()
        {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");
            suspension.ApplySpringForce(vertical);
            if (!suspension.grounded) return;
            engine.Run(vertical);
            steering.Apply(horizontal, engine);
        }
    }
}
