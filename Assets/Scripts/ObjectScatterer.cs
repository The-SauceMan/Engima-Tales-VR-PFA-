using UnityEngine;
using System.Collections.Generic;

public class ObjectScatterer : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] prefabsToSpawn;     // Array of prefabs to scatter
    public int numberOfObjects = 50;         // How many objects to spawn
    
    [Header("Spawn Area (Flat Plane)")]
    public SpawnShape spawnShape = SpawnShape.Rectangle;
    public float planeWidth = 20f;            // Width of spawn area (X-axis)
    public float planeLength = 20f;           // Length of spawn area (Z-axis)
    public float spawnRadius = 20f;           // Radius for circular spawn (when using Circle shape)
    public float minHeight = 0f;              // Minimum Y position
    public float maxHeight = 5f;              // Maximum Y position
    
    [Header("Distance Constraints")]
    public float minDistanceBetweenObjects = 1.5f;  // Minimum distance between any two objects
    public int maxSpawnAttempts = 1000;      // Max attempts to find valid position
    
    [Header("Rotation Settings")]
    public RotationMode rotationMode = RotationMode.RandomYOnly;
    public Vector3 fixedRotation = Vector3.zero;     // Fixed rotation when mode is set to Fixed
    public Vector3 randomRotationMin = Vector3.zero;  // Minimum random rotation (X, Y, Z)
    public Vector3 randomRotationMax = new Vector3(0, 360, 0); // Maximum random rotation (X, Y, Z)
    public bool alignToSurfaceNormal = false;  // Rotate to match ground surface normal (requires alignToGround = true)
    
    [Header("Optional Settings")]
    public Transform centerPoint;             // Center point of spawn area (uses this object's position if null)
    public bool alignToGround = false;        // Raycast to ground and place on surface
    public LayerMask groundLayer = -1;        // Which layers count as ground for alignment
    public float groundRaycastHeight = 100f;   // Height to raycast from when aligning to ground
    public bool parentToThisObject = true;     // Make spawned objects children of this object
    
    [Header("Debug")]
    public bool showDebugGizmos = true;        // Show spawn area in Scene view
    
    public enum RotationMode
    {
        Fixed,          // Use exact fixedRotation values
        RandomYOnly,    // Random rotation only on Y axis (0-360)
        RandomFull,     // Random rotation on all axes within min/max range
        RandomCustom,   // Random rotation on specified axes using min/max values
        FaceCenter,     // Rotate to face the center point
        FaceAwayCenter  // Rotate away from the center point
    }
    
    public enum SpawnShape
    {
        Rectangle,      // Rectangular flat plane area
        Circle          // Circular area (original behavior)
    }
    
    private List<Vector3> spawnedPositions = new List<Vector3>();
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<Quaternion> spawnedRotations = new List<Quaternion>();
    
    void Start()
    {
        ScatterObjects();
    }
    
    [ContextMenu("Scatter Objects")]
    public void ScatterObjects()
    {
        // Clear existing objects if any
        ClearSpawnedObjects();
        
        // Set up center point
        Vector3 center = centerPoint != null ? centerPoint.position : transform.position;
        
        int successfulSpawns = 0;
        
        for (int i = 0; i < numberOfObjects; i++)
        {
            bool spawned = false;
            
            for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
            {
                // Generate random position based on spawn shape
                Vector3 randomPosition = GetRandomPositionInSpawnArea(center);
                
                // Store surface normal for alignment
                Vector3 surfaceNormal = Vector3.up;
                
                // Set Y position (height)
                float yPos = Random.Range(minHeight, maxHeight);
                
                // If aligning to ground, raycast to find ground Y and normal
                if (alignToGround)
                {
                    RaycastHit hit;
                    Vector3 rayStart = new Vector3(randomPosition.x, groundRaycastHeight, randomPosition.z);
                    
                    if (Physics.Raycast(rayStart, Vector3.down, out hit, groundRaycastHeight * 2, groundLayer))
                    {
                        yPos = hit.point.y;
                        surfaceNormal = hit.normal;
                    }
                }
                
                randomPosition.y = yPos;
                
                // Check distance from other spawned objects
                bool tooClose = false;
                foreach (Vector3 existingPos in spawnedPositions)
                {
                    if (Vector3.Distance(randomPosition, existingPos) < minDistanceBetweenObjects)
                    {
                        tooClose = false;
                        break;
                    }
                }
                
                if (!tooClose)
                {
                    // Calculate rotation for this object
                    Quaternion rotation = CalculateRotation(randomPosition, center, surfaceNormal);
                    
                    // Apply surface alignment if enabled
                    if (alignToSurfaceNormal && alignToGround)
                    {
                        rotation = AlignToSurface(rotation, surfaceNormal);
                    }
                    
                    // Spawn the object
                    GameObject prefabToSpawn = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];
                    GameObject newObject = Instantiate(prefabToSpawn, randomPosition, rotation);
                    
                    // Parent to this object if enabled
                    if (parentToThisObject)
                    {
                        newObject.transform.parent = transform;
                    }
                    
                    // Store position and object reference
                    spawnedPositions.Add(randomPosition);
                    spawnedObjects.Add(newObject);
                    spawnedRotations.Add(rotation);
                    
                    successfulSpawns++;
                    spawned = true;
                    break;
                }
            }
            
            if (!spawned)
            {
                Debug.LogWarning($"Failed to spawn object {i} after {maxSpawnAttempts} attempts. Try increasing spawn area or reducing min distance.");
            }
        }
        
        Debug.Log($"Successfully spawned {successfulSpawns} out of {numberOfObjects} objects in {(spawnShape == SpawnShape.Rectangle ? "rectangle" : "circle")} area");
    }
    
    private Vector3 GetRandomPositionInSpawnArea(Vector3 center)
    {
        if (spawnShape == SpawnShape.Rectangle)
        {
            // Generate random position within rectangle bounds
            float randomX = Random.Range(-planeWidth / 2f, planeWidth / 2f);
            float randomZ = Random.Range(-planeLength / 2f, planeLength / 2f);
            
            return new Vector3(
                center.x + randomX,
                0,  // Temporary Y value
                center.z + randomZ
            );
        }
        else // Circle shape
        {
            // Generate random position within circle radius
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            return new Vector3(
                center.x + randomCircle.x,
                0,  // Temporary Y value
                center.z + randomCircle.y
            );
        }
    }
    
    private Quaternion CalculateRotation(Vector3 position, Vector3 center, Vector3 surfaceNormal)
    {
        switch (rotationMode)
        {
            case RotationMode.Fixed:
                return Quaternion.Euler(fixedRotation);
                
            case RotationMode.RandomYOnly:
                float randomY = Random.Range(0, 360);
                return Quaternion.Euler(0, randomY, 0);
                
            case RotationMode.RandomFull:
                return Quaternion.Euler(
                    Random.Range(randomRotationMin.x, randomRotationMax.x),
                    Random.Range(randomRotationMin.y, randomRotationMax.y),
                    Random.Range(randomRotationMin.z, randomRotationMax.z)
                );
                
            case RotationMode.RandomCustom:
                Vector3 randomRot = new Vector3();
                randomRot.x = randomRotationMin.x != randomRotationMax.x ? Random.Range(randomRotationMin.x, randomRotationMax.x) : randomRotationMin.x;
                randomRot.y = randomRotationMin.y != randomRotationMax.y ? Random.Range(randomRotationMin.y, randomRotationMax.y) : randomRotationMin.y;
                randomRot.z = randomRotationMin.z != randomRotationMax.z ? Random.Range(randomRotationMin.z, randomRotationMax.z) : randomRotationMin.z;
                return Quaternion.Euler(randomRot);
                
            case RotationMode.FaceCenter:
                Vector3 directionToCenter = (center - position).normalized;
                directionToCenter.y = 0; // Keep upright
                if (directionToCenter != Vector3.zero)
                    return Quaternion.LookRotation(directionToCenter);
                else
                    return Quaternion.identity;
                
            case RotationMode.FaceAwayCenter:
                Vector3 directionAwayFromCenter = (position - center).normalized;
                directionAwayFromCenter.y = 0; // Keep upright
                if (directionAwayFromCenter != Vector3.zero)
                    return Quaternion.LookRotation(directionAwayFromCenter);
                else
                    return Quaternion.identity;
                
            default:
                return Quaternion.identity;
        }
    }
    
    // Method to align rotation to surface normal (use with alignToGround)
    private Quaternion AlignToSurface(Quaternion originalRotation, Vector3 surfaceNormal)
    {
        Quaternion surfaceAlignment = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
        return surfaceAlignment * originalRotation;
    }
    
    [ContextMenu("Clear Spawned Objects")]
    public void ClearSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
                DestroyImmediate(obj);
        }
        spawnedObjects.Clear();
        spawnedPositions.Clear();
        spawnedRotations.Clear();
    }
    
    // Optional: Method to respawn with different settings
    public void RespawnWithSettings(int newCount, float newWidth, float newLength, float newMinDistance)
    {
        numberOfObjects = newCount;
        planeWidth = newWidth;
        planeLength = newLength;
        minDistanceBetweenObjects = newMinDistance;
        ClearSpawnedObjects();
        ScatterObjects();
    }
    
    // Method to update rotations of existing objects (useful for tweaking)
    [ContextMenu("Refresh Rotations")]
    public void RefreshRotations()
    {
        if (spawnedObjects.Count == 0) return;
        
        Vector3 center = centerPoint != null ? centerPoint.position : transform.position;
        
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] != null)
            {
                Vector3 surfaceNormal = Vector3.up;
                
                // Get surface normal if ground alignment is on
                if (alignToGround)
                {
                    RaycastHit hit;
                    Vector3 rayStart = new Vector3(spawnedPositions[i].x, groundRaycastHeight, spawnedPositions[i].z);
                    if (Physics.Raycast(rayStart, Vector3.down, out hit, groundRaycastHeight * 2, groundLayer))
                    {
                        surfaceNormal = hit.normal;
                    }
                }
                
                Quaternion newRotation = CalculateRotation(spawnedPositions[i], center, surfaceNormal);
                
                // Apply surface alignment if enabled
                if (alignToSurfaceNormal && alignToGround)
                {
                    newRotation = AlignToSurface(newRotation, surfaceNormal);
                }
                
                spawnedObjects[i].transform.rotation = newRotation;
                spawnedRotations[i] = newRotation;
            }
        }
        
        Debug.Log($"Refreshed rotations for {spawnedObjects.Count} objects");
    }
    
    // Draw gizmos to visualize spawn area
    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        
        Vector3 center = centerPoint != null ? centerPoint.position : transform.position;
        
        // Draw spawn area based on selected shape
        if (spawnShape == SpawnShape.Rectangle)
        {
            // Draw rectangle area
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Vector3 rectCenter = new Vector3(center.x, center.y + ((minHeight + maxHeight) / 2f), center.z);
            Vector3 rectSize = new Vector3(planeWidth, maxHeight - minHeight, planeLength);
            Gizmos.DrawWireCube(rectCenter, rectSize);
            
            // Draw filled rectangle on ground plane
            Gizmos.color = new Color(0, 1, 0, 0.1f);
            Vector3 groundRectCenter = new Vector3(center.x, center.y + minHeight, center.z);
            Vector3 groundRectSize = new Vector3(planeWidth, 0.1f, planeLength);
            Gizmos.DrawCube(groundRectCenter, groundRectSize);
        }
        else // Circle shape
        {
            // Draw circle area
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawWireSphere(center, spawnRadius);
            
            // Draw height range for circle
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(center.x - spawnRadius, center.y + minHeight, center.z), 
                           new Vector3(center.x + spawnRadius, center.y + minHeight, center.z));
            Gizmos.DrawLine(new Vector3(center.x - spawnRadius, center.y + maxHeight, center.z), 
                           new Vector3(center.x + spawnRadius, center.y + maxHeight, center.z));
        }
        
        // Draw center point
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(center, 0.5f);
        
        // Draw border lines for rectangle
        if (spawnShape == SpawnShape.Rectangle)
        {
            Gizmos.color = Color.green;
            Vector3 corner1 = new Vector3(center.x - planeWidth/2, center.y + minHeight, center.z - planeLength/2);
            Vector3 corner2 = new Vector3(center.x + planeWidth/2, center.y + minHeight, center.z - planeLength/2);
            Vector3 corner3 = new Vector3(center.x + planeWidth/2, center.y + minHeight, center.z + planeLength/2);
            Vector3 corner4 = new Vector3(center.x - planeWidth/2, center.y + minHeight, center.z + planeLength/2);
            
            Gizmos.DrawLine(corner1, corner2);
            Gizmos.DrawLine(corner2, corner3);
            Gizmos.DrawLine(corner3, corner4);
            Gizmos.DrawLine(corner4, corner1);
        }
        
        // Draw spawned positions with rotation indicators (only in play mode)
        if (Application.isPlaying && showDebugGizmos && spawnedObjects.Count > 0)
        {
            Gizmos.color = Color.blue;
            foreach (Vector3 pos in spawnedPositions)
            {
                Gizmos.DrawWireSphere(pos, minDistanceBetweenObjects / 2);
            }
            
            // Draw forward direction arrows for spawned objects
            Gizmos.color = Color.green;
            for (int i = 0; i < spawnedObjects.Count && i < spawnedRotations.Count; i++)
            {
                if (spawnedObjects[i] != null)
                {
                    Vector3 forward = spawnedObjects[i].transform.forward;
                    Gizmos.DrawRay(spawnedPositions[i], forward * 1f);
                }
            }
        }
    }
}