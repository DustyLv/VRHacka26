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
        public bool debug;

        private void FixedUpdate()
        {
            if (!debug && (wheelController == null || !wheelController.IsConnected)) return;
            float steerInput;
            float gasInput;
            float brakeInput;
            if (debug)
            {
                steerInput = Input.GetAxis("Horizontal");
                gasInput = Input.GetAxis("Vertical");
                brakeInput = Input.GetKey(KeyCode.Space) ? 1 : 0;
            }
            else
            {
                steerInput = wheelController.Steering;
                gasInput = wheelController.Throttle > 0.001 ? wheelController.Throttle : -wheelController.Brake;
                brakeInput = wheelController.Brake;
            }
            suspension.ApplySpringForce(gasInput);
            if (!suspension.grounded) return;
            engine.Run(gasInput, brakeInput);
            steering.Apply(steerInput, brakeInput, engine);
        }
    }
}
