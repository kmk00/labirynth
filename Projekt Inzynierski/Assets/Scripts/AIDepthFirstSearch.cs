using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDepthFirst : MonoBehaviour
{

    [SerializeField]
    private float speed;

    public Stack<MazeCellStack> maze = new Stack<MazeCellStack>();
    bool[,,] visited;
    bool isGoingBack = false;
    bool isEnd = false;
    bool isMovingTowardsCenter = false;
    bool isPaused = true;
    private Vector3 targetCenterPosition;
    private Vector3 finalPosition = new Vector3(0, 0, 0);
    Stack<Vector3> pathStack = new Stack<Vector3>();
    public Vector3 start;
    int noWayCounter = 0;
    float timer = 0f;

    void PrintVisitedValues()
    {
        for (int x = 0; x < visited.GetLength(0); x++)
        {
            for (int y = 0; y < visited.GetLength(1); y++)
            {
                for (int z = 0; z < visited.GetLength(2); z++)
                {
                    if (visited[x, y, z])
                    {
                        Debug.Log($"Visited: ({x - 100}, {z - 100})");
                    }
                }
            }
        }
    }

    void Start()
    {
        Time.timeScale = 1f;
        int maxX = 100;
        int maxY = 100;
        int maxZ = 100;
        visited = new bool[2 * maxX, 2 * maxY, 2 * maxZ];
        GameObject aimazeObject = GameObject.Find("AIMaze");

        GameObject endingCell = GameObject.Find("Ending Cell(Clone)");

        if (aimazeObject != null)
        {
            Transform endingCellTransform = aimazeObject.transform.Find("Ending Cell(Clone)");
            if (endingCellTransform != null) { endingCell = endingCellTransform.gameObject; }
        }

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

        GameObject startingCell = GameObject.Find("Starting Cell(Clone)");

        if (aimazeObject != null)
        {
            Transform startingCellTransform = aimazeObject.transform.Find("Starting Cell(Clone)");
            if (startingCellTransform != null) { startingCell = startingCellTransform.gameObject; }
        }

        if (startingCell != null)
        {
            Transform groundTransform = startingCell.transform.Find("Ground");

            if (groundTransform != null)
            {
                start = groundTransform.position;
                pathStack.Push(start);
                visited[(int)start.x + maxX, (int)start.y + maxY, (int)start.z + maxZ] = true;
            }
            else
            {
                Debug.LogError("Ground object not found inside Starting Cell(Clone).");
            }
        }
        else
        {
            Debug.LogError("Starting Cell(Clone) not found.");
        }

        PrintVisitedValues();

    }


    void Update()
    {
        if (isPaused == true)
        {
            if (timer < 1f)
            {
                timer += Time.deltaTime;
            }
            else
            {
                isPaused = false;
            }
        }
        else
        {
            finalPosition.y = transform.position.y;
            if (isEnd == false)
            {
                if (isMovingTowardsCenter)
                {

                    float step = speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, targetCenterPosition, step);
                    if (transform.position == targetCenterPosition) { isMovingTowardsCenter = false; }
                }
                else if (isMovingTowardsCenter == false & isGoingBack == false)
                {
                    if (Mathf.Approximately(0f, GetComponent<Rigidbody>().velocity.sqrMagnitude))
                    {
                        FindAllWays();
                        if (isGoingBack == true) { pathStack.Pop(); } else { noWayCounter = 0; }
                    }
                }
                else
                {
                    if (Mathf.Approximately(0f, GetComponent<Rigidbody>().velocity.sqrMagnitude))
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
                            if (noWayCounter == 3)
                            {
                                isEnd = true;
                                Debug.Log("Nie ma drogi!");
                                PrintVisitedValues();
                            }
                            else
                            {
                                isGoingBack = false;
                                noWayCounter++;
                            }
                        }
                    }
                }
                if (transform.position == finalPosition)
                {
                    isEnd = true;
                }
            }
        }
    }

    public Vector3 GetNextCenter(MazeCellStack s)
    {
        return s.GetCenter();
    }

    public void FindAllWays()
    {
        int i = 0;
        float rayAngle = 30f;
        Quaternion rayRotation;
        Vector3 rayDirection;
        Ray ray;
        float maxDistance = 2f;

        for (i = 0; i < 4; ++i)
        {
            switch (i)
            {
                case 0:
                    rayRotation = Quaternion.Euler(rayAngle, 0, 0);
                    rayDirection = rayRotation * Vector3.forward;
                    ray = new Ray(transform.position, rayDirection);
                    Debug.DrawRay(transform.position, rayDirection, Color.red);
                    if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                    {
                        if (hit.collider.gameObject.name == "Cube" && hit.collider.gameObject.transform.parent.name == "Ground" && !CheckVisited(hit.collider.gameObject.transform.position))
                        {
                            Transform hitObjectTransform = hit.collider.gameObject.transform;
                            visited[(int)hitObjectTransform.position.x + 100, (int)hitObjectTransform.position.y + 100, (int)hitObjectTransform.position.z + 100] = true;
                            targetCenterPosition = new Vector3(hitObjectTransform.position.x, transform.position.y, hitObjectTransform.position.z);
                            pathStack.Push(targetCenterPosition);
                            isMovingTowardsCenter = true;
                            i = 5;
                            isGoingBack = false;
                            Debug.Log("Up Added");
                        }
                        else
                        {
                            Debug.Log("1");
                        }
                    }
                    break;
                case 1:
                    rayRotation = Quaternion.Euler(-rayAngle, 0, 0);
                    rayDirection = rayRotation * Vector3.back;
                    ray = new Ray(transform.position, rayDirection);
                    if (Physics.Raycast(ray, out hit, maxDistance))
                    {
                        if (hit.collider.gameObject.name == "Cube" && hit.collider.gameObject.transform.parent.name == "Ground" && !CheckVisited(hit.collider.gameObject.transform.position))
                        {
                            Transform hitObjectTransform = hit.collider.gameObject.transform;
                            visited[(int)hitObjectTransform.position.x + 100, (int)hitObjectTransform.position.y + 100, (int)hitObjectTransform.position.z + 100] = true;
                            targetCenterPosition = new Vector3(hitObjectTransform.position.x, transform.position.y, hitObjectTransform.position.z);
                            isMovingTowardsCenter = true;
                            pathStack.Push(targetCenterPosition);
                            i = 5;
                            isGoingBack = false;
                            Debug.Log("Down Added");
                        }
                        else
                        {
                            Debug.Log("2");
                        }
                    }
                    break;
                case 2:
                    rayRotation = Quaternion.Euler(0, 0, rayAngle);
                    rayDirection = rayRotation * Vector3.left;
                    ray = new Ray(transform.position, rayDirection);
                    if (Physics.Raycast(ray, out hit, maxDistance))
                    {
                        if (hit.collider.gameObject.name == "Cube" && hit.collider.gameObject.transform.parent.name == "Ground" && !CheckVisited(hit.collider.gameObject.transform.position))
                        {
                            Transform hitObjectTransform = hit.collider.gameObject.transform;
                            visited[(int)hitObjectTransform.position.x + 100, (int)hitObjectTransform.position.y + 100, (int)hitObjectTransform.position.z + 100] = true;
                            targetCenterPosition = new Vector3(hitObjectTransform.position.x, transform.position.y, hitObjectTransform.position.z);
                            isMovingTowardsCenter = true;
                            pathStack.Push(targetCenterPosition);
                            i = 5;
                            isGoingBack = false;
                            Debug.Log("Left Added");
                        }
                        else
                        {
                            Debug.Log("3");
                        }
                    }
                    break;
                case 3:
                    rayRotation = Quaternion.Euler(0, 0, -rayAngle);
                    rayDirection = rayRotation * Vector3.right;
                    ray = new Ray(transform.position, rayDirection);
                    if (Physics.Raycast(ray, out hit, maxDistance))
                    {
                        if (hit.collider.gameObject.name == "Cube" && hit.collider.gameObject.transform.parent.name == "Ground" && !CheckVisited(hit.collider.gameObject.transform.position))
                        {
                            Transform hitObjectTransform = hit.collider.gameObject.transform;
                            visited[(int)hitObjectTransform.position.x + 100, (int)hitObjectTransform.position.y + 100, (int)hitObjectTransform.position.z + 100] = true;
                            targetCenterPosition = new Vector3(hitObjectTransform.position.x, transform.position.y, hitObjectTransform.position.z);
                            isMovingTowardsCenter = true;
                            pathStack.Push(targetCenterPosition);
                            i = 5;
                            isGoingBack = false;
                            Debug.Log("Right Added");
                        }
                        else
                        {
                            Debug.Log("4");
                            isGoingBack = true;
                            Debug.Log("Going Back!");
                        }
                    }
                    break;
            }
        }
    }

    public bool CheckVisited(Vector3 location)
    {
        int x = (int)location.x + 100;
        int y = (int)location.y + 100;
        int z = (int)location.z + 100;

        if (x >= 0 && x < visited.GetLength(0) &&
            y >= 0 && y < visited.GetLength(1) &&
            z >= 0 && z < visited.GetLength(2))
        {
            return visited[x, y, z];
        }

        return false;
    }
}

public class MazeCellStack : MonoBehaviour
{
    private Vector3 center = Vector3.zero;

    public Vector3 GetCenter()
    {
        return center;
    }
}
