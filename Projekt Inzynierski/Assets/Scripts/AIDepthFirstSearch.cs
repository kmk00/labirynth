using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class AIDepthFirst : MonoBehaviour
{
    public Stack<MazeCellStack> maze = new Stack<MazeCellStack>();
    bool[,,] visited;
    bool isGoingBack = false;
    bool isEnd = false;
    bool isMovingTowardsCenter = false;
    private Vector3 targetCenterPosition;
    private Vector3 finalPosition = new Vector3(0,0,0);
    Stack<Vector3> pathStack = new Stack<Vector3>(); 
    // Start is called before the first frame update
    void Start()
    {
        Vector3 start = transform.position;
        pathStack.Push(start);
        //inicjalizacja wszystkich zmiennych na fa³sz
        int maxX = 1000;
        int maxY = 1000;
        int maxZ = 1000;
        visited = new bool[maxX, maxY, maxZ];

        visited[(int)start.x, (int)start.y, (int)start.z] = true;

        GameObject endingCell = GameObject.Find("Ending Cell(Clone)");
        if (endingCell != null)
        {
            Transform groundTransform = endingCell.transform.Find("Ground");

            if (groundTransform != null)
            {
                finalPosition = groundTransform.position;
            }
            else
            {
                Debug.LogError("Ground object not found inside Ending Cell(Clone).");
            }
        }
        else
        {
            Debug.LogError("Ending Cell(Clone) not found.");
        }

        Time.timeScale = 10f;
        PauseForSeconds((float)0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        finalPosition.y = transform.position.y;
        if (isEnd == false)
        {
            if (isMovingTowardsCenter)
            {
                float speed = 5.0f;
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetCenterPosition, step);
                if (transform.position == targetCenterPosition) { isMovingTowardsCenter = false; }
            }
            else if (isMovingTowardsCenter == false & isGoingBack == false)
            {
                if (Mathf.Approximately(0f, GetComponent<Rigidbody>().velocity.sqrMagnitude))
                {
                    
                    FindAllWays();
                    if (isGoingBack==true) { pathStack.Pop(); }
                    
                }

            }
            else
            {
                if (pathStack.Count > 0)
                {
                    Vector3 nextWaypoint = pathStack.Peek();
                    isMovingTowardsCenter = true;
                    targetCenterPosition = new Vector3(nextWaypoint.x, transform.position.y, nextWaypoint.z);
                    isGoingBack = false;
                }
                else
                {
                    // No more waypoints in the stack, stop going back
                    isGoingBack = false;
                    Debug.Log("Nie ma drogi!");
                }
            }
            if (transform.position == finalPosition)
            {
                isEnd = true;
            }
        }
        
    }

    public Vector3 GetNextCenter(MazeCellStack s)
    {
        return s.GetCenter();
    }
    public void FindAllWays()
    {
        //K¹t
        float rayAngle = 30f;
        Quaternion rayRotation;
        Vector3 rayDirection;
        Ray ray;
        float maxDistance = 2f;

        //Debug.DrawRay(transform.position, rayDirection, Color.red);
        for (int i = 0; i < 4; ++i)
        {
            switch (i)
            {
                case 0:
                    {
                        rayRotation = Quaternion.Euler(rayAngle, 0, 0);
                        rayDirection = rayRotation * Vector3.forward;
                        ray = new Ray(transform.position, rayDirection);
                        Debug.DrawRay(transform.position, rayDirection, Color.red);
                        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                        {
                            if (hit.collider.gameObject.name == "Cube" & hit.collider.gameObject.transform.parent.name == "Ground" & !CheckVisited(hit.collider.gameObject.transform.position))
                            {
                                Debug.Log("Ray hit a Cube");
                                Transform hitObjectTransform = hit.collider.gameObject.transform;
                                visited[(int)hitObjectTransform.position.x, (int)hitObjectTransform.position.y, (int)hitObjectTransform.position.z] = true;
                                //Vector3 centerOfHitObject = new Vector3(hitObjectTransform.position.x, transform.position.y, hitObjectTransform.position.z );
                                targetCenterPosition = new Vector3(hitObjectTransform.position.x, transform.position.y, hitObjectTransform.position.z);
                                pathStack.Push(targetCenterPosition);
                                isMovingTowardsCenter = true;
                                i = 4;
                                isGoingBack = false;
                            }
                            else
                            {
                                Debug.Log("Ray hit else");
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        rayRotation = Quaternion.Euler(-rayAngle, 0, 0);
                        rayDirection = rayRotation * Vector3.back;
                        ray = new Ray(transform.position, rayDirection);
                        Debug.DrawRay(transform.position, rayDirection, Color.red);
                        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                        {
                            if (hit.collider.gameObject.name == "Cube" & hit.collider.gameObject.transform.parent.name == "Ground" & !CheckVisited(hit.collider.gameObject.transform.position))
                            {
                                Debug.Log("Ray hit a Cube");
                                Transform hitObjectTransform = hit.collider.gameObject.transform;
                                visited[(int)hitObjectTransform.position.x, (int)hitObjectTransform.position.y, (int)hitObjectTransform.position.z] = true;
                                //Vector3 centerOfHitObject = new Vector3(hitObjectTransform.position.x, transform.position.y, hitObjectTransform.position.z );
                                targetCenterPosition = new Vector3(hitObjectTransform.position.x, transform.position.y, hitObjectTransform.position.z);
                                isMovingTowardsCenter = true;
                                pathStack.Push(targetCenterPosition);
                                i = 4;
                                isGoingBack = false;
                            }
                            else
                            {
                                Debug.Log("Ray hit else");
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        rayRotation = Quaternion.Euler(0, 0, rayAngle);
                        rayDirection = rayRotation * Vector3.left;
                        ray = new Ray(transform.position, rayDirection);
                        Debug.DrawRay(transform.position, rayDirection, Color.red);
                        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                        {
                            if (hit.collider.gameObject.name == "Cube" & hit.collider.gameObject.transform.parent.name == "Ground" & !CheckVisited(hit.collider.gameObject.transform.position))
                            {
                                Debug.Log("Ray hit a Cube");
                                Transform hitObjectTransform = hit.collider.gameObject.transform;
                                visited[(int)hitObjectTransform.position.x, (int)hitObjectTransform.position.y, (int)hitObjectTransform.position.z] = true;
                                //Vector3 centerOfHitObject = new Vector3(hitObjectTransform.position.x, transform.position.y, hitObjectTransform.position.z );
                                targetCenterPosition = new Vector3(hitObjectTransform.position.x, transform.position.y, hitObjectTransform.position.z);
                                isMovingTowardsCenter = true;
                                pathStack.Push(targetCenterPosition);
                                i = 4;
                                isGoingBack = false;
                            }
                            else
                            {
                                Debug.Log("Ray hit else");
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        rayRotation = Quaternion.Euler(0, 0, -rayAngle);
                        rayDirection = rayRotation * Vector3.right;
                        ray = new Ray(transform.position, rayDirection);
                        Debug.DrawRay(transform.position, rayDirection, Color.red);
                        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                        {
                            if (hit.collider.gameObject.name == "Cube" & hit.collider.gameObject.transform.parent.name == "Ground" & !CheckVisited(hit.collider.gameObject.transform.position))
                            {
                                Debug.Log("Ray hit a Cube");
                                Transform hitObjectTransform = hit.collider.gameObject.transform;
                                visited[(int)hitObjectTransform.position.x, (int)hitObjectTransform.position.y, (int)hitObjectTransform.position.z] = true;
                                //Vector3 centerOfHitObject = new Vector3(hitObjectTransform.position.x, transform.position.y, hitObjectTransform.position.z );
                                targetCenterPosition = new Vector3(hitObjectTransform.position.x, transform.position.y, hitObjectTransform.position.z);
                                isMovingTowardsCenter = true;
                                pathStack.Push(targetCenterPosition);
                                i = 4;
                            }
                            else
                            {
                                Debug.Log("Ray hit else");
                                isGoingBack = true;
                                Debug.Log("Going Back!");
                            }
                        }
                        break;
                    }

            }
        }
    }

    public bool CheckVisited(Vector3 location)
    {
        int x = (int)location.x;
        int y = (int)location.y;
        int z = (int)location.z;

        // Check if the provided location is within the array bounds
        if (x >= 0 && x < visited.GetLength(0) &&
            y >= 0 && y < visited.GetLength(1) &&
            z >= 0 && z < visited.GetLength(2))
        {
            return visited[x, y, z];
        }

        return false;
    }

    IEnumerator PauseForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

}

public class MazeCellStack : MonoBehaviour
{
    private Vector3 center = Vector3.zero;

    public Vector3 GetCenter()
    { return center; }
}
