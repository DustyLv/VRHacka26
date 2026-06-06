using UnityEngine;
using UnityEngine.AI;
using ithappy.City_Characters.Controller;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterMover))]
public class NPCAIController : MonoBehaviour
{
    public WaypointManager waypointManager;
    public float arrivalRadius = 1.5f;
    public bool enableRunning = false;

    private NavMeshAgent m_Agent;
    private CharacterMover m_Mover;

    private void Awake()
    {
        waypointManager = (WaypointManager)FindAnyObjectByType(typeof(WaypointManager));
        m_Agent = GetComponent<NavMeshAgent>();
        m_Mover = GetComponent<CharacterMover>();
        
        m_Agent.updatePosition = false;
        m_Agent.updateRotation = false;
    }

    private void Start()
    {
        SetNextWaypoint();
    }

    private void Update()
    {
        // Debug.Log($"[NPCAI] PathPending: {m_Agent.pathPending} | HasPath: {m_Agent.hasPath} | RemainingDist: {m_Agent.remainingDistance}");

        if (!m_Agent.pathPending && m_Agent.remainingDistance <= arrivalRadius)
        {
            // Debug.Log("[NPCAI] Arrival radius reached. Acquiring new waypoint.");
            SetNextWaypoint();
        }

        NavigateToTarget();
    }

    private void LateUpdate()
    {
        m_Agent.nextPosition = transform.position;
    }

    private void NavigateToTarget()
    {
        if (m_Agent.pathPending || m_Agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            // Debug.Log($"[NPCAI] Halting. PathPending: {m_Agent.pathPending} | Status: {m_Agent.pathStatus}");
            m_Mover.SetInput(Vector2.zero, transform.position + transform.forward, false, false);
            return;
        }

        Vector3 steeringTarget = m_Agent.steeringTarget;
        Vector3 directionToTarget = steeringTarget - transform.position;
        directionToTarget.y = 0f;

        float sqrDist = directionToTarget.sqrMagnitude;
        Vector2 inputAxis = Vector2.zero;

        if (sqrDist > 0.01f)
        {
            inputAxis = new Vector2(0f, 1f); 
        }

        // Debug.Log($"[NPCAI] InputAxis: {inputAxis} | TargetSqrDist: {sqrDist} | SteeringTarget: {steeringTarget}");

        m_Mover.SetInput(in inputAxis, in steeringTarget, in enableRunning, false);
    }

    private void SetNextWaypoint()
    {
        if (waypointManager == null) 
        {
            // Debug.LogError("[NPCAI] WaypointManager is unassigned.");
            return;
        }
        
        Transform targetWaypoint = waypointManager.GetRandomWaypoint();
        if (targetWaypoint != null)
        {
            // Debug.Log($"[NPCAI] Setting destination: {targetWaypoint.name} ({targetWaypoint.position})");
            m_Agent.SetDestination(targetWaypoint.position);
        }
    }
}