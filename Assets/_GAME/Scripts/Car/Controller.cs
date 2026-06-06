using System;

using UnityEngine;

namespace Assets._GAME.Scripts.Car
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private G27QuestController wheelController;

        public Suspension suspension;
        public Engine engine;
        public Steering steering;

        private void FixedUpdate()
        {
            if (wheelController == null || !wheelController.IsConnected) return;
            float steerInput = wheelController.Steering;
            float gasInput = wheelController.Throttle * 2 - 1;
            float brakeInput = wheelController.Brake;
            suspension.ApplySpringForce(gasInput);
            if (!suspension.grounded) return;
            engine.Run(gasInput, brakeInput);
            steering.Apply(steerInput, brakeInput, engine);
        }
    }
}
