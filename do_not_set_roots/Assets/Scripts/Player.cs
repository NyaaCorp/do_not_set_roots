using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // 2D platform player controller
    
    // Variables
    //movement variables
    public float speed = 5f;
    public float jumpForce = 5f;
    public bool canMove = true;
    public bool canJump = true;
    
    //player states
    public bool isGrounded;
    public bool isJumping;
    public bool isShooting;

    //ground check variables
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    
    //shooting variables
    public bool canShoot = true;
    public float shootTime = 0.5f;
    public float shootTimeCounter;
    public float bulletSpeed = 5f;
    public GameObject bullet;
    
    //rooting variables
    private float rootIncreaseStep = 0.1f;  //root level increase every 0.1s
    public float rootLevel = 0f;
    public int currentRootStage = -1;       //-1 = no root, 0 = root stage 1, 1 = root stage 2...
    private int criticalRootStage = 1;       //root stage that will completely stop player movement, need to break out of root
    public int rootStageAfterLanding = 1;
    //declare array of root stage thresholds with initial values
    public float[] rootStageTimeThresholds = new float[4] {1f, 1.75f, 2.5f, 3.5f};
    public bool canBreakOutOfRoot = true;
    private bool breakingOutOfRoot = false;

    //player components
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sr;
    public Transform groundCheck;
    public Transform firePoint;

    //array of root images
    public GameObject[] rootImages;

    [Header("UI")] 
    public GameObject winPanel;
    public GameObject loosePanel;

    //coroutines
    private Coroutine rootLevelIncreaseCoroutine;
    private Coroutine breakOutOfRootCoroutine;
    private static readonly int IsRunning = Animator.StringToHash("isRunning");

    public void Kill()
    {
        loosePanel.SetActive(true);
        Destroy(gameObject);
    }
    
    public void Retry()
    {
        SceneManager.LoadScene("Level");
    }

    // Start is called before the first frame update
    void Start()
    {
        //initialize root level increase coroutine
        rootLevelIncreaseCoroutine = StartCoroutine(RootLevelIncrease());
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //it can move if it is not shooting and not rooted
        canMove = !isShooting && currentRootStage < criticalRootStage;
        //movement input with arrow keys and spacebar
        if (Input.GetKey(KeyCode.LeftArrow) && canMove)
        {
            if(!isJumping)
                anim.SetBool("isRunning",true);
            sr.flipX = true;
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
        else if (Input.GetKey(KeyCode.RightArrow) && canMove)
        {
            if(!isJumping)
                anim.SetBool("isRunning",true);
            sr.flipX = false;
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
        else
        {
            anim.SetBool("isRunning",false);
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        
        //can jump if it is grounded and not rooted
        canJump = (!isJumping && currentRootStage == -1);
        //jumping input with spacebar
        if (Input.GetKeyDown(KeyCode.Space) && canMove && canJump)
        {
            anim.SetBool("isJumping",true);
            isJumping = true;
            canJump = false;
            rb.velocity = Vector2.up * jumpForce;
        }
        
        if(currentRootStage == criticalRootStage && canBreakOutOfRoot && !breakingOutOfRoot)
        {
            breakingOutOfRoot = true;
            StartCoroutine(BreakOutOfRoot());
        }

    }
    
    //detect if player is grounded
    private void FixedUpdate()
    {
        if (isJumping)
        {
            isGrounded = Physics2D.BoxCast(groundCheck.position, new Vector2(0.5f, 0.5f), 0f, Vector2.down, groundCheckRadius, whatIsGround);
            //isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, whatIsGround);
            if(isGrounded)
                Debug.Log("Grounded!");
            if ( isGrounded && rb.velocity.y <= 0 )
            {
                isJumping = false;
                anim.SetBool("isJumping",false);
                rootLevel = rootStageTimeThresholds[rootStageAfterLanding];
            }
        }   
        isGrounded = false;
    }

    private void LateUpdate()
    {
        var cameraPosition = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, cameraPosition.z);
    }

    //coroutine to increase root level while player is standing still
    private IEnumerator RootLevelIncrease()
    {
        int nextRootStage = 0;
        while (true)
        {
            if (rb.velocity.x == 0 && !isJumping)
            {
                rootLevel += 0.1f;
                if(nextRootStage < rootStageTimeThresholds.Length && rootLevel >= rootStageTimeThresholds[nextRootStage])
                {
                    currentRootStage++;
                    Debug.Log("Root Stage: " + currentRootStage);
                    if (currentRootStage == criticalRootStage)
                    {
                        Debug.Log("Critical root stage reached, need to break out of it to move again!");
                    }
                    if (currentRootStage == rootStageTimeThresholds.Length-1)
                    {
                        //TODO: end game
                        Debug.Log("Game Over!! YOU ROOTED TOO MUCH BOI!!");
                    }
                    nextRootStage++;
                }
                
                
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                rootLevel = 0f;
                currentRootStage = -1;
                nextRootStage = 0;
                yield return new WaitForSeconds(0.1f);
            }
            ShowRootImage();
        }
    }
    
    //coroutine to get alternate input between Q and E 5 times to break out of root
    private IEnumerator BreakOutOfRoot()
    {
        int inputCount = 0;
        bool getLeft = true;
        anim.SetBool("isStuck",true);
        while (inputCount < 5)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Q));
            Debug.Log("You pressed Q!");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            Debug.Log("You pressed E!");
            inputCount++;
        }
        anim.SetBool("isStuck",false);
        currentRootStage = -1;
        rootLevel = 0f;
        breakingOutOfRoot = false;
        //canBreakOutOfRoot = false;  
        Debug.Log("You broke out of root!");
    }
    
    private void ShowRootImage()
    {
        //disable all other root images
        for (int i = 0; i < rootImages.Length; i++)
        {
            rootImages[i].SetActive(false);
        }
        if(currentRootStage >= 0 && currentRootStage < rootImages.Length)
            rootImages[currentRootStage].SetActive(true);
    }
}
