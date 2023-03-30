using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CustomerGroup = GameStateManager.CustomerGroup;

public class DoorPoint : MonoBehaviour
{
    static DoorPoint instance;
    //create getter
    public static DoorPoint Instance { get { return instance; } }
    List<CustomerGroup> groupList = new();//creating new queue for cutsomers lined up at door

    List<TableAppliance> tables = new();

    [SerializeField]
    PopupController popupController;

    private void Start()
    {
        instance = this;//there is only one door point, and we can get it from anywhere using Instance
    }

    private void OnEnable()
    {
        GameStateManager.Instance.OnStateChange += OnGameStateChange;
    }

    private void OnGameStateChange(GameStateManager.GameState obj)
    {
        if (obj == GameStateManager.GameState.Dining)
        {
            tables.Clear();
            //only tables that are slotted in an appliance slot (should be all spawned tables) will be linked
            tables.AddRange(FindObjectsOfType<TableAppliance>().Where(t=>t.ApplianceCore.IsSlotted));
        }
    }

    private void OnDisable()
    {
        GameStateManager.Instance.OnStateChange -= OnGameStateChange;
    }

    private void Update()
    {
        if (groupList.Count != 0)
        {
            popupController.SetActive(true);
            popupController.SetPercent(groupList.First().DoorPatience);
        }
        else
        {
            popupController.SetActive(false);
        }
    }

    public Transform GetPersistantTarget(CustomerGroup group)
    {
        int index = groupList.IndexOf(group);
        if (index == 0)
            return transform;
        else
            return groupList[index - 1].LastCustomerTransform();
    }

    public void AddGroup(CustomerGroup group)
    {
        groupList.Add(group);
        foreach (var table in tables)//Item is the current table were looking at. Foreach will look through all tables in "table"
        {
            if (table.TryReserveCustomer())
                break;
        }
        group.UpdateDoorPathfinding();
    }

    public CustomerGroup GetGroup(Func<CustomerGroup, bool> predicate) => groupList.FirstOrDefault(predicate);

    public void RemoveGroup(CustomerGroup toRemove)
    {
        if (!groupList.Contains(toRemove))
            return;
        groupList.Remove(toRemove);

        groupList.ForEach(g=>g.UpdateDoorPathfinding());
    }

}
