using System;

using UnityEngine;

namespace Assets._GAME.Scripts.Car
{
    [Serializable]
    public struct Suspension
    {
        public Rigidbody car;
        public bool grounded;
        public Wheel[] wheels;

        public void ApplySpringForce(float input)
        {
            int grounded = 0;
            foreach (Wheel wheel in wheels)
            {
                float springForce = wheel.GetSpringForce(car, input);
                car.AddForceAtPosition(springForce * wheel.springRoot.up, wheel.springRoot.position);
                grounded += wheel.grounded ? 1 : 0;
            }
            this.grounded = grounded >= 2;
        }
    }
}
