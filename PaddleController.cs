using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public GameManager GameManager;
    public int playerIndex;
    public float speed = 5f;
    public GameObject paddleToColor;
    public float paddleBound;

    private Vector2 touchStartPosition;
    private bool isMoving = false;
    public float sensitivity;

    void Start()
    {
        paddleBound = GameManager.paddleBound;
    }

    void Update()
    {
        if(!canMove){return;}

         if(playerIndex == 0){transform.Translate(0f, 0f, Input.GetAxis ("Horizontal") * speed * Time.deltaTime);}

        if(playerIndex == 1){transform.Translate(0f, 0f, Input.GetAxis ("Horizontal2") * speed * Time.deltaTime);}

        if(this.transform.localPosition.x > paddleBound){this.transform.localPosition = new Vector3(paddleBound, this.transform.localPosition.y, this.transform.localPosition.z);}

        if(this.transform.localPosition.x < paddleBound * -1f){this.transform.localPosition = new Vector3(paddleBound * -1f, this.transform.localPosition.y, this.transform.localPosition.z);}

        // Check for touch input
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            // Check if this touch is for the current player
            if ((playerIndex == 0 && touch.position.y < Screen.height * 0.5f) ||
                (playerIndex == 1 && touch.position.y >= Screen.height * 0.5f))
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                    GameManager.LogToFile($"Player #{playerIndex + 1} touched screen {touch.position}");
                        touchStartPosition = touch.position;
                        isMoving = true;
                        break;

                    case TouchPhase.Moved:
                        if (isMoving)
                        {
                            float deltaX = (touch.position.x - touchStartPosition.x);
                            float newX = transform.localPosition.x + deltaX * sensitivity * Time.deltaTime;
                            newX = Mathf.Clamp(newX, -paddleBound, paddleBound);
                            transform.localPosition = new Vector3(newX, transform.localPosition.y, transform.localPosition.z);
                            touchStartPosition = touch.position;
                        }
                        break;

                    case TouchPhase.Ended:
                    UnityEngine.Debug.Log("TOUCH END");
                        isMoving = false;
                        break;
                }
            }
        }
    }

    public void SetPaddleTexture(Texture newTexture)
    {
        paddleToColor.GetComponent<Renderer>().material.mainTexture = newTexture;
    }

    public void ResetPaddle()
    {
        transform.localPosition = new Vector3(0f, transform.localPosition.y, transform.localPosition.z);
    }

    public bool canMove;
    public void StopMovement()
    {
        canMove = false;
    }

    public void StartMovement()
    {
        canMove = true;
    }
}
