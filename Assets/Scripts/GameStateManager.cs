using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameStateManager : Singleton<GameStateManager>
{
    [SerializeField] private PlayerController playerController;
    private int _currentPriority = 0;

    public void LockPlayer(int priority = 0)
    {
        if (priority >= _currentPriority)
        {
            _currentPriority = priority;
            playerController.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log($"LockPlayer (Priority: {priority}) - PlayerController.enabled: {playerController.enabled}");
        }
    }

    public void UnlockPlayer(int priority = 0)
    {
        if (priority >= _currentPriority)
        {
            _currentPriority = 0;
            playerController.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log($"UnlockPlayer - PlayerController.enabled: {playerController.enabled}");
        }
    }

    public bool IsPlayerLocked() => !playerController.enabled;

}
