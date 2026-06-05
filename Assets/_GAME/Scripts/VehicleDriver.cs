using TMPro;
using UnityEngine;

public class VehicleDriver : MonoBehaviour
{
    public TextMeshProUGUI UIText;
    
    [SerializeField] private G27QuestController wheelController;

    void Update()
    {
        if (wheelController == null || !wheelController.IsConnected) return;

        // Fetch values directly from the API
        float steerInput = wheelController.Steering;
        float gasInput = wheelController.Throttle;
        float brakeInput = wheelController.Brake;

        // Apply logic to your game mechanics
        ApplyVehicleForces(steerInput, gasInput, brakeInput);

        // Check button inputs cleanly via Enum keys
        if (wheelController.IsButtonPressed(G27QuestController.G27Button.RightPaddle))
        {
            ShiftUp();
        }
    }

    private void ApplyVehicleForces(float s, float g, float b)
    {
        string t = $"Steer: {s}. <br> Gas: {g}. <br> Brake: {b}.";
        UIText.text = t;
        Debug.Log($"Steer: {s}");
        Debug.Log($"Gas: {g}");
        Debug.Log($"Brake: {b}");
    }
    private void ShiftUp() { }
}