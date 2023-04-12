using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnRenovationState : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameStateManager.Instance.OnStateChange += DestroyOnRenovation;
    }

    private void DestroyOnRenovation(GameStateManager.GameState s)
    {
        if (s == GameStateManager.GameState.Renovation && this)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance)
            GameStateManager.Instance.OnStateChange -= DestroyOnRenovation;
    }
}
