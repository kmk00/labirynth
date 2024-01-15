using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIBreadthFirstSearch : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
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
    int noWayCounter = 0;
    float timer = 0f;
    int globalBFS = 0;
    int maxBFS = 0;
    public Vector3 lastKnownCommon;
    public int lastKnownCommonNumber = 0;
    public Vector3 playerPosition;

    Queue<Vector3[]> BFS = new Queue<Vector3[]>();
    Queue<Vector3> playerBFS = new Queue<Vector3>();
    Queue<Vector3> destination = new Queue<Vector3>();
    Queue<Vector3> returnS = new Queue<Vector3>();
    Stack<Vector3> destinationStack = new Stack<Vector3>();
    Stack<Vector3> returnStack = new Stack<Vector3>();


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
                start = new Vector3(groundPosition.x, 0.6f, groundPosition.z);
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

        //PrintVisitedValues();
    }

    // Update is called once per frame
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
                    speed = 5f;
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
                    if (destinationPhase) //Je¿eli AI jest równo z ostatnim wspólnym miejscem, to znaczy, ¿e mo¿e wyszukiwaæ drogê do BFS.ElementAt(globalBFS).Last()
                    {
                        //Debug.Log("Idê");
                        //Debug.Log("Last Known Common: " + lastKnownCommon);
                        //Debug.Log("Last Known Common Number: " + lastKnownCommonNumber);
                        //Debug.Log("Player Position: " + playerPosition);
                        //lastKnownCommonNumber = BFS.ElementAt(lastKnownCommonNumber).Length-1;

                        if (destination.Count > 0)
                        {
                            //Debug.Log("Ide do: " + destinationStack.Peek());
                            //playerPosition = destinationStack.Pop();
                            playerPosition = destination.Dequeue();
                            returnS.Enqueue(playerPosition);
                            isMovingTowardsCenter = true;
                        }

                        //Debug.Log("Nastêpny: " + destinationStack.Peek());
                        //lastKnownCommon = playerPosition;

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
                        //CheckLastKnownCommon();
                        //Debug.Log("Cofam");
                        //Debug.Log("Last Known Common: " + lastKnownCommon);
                        //Debug.Log("Last Known Common Number: " + lastKnownCommonNumber);
                        //Debug.Log("Player Position: " + playerPosition);
                        //Debug.Log("Cofam do: " + returnS.Peek());
                        if (playerPosition != start)
                        {
                            // Debug.Log("Cofam do: " + returnS.Peek());
                            //ReverseReturnS();
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
                            //destinationStack = CheckNextDestination();
                            //returnS.Enqueue(start);
                            Debug.Log("GlobalBFS = " + globalBFS);
                        }
                    }
                }
                else if (checkPhase)
                {
                    if (Mathf.Approximately(0f, GetComponent<Rigidbody>().velocity.sqrMagnitude)) //Je¿eli nie rusza siê
                    {

                        FindAllWays();

                        /*foreach (Vector3 vector in BFS.ElementAt(globalBFS))
                        {
                            //playerBFS.Enqueue(vector);
                            Debug.Log(globalBFS + " + " + vector);
                        }
                        foreach (Vector3 vector in BFS.ElementAt(globalBFS+1))
                        {
                            //playerBFS.Enqueue(vector);
                            Debug.Log(globalBFS+1 + " + " + vector);
                        }*/

                        globalBFS += 1;
                        checkPhase = false;
                        movePhase = true;
                        destinationPhase = true;

                        if (playerPosition == start)
                        {
                            foreach (Vector3 vector in BFS.ElementAt(globalBFS))
                            {
                                destination.Enqueue(vector);
                                //Debug.Log(vector);
                            }
                            //destinationStack = CheckNextDestination();
                            //Debug.Log("GlobalBFS = " + globalBFS);
                        }
                        //CheckLastKnownCommon();
                        /*for (int i = 0; i < lastKnownCommonNumber-2; i++)
                        {
                            destination.Dequeue();
                        }*/



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
            //Debug.Log("Stack: " + destination.Peek());
            pathStack.Push(destination.Dequeue());
        }
        return pathStack;
    }

    public void FindAllWays()
    {
        float maxDistance = 2f;
        float rayAngle = 30f;
        int counter = 0;
        List<Vector3> tempList = new List<Vector3>();

        for (int i = 0; i < 4; ++i)
        {
            Quaternion rayRotation = Quaternion.Euler(rayAngle, i * 90f, 0); // Dodaj k¹t do wszystkich promieni
            Vector3 rayDirection = rayRotation * Vector3.forward;

            Ray ray = new Ray(transform.position, rayDirection);

            // Dodaj czerwony debugowy promieñ
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
                    /*foreach (Vector3 vector in BFS.ElementAt(maxBFS))
                    {
                        playerBFS.Enqueue(vector);
                        //Debug.Log(vector);
                    }*/
                    maxBFS++;
                    counter++;
                    //Debug.Log(maxBFS-counter);



                    isGoingBack = false;
                    //Debug.Log("Added: " + GetDirectionName(i));
                }
                else
                {
                    //Debug.Log("Not Added: " + i);
                }
            }
        }
    }

    public void RemoveDuplicatesFromReturnS()
    {
        // Stwórz now¹ kolejkê, dodaj¹c tylko pierwszy element (start) z oryginalnej kolejki
        Queue<Vector3> newReturnS = new Queue<Vector3>();
        HashSet<Vector3> uniquePositions = new HashSet<Vector3>();

        foreach (Vector3 position in returnS)
        {
            // Dodaj do nowej kolejki, je¿eli pozycja nie istnieje jeszcze w zbiorze
            if (uniquePositions.Add(position))
            {
                newReturnS.Enqueue(position);
            }
        }

        // Przypisz now¹ kolejkê do returnS
        returnS = newReturnS;
    }

    public void ReverseReturnS()
    {
        // Stwórz tymczasow¹ listê, aby odwróciæ elementy
        List<Vector3> tempList = new List<Vector3>(returnS);

        // Wyczyœæ kolejkê
        returnS.Clear();

        // Dodaj odwrócone elementy z listy z powrotem do kolejki
        for (int i = tempList.Count - 1; i >= 0; i--)
        {
            returnS.Enqueue(tempList[i]);
        }
    }

    string GetDirectionName(int direction)
    {
        switch (direction)
        {
            case 0: return "Forward";
            case 1: return "Right";
            case 2: return "Backward";
            case 3: return "Left";
            default: return "Unknown";
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