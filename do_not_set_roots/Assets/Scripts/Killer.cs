using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killer : MonoBehaviour
{
    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<Player>().Kill();
    }
}
