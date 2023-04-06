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

    [SerializeField]
    CustomerController customerPrefab;
    [SerializeField]
    GrabItemReference[] possibleFoods;
    [field: SerializeField]
    public float DoorPatience { get; private set; }
    [field: SerializeField]
    public float OrderPatience { get; private set; }
    [field: SerializeField]
    public float FoodPatience { get; private set; }

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
        return newCustomer;
    }
    protected void DespawnCustomer(CustomerController toDespawn)
    {
        customerPool.Enqueue(toDespawn);
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

    public int GetRandomFood() => possibleFoods[Random.Range(0, possibleFoods.Length-1)].Id;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        CurrentState = GameState.Renovation;
        //CurrentState = SceneManager.GetActiveScene().buildIndex switch
        //{
        //    0=>GameState.MainMenu,
        //    1=>GameState.Renovation,
        //    _=>GameState.MainMenu
        //};
    }

    Coroutine currentCustomerSpawner;

    public void StartDay()
    {
        currentCustomerSpawner = StartCoroutine(CustomerSpawner(4, 1, 50));
        CurrentState = GameState.Dining;
    }

    public void GameOver()
    {
        if (CurrentState == GameState.Dining)
        {
            if (currentCustomerSpawner != null)
                StopCoroutine(currentCustomerSpawner);
            //show Game Over UI
        }
    }

    IEnumerator CustomerSpawner(int customerCount, int maxGroupSize, float minnimumDayLength)
    {
        Queue<int> groupQueue = new();
        for (int i = 0; i < customerCount; i++)
        {
            i--;
            int groupSize = Random.Range(1, Mathf.Max(1, maxGroupSize));
            i+= groupSize;
            groupQueue.Enqueue(groupSize);
        }
        float dayLength = Mathf.Max(groupQueue.Count*10, minnimumDayLength);
        float timeBetweenGroups = dayLength / (groupQueue.Count - 1);
        Debug.Log("groupCount " + groupQueue.Count);

        yield return null;

        //start gameplay timer
        while (groupQueue.Count > 0)
        {
            int groupSize = groupQueue.Dequeue();
            SpawnCustomerGroup().SetCustomerCount(groupSize).BeginLogic();
            yield return new WaitForSeconds(timeBetweenGroups);
        }

        //wait until all active customers have despawned
        yield return new WaitUntil(()=> activeGroups.Count==0);
        //wait an extra bit of time
        yield return new WaitForSeconds(3);

        CurrentState = GameState.Renovation;
    }

    public class CustomerGroup
    {
        List<CustomerController> customers = new();
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

        private IEnumerator CustomerLogic()
        {
            if(customersToSpawn==0)
                yield break;

            for (int i = 0; i < customersToSpawn; i++)
            {
                customers.Add(Instance.SpawnCustomer());
            }


            reservedTable = null;
            customers.ForEach(c => c.Agent.stoppingDistance = 1);
            CurrentState = CustomerState.WaitingAtDoor;
            DoorPoint.Instance.AddGroup(this);
            timerCoroutine = Instance.StartCoroutine(CoroutineHelpers.Timer(Instance.FoodPatience, p => DoorPatience = p, Instance.GameOver));
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
                for (int i = 0; i < customers.Count; i++)
                {
                    customers[i].Agent.destination = activeChairs[i].transform.position;
                }
                yield return WaitForPathfinding(CustomerState.WaitingForOrder);
                Debug.Log("Seat");
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
            }

            foreach (var customer in customers)
            {
                yield return new WaitUntil(() => customer.RecievedFood);
            }
            Debug.Log("Eating");

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
            yield return CoroutineHelpers.WaitUntilAll(customers, c => CurrentState != CustomerState.Pathfinding || (c.transform.position - c.Agent.destination).magnitude < 0.25f);
            Debug.Log("Pathfinding complete");
            if (CurrentState == CustomerState.Pathfinding)
                CurrentState = stateOnComplete;
        }
    }
}
