using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentTrainingController : Agent
{
    // Ustawienia œrodowiska agenta
    [SerializeField]
    private GameObject mazeGeneratorObject; // Obiekt generatora labiryntu.
    [SerializeField]
    private GameObject environmentParentGameObject; // Rodzic œrodowiska, w którym dzia³a agent.
    [SerializeField]
    private float moveSpeed; // Prêdkoœc ruchu agenta
    [SerializeField]
    private float timeForEp; // Czas trwania epizodu
    [SerializeField]
    private float pointReactivationTime; // Czas reaktywacji punktu po podniesieniu

    // Definicje nagród i kar
    [SerializeField]
    private float wallReward; // Nagroda za uderzenie w œciane 
    [SerializeField]
    private float pointReward; // Nagroda za zdobycie punktu
    [SerializeField]
    private float timePenaltyReward; // Nagroda za przekroczenie czasu wyszukiwania celu
    [SerializeField]
    private float goalReward; // Nagroda za znalezienie celu
    [SerializeField]
    private float OneSecondPenalty; // Nagroda otrzymywana co okreœlony czas

    // Zmienne prywatne do kontroli logiki agenta
    private GameObject currentMazeInstance; //Aktualna instancja labiryntu
    private GameObject startingCell; // Komórka startowa w labiryncie
    private Vector3 startingCellPos; // Pozycja komórki startowej

    private float lastCollisionTime = -1f; // Ostatni czas kolizji
    private float collisionCooldown = 0.4f; // Czas odnowienia kolizji

    private Rigidbody rb; // Komponent Rigidbody agenta
    private Renderer agentRenderer; // Render agenta

    private List<GameObject> disabledPoints = new List<GameObject>(); // Lista wy³¹czonych punktów

    private float timeLeft; // Pozosta³y czas do koñca epizodu

    private float lastPenaltyTime; // Ostatni czas na³o¿enia kary
    
    // Inicjalizacja agenta
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        agentRenderer = GetComponent<Renderer>();
        lastPenaltyTime = Time.time;
    }

    // Aktualizacja stanu agenta w ka¿dej klatce
    private void Update()
    {
        CheckRemainingTime();
    }

    // Logika rozpoczêcia nowego epizodu
    public override void OnEpisodeBegin()
    {
        // Tworzenie nowego labiryntu, jeœli nie istnieje
        if (currentMazeInstance == null)
        {
            currentMazeInstance = Instantiate(mazeGeneratorObject, Vector3.zero, Quaternion.identity, environmentParentGameObject.transform);
            StartCoroutine(InitializeStartPosition());
        }

        StartCoroutine(InitializeStartPosition());
        EpisodeTimerNew();
        lastPenaltyTime = Time.time;
    }

    // Zbieranie obserwacji przez agenta
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
    }

    // Odbieranie i przetwarzanie akcji od agenta
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed * 2.5f, 0f, Space.Self);
    }

    // Heurystyka dla testowania agenta bez uczenia maszynowego
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }

    // Detekcja kolizji agenta
    public void OnTriggerEnter(Collider other)
    {


        if (Time.time - lastCollisionTime < collisionCooldown)
            // Rozwi¹zuje problem podwójnego collidera w jedej œcianie
            return; 
        
        /* Instrukcja warunkowa wykorzystywana przy treningu agenta
        if (other.CompareTag("OutsideWall") || other.CompareTag("InnerWall"))
        {
            
            lastCollisionTime = Time.time; // Update last collision time
            SetReward(wallReward);
            ResetDisabledPoints();
            agentRenderer.material.color = Color.red;
            EndEpisode();
        }
         */

        if (other.CompareTag("Point"))
        {
            SetReward(pointReward);
            disabledPoints.Add(other.gameObject);
            StartCoroutine(ReactivatePointAfterDelay(other.gameObject, pointReactivationTime));
            other.gameObject.SetActive(false);
        }

        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(goalReward);
            DestroyMazeElementsExceptTag(environmentParentGameObject,"Agent");
            currentMazeInstance = null;
            disabledPoints.Clear();
            agentRenderer.material.color = Color.green;
            EndEpisode();
        }
    }

    //-----------------------------------------------------------------------------------------


    private IEnumerator ReactivatePointAfterDelay(GameObject point, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (point != null)
        {
            point.SetActive(true);
            disabledPoints.Remove(point);
        }
    }

    // Ustawia wszystkie zebrane punkty na aktywne
    private void ResetDisabledPoints()
    {
        foreach (var point in disabledPoints)
        {
            point.SetActive(true);
        }
        disabledPoints.Clear();
    }

    // Zniszczenie wszystkich elementów za wyj¹tkiem elementów z wybranym tagiem
    private void DestroyMazeElementsExceptTag(GameObject parent, string tag)
    {
        foreach (Transform child in parent.transform)
        {
            if (!child.CompareTag(tag))
            {
                Destroy(child.gameObject);
            }
        }

    }

    // Ustala pozycje startow¹ agenta po stworzeniu labiryntu dla konkretnego œrodowiska
    private IEnumerator InitializeStartPosition()
    {
        yield return null;

        startingCell = FindChildWithTag(environmentParentGameObject, "StartCell");

        if (startingCell != null)
        {
            startingCellPos = startingCell.transform.localPosition;
            transform.localPosition = new Vector3(startingCellPos.x, .4f, startingCellPos.z);
            rb.angularVelocity = Vector3.zero; 
            rb.velocity = Vector3.zero;
            transform.rotation = Quaternion.identity;

        }
    }

    // Wyszukuje child element wybranego rodzica
    GameObject FindChildWithTag(GameObject parent, string tag)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }
        return null; 
    }

    // Rozpoczêcie liczenia czasu
    private void EpisodeTimerNew()
    {
        timeLeft = Time.time + timeForEp;
    }

    // Obs³uga zwi¹zane z czasem
    private void CheckRemainingTime()
    {
        //Sprawdzenie czy cel zostaje znaleziony w czasie
        if(Time.time >= timeLeft)
        {
            AddReward(timePenaltyReward);
            agentRenderer.material.color = Color.blue;
            ResetDisabledPoints();
            EndEpisode();
        }
        //Kara za sekunde wyszukiwania 
        else if (Time.time - lastPenaltyTime > 1.0f) 
        {
            AddReward(-OneSecondPenalty);
            lastPenaltyTime = Time.time;
        }
    }
}
