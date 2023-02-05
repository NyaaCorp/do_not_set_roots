using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool changer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (changer)
            {
                var animator = other.GetComponent<Animator>();
                animator.SetLayerWeight(animator.GetLayerIndex("Old_Layer"), 1);
            }
            else
            {
                other.GetComponent<Player>().Win(); 
            }
        }
    }
}
