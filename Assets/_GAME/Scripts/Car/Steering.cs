using System;

using UnityEngine;

namespace Assets._GAME.Scripts.Car
{
    [Serializable]
    public struct Steering
    {
        public Rigidbody car;
        public float strength;
        public AnimationCurve turning;
        public float dragCoefficient;
        public float brakeDragCoefficient;
        public Transform[] tires;
        public float maxAngle;
        public TrailRenderer[] skidMarks;
        public ParticleSystem[] skidSmokes;
        public float minSideVelocityForSkid;
        public AudioSource audioSkid;
        public AudioSource audioEngine;
        [Range(0, 1)] public float minAudioEnginePitch;
        [Range(1, 5)] public float maxAudioEnginePitch;

        private readonly void ToggleSkidMarks(bool on)
        {
            foreach (TrailRenderer mark in skidMarks) mark.emitting = on;
        }

        private readonly void ToggleSkidSmokes(bool on)
        {
            foreach (ParticleSystem smoke in skidSmokes)
            {
                if (on) smoke.Play();
                else smoke.Stop();
            }
        }

        private readonly void ToggleSkidSound(bool on) => audioSkid.mute = !on;

        public void Apply(float input, Engine engine)
        {
            // only visual
            float angle = maxAngle * input;
            foreach (Transform tire in tires)
            {
                Vector3 angles = tire.localEulerAngles;
                tire.localEulerAngles = new Vector3(angles.x, angle, angles.z);
            }
            // torque
            float ratio = engine.velocityRatio;
            car.AddTorque(strength * input * turning.Evaluate(MathF.Abs(ratio)) * MathF.Sign(ratio) * car.transform.up, ForceMode.Acceleration);
            // drag
            float sideSpeed = engine.velocity.x;
            float drag = -sideSpeed * (Input.GetKey(KeyCode.Space) ? brakeDragCoefficient : dragCoefficient);
            Vector3 dragForce = car.transform.right * drag;
            car.AddForceAtPosition(dragForce, car.worldCenterOfMass, ForceMode.Acceleration);
            // only visual
            bool skid = MathF.Abs(sideSpeed) >= minSideVelocityForSkid && ratio > 0;
            ToggleSkidMarks(skid);
            ToggleSkidSmokes(skid);
            // only sound
            audioEngine.pitch = Mathf.Lerp(minAudioEnginePitch, maxAudioEnginePitch, Mathf.Abs(ratio));
            ToggleSkidSound(skid);
        }
    }
}
