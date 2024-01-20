using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIBreadthFirstSearch : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;
    bool[,,] visited;
    bool isGoingBack = false;
    bool isEnd = false;
    bool isMovingTowardsCenter = false;
    bool isPaused = true;

    bool movePhase = false;
    bool checkPhase = true;

    bool destinationPhase = false;
    bool goBackPhase = false;

    private Vector3 targetCenterPosition;
    private Vector3 finalPosition = new Vector3(0, 0, 0);
    public Vector3 start;
    float timer = 0f;
    int globalBFS = 0;
    public Vector3 lastKnownCommon;
    public int lastKnownCommonNumber = 0;
    public Vector3 playerPosition;

    Queue<Vector3[]> BFS = new Queue<Vector3[]>();
    Queue<Vector3> destination = new Queue<Vector3>();
    Queue<Vector3> returnS = new Queue<Vector3>();


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
                Vector3 groundPosition = groundTransform.position;
                start = new Vector3(groundPosition.x, 20.6f, groundPosition.z);
                BFS.Enqueue(new Vector3[] { start });
                visited[(int)start.x + maxX, (int)start.y + maxY, (int)start.z + maxZ] = true;
                playerPosition = start;
                lastKnownCommon = playerPosition;
                returnS.Enqueue(start);
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
                    transform.position = Vector3.MoveTowards(transform.position, playerPosition, step);
                    if (transform.position == playerPosition)
                    {
                        isMovingTowardsCenter = false;
                        isGoingBack = false;
                        if (Vector3.Equals(playerPosition, BFS.ElementAt(globalBFS).Last()))
                        {
                            movePhase = false;
                            checkPhase = true;
                            Debug.Log("BFS wzrasta");
                        }
                    }
                }
                else if (movePhase)
                {
                    if (destinationPhase)
                    {

                        if (destination.Count > 0)
                        {
                            playerPosition = destination.Dequeue();
                            returnS.Enqueue(playerPosition);
                            isMovingTowardsCenter = true;
                        }
                        else if (destination.Count == 0)
                        {
                            RemoveDuplicatesFromReturnS();
                            ReverseReturnS();
                            destinationPhase = false;
                            goBackPhase = true;
                        }
                    }
                    else if (goBackPhase && !isGoingBack)
                    {
                        if (playerPosition != start)
                        {
                            RemoveDuplicatesFromReturnS();
                            Debug.Log("ReturnS:");
                            foreach (Vector3 vector in returnS)
                            {
                                Debug.Log(vector);
                            }

                            playerPosition = returnS.Dequeue();
                            isMovingTowardsCenter = true;
                            isGoingBack = true;

                        }
                        else
                        {
                            returnS.Clear();
                            goBackPhase = false;
                            destinationPhase = true;
                            isGoingBack = false;
                            foreach (Vector3 vector in BFS.ElementAt(globalBFS))
                            {
                                destination.Enqueue(vector);
                                Debug.Log(vector);
                            }
                            Debug.Log("GlobalBFS = " + globalBFS);
                        }
                    }
                }
                else if (checkPhase)
                {
                    if (Mathf.Approximately(0f, GetComponent<Rigidbody>().velocity.sqrMagnitude))
                    {

                        FindAllWays();
                        globalBFS += 1;
                        checkPhase = false;
                        movePhase = true;
                        destinationPhase = true;

                        if (playerPosition == start)
                        {
                            foreach (Vector3 vector in BFS.ElementAt(globalBFS))
                            {
                                destination.Enqueue(vector);
                            }
                        }
                    }
                }
            }
        }
        if (transform.position == finalPosition)
        {
            isEnd = true;
        }
    }


    public Stack<Vector3> CheckNextDestination()
    {
        Stack<Vector3> pathStack = new Stack<Vector3>();
        while (destination.Count > 0)
        {
            pathStack.Push(destination.Dequeue());
        }
        return pathStack;
    }

    public void FindAllWays()
    {
        float maxDistance = 2f;
        float rayAngle = 30f;
        List<Vector3> tempList = new List<Vector3>();

        for (int i = 0; i < 4; ++i)
        {
            Quaternion rayRotation = Quaternion.Euler(rayAngle, i * 90f, 0);
            Vector3 rayDirection = rayRotation * Vector3.forward;
            Ray ray = new Ray(transform.position, rayDirection);
            Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                if (hit.collider.gameObject.name == "Cube" &&
                    hit.collider.gameObject.transform.parent.name == "Ground" &&
                    !CheckVisited(hit.collider.gameObject.transform.position))
                {
                    Transform hitObjectTransform = hit.collider.gameObject.transform;
                    visited[(int)hitObjectTransform.position.x + 100, (int)hitObjectTransform.position.y + 100, (int)hitObjectTransform.position.z + 100] = true;
                    targetCenterPosition = new Vector3(hitObjectTransform.position.x, transform.position.y, hitObjectTransform.position.z);
                    tempList = returnS.ToList();
                    tempList.Add(targetCenterPosition);
                    BFS.Enqueue(tempList.ToArray());
                    tempList.Clear();
                    isGoingBack = false;
                }
            }
        }
    }

    public void RemoveDuplicatesFromReturnS()
    {
        Queue<Vector3> newReturnS = new Queue<Vector3>();
        HashSet<Vector3> uniquePositions = new HashSet<Vector3>();

        foreach (Vector3 position in returnS)
        {
            if (uniquePositions.Add(position))
            {
                newReturnS.Enqueue(position);
            }
        }
        returnS = newReturnS;
    }

    public void ReverseReturnS()
    {
        List<Vector3> tempList = new List<Vector3>(returnS);
        returnS.Clear();
        for (int i = tempList.Count - 1; i >= 0; i--)
        {
            returnS.Enqueue(tempList[i]);
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