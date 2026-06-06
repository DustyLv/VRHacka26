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
            
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");
            
            float steerInput = wheelController.Steering;
            float gasInput = wheelController.Throttle;
            float brakeInput = wheelController.Brake;
            
            
            suspension.ApplySpringForce(vertical);
            if (!suspension.grounded) return;
            engine.Run(vertical);
            steering.Apply(horizontal, engine);
        }
    }
}
