using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AchievementSystem;
using UnityEngine.InputSystem;

public class TTTTTTTestjiaoben : MonoBehaviour
{
    [SerializeField] private Achievement achievement;

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AchievementManager.Enable(achievement);
        };
    }
}
