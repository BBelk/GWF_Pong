using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    public GameManager GameManager;
    public int playerInt;
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entered object has a GameObject
        if (other.gameObject != null)
        {
            if(other.gameObject.GetComponent<BallController>() != null){
                GameManager.GoalHit(playerInt, other.gameObject.GetComponent<BallController>().lastHitIndex);
            }
        }
    }
}
