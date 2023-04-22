using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    static GameStateManager instance;
    public static GameStateManager Instance
    {
        get { 
            if (instance == null)
                instance = FindObjectOfType<GameStateManager>();
            return instance;
        }
    }

    public enum GameState
    {
        MainMenu,
        Renovation,
        Dining
    }

    [ContextMenu("PrintGroups")]
    void PrintGroups()
    {
        activeGroups.ForEach(g =>
        {
            g.PrintCustomers();
        });
    }

    [SerializeField]
    CustomerController customerPrefab;
    [SerializeField]
    Transform clockHand;
    [SerializeField]
    GameObject startDayButton;
    [SerializeField]
    GrabItemReference[] possibleFoods;
    [SerializeField]
    ApplianceCore[] possibleAppliances;
    [SerializeField]
    Transform applianceSpawnPos;
    [SerializeField]
    float doorPatience;
    public float DoorPatience => doorPatience;
    [SerializeField]
    float orderPatience;
    public float OrderPatience => orderPatience;
    [SerializeField]
    float foodPatience;
    public float FoodPatience => foodPatience;
    [Header("Game Balance")]
    [SerializeField]
    Vector3 customerCountBTS;
    [SerializeField]
    AnimationCurve customerCountCurve;
    [SerializeField]
    Vector3 dayLengthBTS;
    [SerializeField]
    AnimationCurve dayLengthCurve;
    [SerializeField]
    Transform playerTransform;
    [SerializeField]
    Transform gameOverTeleportPoint;

    static bool testingSkipPathfinding = false;
    public static bool TestingSkipPathfinding
    {
        get { return testingSkipPathfinding; }
        set { testingSkipPathfinding = value; }
    }


    #region group pool
    Queue<CustomerGroup> groupPool = new();
    List<CustomerGroup> activeGroups = new();
    CustomerGroup SpawnCustomerGroup()
    {
        if (groupPool.Count == 0)
            groupPool.Enqueue(new());
        var newGroup = groupPool.Dequeue();
        activeGroups.Add(newGroup);
        return newGroup;
    }
    void DespawnCustomerGroup(CustomerGroup toDespawn)
    {
        activeGroups.Remove(toDespawn);
        groupPool.Enqueue(toDespawn);
    }
    #endregion
    #region customer pool
    Queue<CustomerController> customerPool = new();
    protected CustomerController SpawnCustomer()
    {
        if (customerPool.Count == 0)
            customerPool.Enqueue(Instantiate(customerPrefab));
        var newCustomer = customerPool.Dequeue();
        newCustomer.transform.position = transform.position;
        newCustomer.transform.rotation = transform.rotation;
        newCustomer.gameObject.SetActive(true);
        newCustomer.ResetState();
        return newCustomer;
    }
    protected void DespawnCustomer(CustomerController toDespawn)
    {
        customerPool.Enqueue(toDespawn);
        toDespawn.gameObject.SetActive(false);
    }
    #endregion

    GameState currentState;
    public event System.Action<GameState> OnStateChange;
    public GameState CurrentState
    {
        get => currentState;
        private set
        {
            currentState = value;
            OnStateChange?.Invoke(currentState);
        }
    }

    public int GetRandomFood() => possibleFoods[Random.Range(0, possibleFoods.Length)].Id;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        CurrentState = GameState.Renovation;
        currentDay = MainMenuUIManager.StartAtDay;
    }

    Coroutine currentCustomerSpawner;

    int currentDay = 1;
    public void StartDay()
    {
        int customerCount = (int)EvaluateDifficultyCurve(customerCountBTS, customerCountCurve);
        int dayLength = (int)EvaluateDifficultyCurve(dayLengthBTS, dayLengthCurve);
        if (currentDay == 0)
        {
            customerCount = 1;
            dayLength = 60;
        }
        StartCustomDay(customerCount, 2, dayLength);
    }

    public void StartCustomDay(int customerCount, int maxGroupSize, float minnimumDayLength)
    {
        currentCustomerSpawner = StartCoroutine(CustomerSpawner(customerCount, maxGroupSize, minnimumDayLength));
        CurrentState = GameState.Dining;
        startDayButton.SetActive(false);
    }

    float EvaluateDifficultyCurve(Vector3 difficultyBTS, AnimationCurve difficultyCurve)
    {
        float basis = difficultyBTS.x + Mathf.Floor(currentDay / difficultyBTS.y) * difficultyBTS.z;
        return basis + difficultyCurve.Evaluate((currentDay % difficultyBTS.y)/ difficultyBTS.y) * difficultyBTS.z;
    }

    public void GameOver()
    {
        if (CurrentState == GameState.Dining && currentDay!=0)
        {
            if (currentCustomerSpawner != null)
                StopCoroutine(currentCustomerSpawner);
            CurrentState = GameState.Renovation;
            playerTransform.position = gameOverTeleportPoint.position;
        }
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator CustomerSpawner(int customerCount, int maxGroupSize, float minnimumDayLength)
    {
        TutorialController.Instance.AdvanceStage(0);
        TutorialController.Instance.AdvanceStage(1);
        TutorialController.Instance.AdvanceStage(2);
        TutorialController.Instance.AdvanceStage(10);

        Queue<int> groupQueue = new();
        for (int i = 0; i < customerCount; i++)
        {
            int groupSize = Random.Range(1, Mathf.Max(1, Mathf.Min(maxGroupSize, customerCount-i))+1);
            i += groupSize-1;
            groupQueue.Enqueue(groupSize);
        }
        float dayLength = Mathf.Max(groupQueue.Count*10, minnimumDayLength);
        float timeBetweenGroups = groupQueue.Count != 1 ? dayLength / (groupQueue.Count - 1) : 0;
        Debug.Log("groupCount " + groupQueue.Count);
        Debug.Log("total " + dayLength);
        Debug.Log("timing " + timeBetweenGroups);

        yield return null;

        var clockCoroutine = StartCoroutine(CoroutineHelpers.Timer(dayLength, p=> clockHand.localRotation = Quaternion.Euler(0, 0, p * 360) , null));
        while (groupQueue.Count > 0)
        {
            int groupSize = groupQueue.Dequeue();
            SpawnCustomerGroup().SetCustomerCount(groupSize).BeginLogic();
            if(groupQueue.Count != 0)
            {
                Debug.Log("Waiting for group cooldown of: " + timeBetweenGroups);
                yield return new WaitForSeconds(timeBetweenGroups);
            }
        }
        Debug.Log("Waiting for groups to complete");
        yield return new WaitUntil(()=> activeGroups.Count==0);

        Debug.Log("Waiting for Clock");
        yield return clockCoroutine;

        Debug.Log("Day Complete");
        CurrentState = GameState.Renovation;
        startDayButton.SetActive(true);

        TutorialController.Instance.AdvanceStage(9);
        var newAppliance = Instantiate(possibleAppliances[Random.Range(0, possibleAppliances.Length)]);
        newAppliance.transform.position = applianceSpawnPos.position;
        newAppliance.EnterBox();

        currentDay++;
        PlayerPrefs.SetInt("maxDays", Mathf.Max(currentDay, PlayerPrefs.GetInt("maxDays")));
    }

    public class CustomerGroup
    {
        List<CustomerController> customers = new();
        public List<CustomerController> Customers => customers;
        TableAppliance reservedTable;
        int customersToSpawn = 0;

        public int GroupSize=>customers.Count;

        public enum CustomerState
        {
            None,
            Pathfinding,
            WaitingAtDoor,
            WaitingForOrder,
            WaitingForFood,
            Eating
        }
        public CustomerState CurrentState { get; private set; }
        public float DoorPatience { get; private set; }

        //builder pattern
        public CustomerGroup SetCustomerCount(int customersToSpawn)
        {
            this.customersToSpawn = customersToSpawn;
            return this;
        }

        public void BeginLogic()
        {
            Instance.StartCoroutine(CustomerLogic());
        }

        public void PrintCustomers()
        {
            Debug.Log(string.Join(", ", customers.Select(c=>c.name)));
        }

        private IEnumerator CustomerLogic() 
        {
            if(customersToSpawn==0)
                yield break;

            customers.Clear();
            for (int i = 0; i < customersToSpawn; i++)
            {
                customers.Add(Instance.SpawnCustomer());
            }
            reservedTable = null;
            customers.ForEach(c => c.Agent.stoppingDistance = 1);
            CurrentState = CustomerState.WaitingAtDoor;
            DoorPoint.Instance.AddGroup(this);
            timerCoroutine = Instance.StartCoroutine(
                CoroutineHelpers.Timer(Instance.FoodPatience, p => DoorPatience = p, Instance.GameOver)
                );
            customers.ForEach(c => c.TriggerAnimation("Idle"));
            Debug.Log("Door");
            yield return WaitUntilState(CustomerState.Pathfinding);
            customers.ForEach(c => c.Agent.stoppingDistance = 0);
            Debug.Log("Found Table");
            if (timerCoroutine != null)
                Instance.StopCoroutine(timerCoroutine);
            if (false)
            {
                //has cash register

                //  pathfind customers to cash register
                //  wait for user to interact with cash regsiter
                //  determine orders of all customers
                //  pathfind customers to table
            }
            else
            {

                var activeChairs = reservedTable.ActiveChairs;
                Debug.Log(string.Join(", ", activeChairs as object[]));
                for (int i = 0; i < customers.Count; i++)
                {
                    customers[i].Agent.destination = activeChairs[i].transform.position;
                }
                yield return WaitForPathfinding(CustomerState.WaitingForOrder);
                Debug.Log("Seat");
                TutorialController.Instance.AdvanceStage(3);
                for (int i = 0; i < customers.Count; i++)
                {
                    customers[i].transform.position = customers[i].Agent.destination;
                    customers[i].transform.rotation = Quaternion.Euler(0,i switch
                    {
                        0=>180,
                        1=>0,
                        2=>270,
                        3=>90,
                        _=>0
                    },0);
                    customers[i].TriggerAnimation("Sit");
                    //customers[i].Agent.enabled = false;
                }
                reservedTable.StartWaiting();
                yield return WaitUntilState(CustomerState.WaitingForFood);
                Debug.Log("Ordered");
                TutorialController.Instance.AdvanceStage(4);
            }

            foreach (var customer in customers)
            {
                yield return new WaitUntil(() => customer.RecievedFood);
            }
            Debug.Log("Eating");
            TutorialController.Instance.AdvanceStage(8);
            customers.ForEach(c => c.TriggerAnimation("Eat"));
            yield return new WaitForSeconds(10);
            Debug.Log("Leaving");
            customers.ForEach(c => c.Agent.destination = Instance.transform.position);
            reservedTable.ClearTable();
            reservedTable = null;
            CurrentState = CustomerState.Pathfinding;
            yield return WaitForPathfinding(CustomerState.None);

            customers.ForEach(c => Instance.DespawnCustomer(c));
            Instance.DespawnCustomerGroup(this);
            yield return null;
        }

        public int TryDeliverFood(int foodID)
        {
            foreach (var customer in customers)
            {
                if (customer.TryDeliverFood(foodID))
                {
                    return customers.IndexOf(customer);
                }
            }
            return -1;
        }

        Coroutine timerCoroutine;

        public Transform LastCustomerTransform() => customers.Last().transform;

        public void UpdateDoorPathfinding()
        {
            if (CurrentState != CustomerState.WaitingAtDoor)
                return;
            for (int i = 0; i < customers.Count; i++)
            {
                if (i == 0)
                    customers[i].PersistentTarget = DoorPoint.Instance.GetPersistantTarget(this);
                else
                    customers[i].PersistentTarget = customers[i - 1].transform;
            }
        }

        public void ReserveTable(TableAppliance table)
        {
            print("Table has been reserved! You're in, bitch!");
            Debug.Log(table);
            CurrentState = CustomerState.Pathfinding;
            customers.ForEach(c => c.PersistentTarget = null);
            reservedTable = table;
        }

        public void TakeOrder()
        {
            customers.ForEach(c => c.PickFood());
            CurrentState = CustomerState.WaitingForFood;
        }

        private IEnumerator WaitUntilState(CustomerState targetState) => new WaitUntil(() => CurrentState == targetState);
        private IEnumerator WaitForPathfinding(CustomerState stateOnComplete)
        {
            yield return WaitUntilState(CustomerState.Pathfinding);
            foreach (var customer in customers)
            {
                customer.Agent.enabled = true;
                customer.TriggerAnimation("Walk");
            }
            Debug.Log("Waiting for pathfinding...");
            if (!TestingSkipPathfinding)
            {
                yield return CoroutineHelpers.WaitUntilAll(customers, c => CurrentState != CustomerState.Pathfinding || (c.transform.position - c.Agent.destination).magnitude < 0.25f);
            }
            Debug.Log("Pathfinding complete");
            if (CurrentState == CustomerState.Pathfinding)
                CurrentState = stateOnComplete;
        }

        public void SetNewItemID(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
