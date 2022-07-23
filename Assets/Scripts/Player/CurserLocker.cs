using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurserLocker : MonoBehaviour
{
    [SerializeField] private bool CursorLockMode;
    public static bool isCursorLocked;

    void Awake()
    {
        isCursorLocked = CursorLockMode;
    }
}
