using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController player;
    public static Transform playerTransform;
    public static float playerHealth;
    public static bool rollActive;
    public Animator animator;

    public Camera playerCamera;

    public GameObject playerMiekkaPrefab;
    public GameObject blockMiekkaPrefab;
    public GameObject playerArrowPrefab;

    public static Vector2 mouseScreenPosition;
    public Vector3 startRotation;
    public Vector3 blockStartRotation;
    public Vector2 localPosition;
    public Vector2 blockLocalPosition;
    public Vector2 rollDirection;

    public float threshold;
    public float speed;
    public float rollSpeed;

    public bool playerStrikeCooldown;
    public bool playerShootCooldown;
    public bool blockCooldown;
    public bool rollCooldown;

    public bool isAttackingUp;
    public bool isAttackingDown;
    public bool isAttackingLeft;
    public bool isAttackingRight;
    public bool isDead;

    public static bool playerArrivingFromNorth;
    public static bool playerArrivingFromEast;
    public static bool playerArrivingFromSouth;
    public static bool playerArrivingFromWest;

    //Debug/Boss taito testaus



    void Awake()
    {
        if (player == null)
        {
            DontDestroyOnLoad(gameObject);
            player = this;
        }
        if (player != this)
        {
            Destroy(gameObject);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 150, 30), "Health: " + playerHealth);
        GUI.Label(new Rect(10, 40, 150, 30), "Roll is = " + rollActive);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = 100;
        playerTransform = transform;
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        //Vanhat Kontrollit
        /*
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        transform.Translate(Vector2.up * verticalInput * speed * Time.deltaTime);
        transform.Translate(Vector2.right * horizontalInput * speed * Time.deltaTime);
        */

        /*
         * GetKeyDown = toistaa koodin kerran, painat napin alas
         * GetKey = Toistaa sisällä olevaa koodia, niin kauan aikaa ku pidät nappia pohjassa
         * GetKeyUp = toistaa koodin kerran, kun lasket napista irti ja nappi tulee ylös.
        */

        if (playerHealth <= 0)
        {
            isDead = true;
        }

        if (isDead == false)
        {
            playerControls();
        }

        if (isDead == true)
        {
            float deathAnimationRunLength = 1.1f;

            IEnumerator AnimationRuntime()
            {
                ResetAnimatorBooleanValues();
                animator.SetBool("IsDead", true);
                yield return new WaitForSeconds(deathAnimationRunLength);
                yield return new WaitForSeconds(1);
                //Destroy(gameObject);
            }

            StartCoroutine(AnimationRuntime());
        }

        
    }
    // Strike Mekaniikka
    void PlayerStrike()
    {

        if (playerStrikeCooldown == false)
        {
            IEnumerator PlayerStrikeCooldown()
            {
                //Debug.Log("Started Coroutine at timestamp : " + Time.time);
                playerStrikeCooldown = true;
                yield return new WaitForSeconds(1);
                playerStrikeCooldown = false;
                //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
            }
            Instantiate(playerMiekkaPrefab, transform.position, playerMiekkaPrefab.transform.rotation, transform.parent = transform);
            float attackAnimationRunLength = 0.4f;
            if (isAttackingUp == true)
            {
                IEnumerator AnimationRunTime()
                {
                    animator.SetBool("IsAttackingUp", true);
                    yield return new WaitForSeconds(attackAnimationRunLength);
                    animator.SetBool("IsAttackingUp", false);
                }
                StartCoroutine(AnimationRunTime());
            }

            if (isAttackingDown == true)
            {
                IEnumerator AnimationRunTime()
                {
                    animator.SetBool("IsAttackingDown", true);
                    yield return new WaitForSeconds(attackAnimationRunLength);
                    animator.SetBool("IsAttackingDown", false);
                }
                StartCoroutine(AnimationRunTime());
            }

            if (isAttackingLeft == true)
            {
                IEnumerator AnimationRunTime()
                {
                    animator.SetBool("IsAttackingLeft", true);
                    yield return new WaitForSeconds(attackAnimationRunLength);
                    animator.SetBool("IsAttackingLeft", false);
                }
                StartCoroutine(AnimationRunTime());
            }

            if (isAttackingRight == true)
            {
                IEnumerator AnimationRunTime()
                {
                    animator.SetBool("IsAttackingRight", true);
                    yield return new WaitForSeconds(attackAnimationRunLength);
                    animator.SetBool("IsAttackingRight", false);
                }
                StartCoroutine(AnimationRunTime());
            }
            StartCoroutine(PlayerStrikeCooldown());
        }
    }
    void PlayerShoot()
    {

        if(playerShootCooldown == false)
        {
            IEnumerator PlayerShootCooldown()
            {
                playerShootCooldown = true;
                yield return new WaitForSeconds(1);
                playerShootCooldown = false;
            }
            Vector3 mousePos = Input.mousePosition;
            mouseScreenPosition = playerCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, playerCamera.nearClipPlane));

            float mousePosX = mouseScreenPosition.x;
            float mousePosY = mouseScreenPosition.y;
            float playerPosX = playerTransform.position.x;
            float playerPosY = playerTransform.position.y;

            Vector2 Point_1 = new Vector2(mousePosX, mousePosY);
            Vector2 Point_2 = new Vector2(playerPosX, playerPosY);
            float rotation = Mathf.Atan2(Point_2.y - Point_1.y, Point_2.x - Point_1.x) * Mathf.Rad2Deg;
            //Miksi vitussa tässä pitää rotationiin laittaa +90, että toi kaava toimii, ku PlayerMiekassa sitä ei tarvi laittaa. EnemyArrow ihan sama juttu, ku PlayerArrow, mutta siinä sen sit taas pitää olla -90.  wtf? :D
            Vector3 projectileStartRotation = new Vector3(0f, 0f, rotation + 90);
            Quaternion quaternion = Quaternion.Euler(projectileStartRotation);

            Instantiate(playerArrowPrefab, transform.position, quaternion);
            StartCoroutine(PlayerShootCooldown());
        }
        
    }
    // Block Mekaniikka
    void Block()
    {
        if (blockCooldown == false)
        {
            IEnumerator BlockCooldown()
            {
                blockCooldown = true;
                yield return new WaitForSeconds(3);
                blockCooldown = false;
            }

            Instantiate(blockMiekkaPrefab, transform.position, blockMiekkaPrefab.transform.rotation, transform.parent = transform);
            StartCoroutine(BlockCooldown());
        }
    }
    // Roll Mekaniikka
    void Roll()
    {
        if (rollCooldown == false)
        {

            StartCoroutine(RollCoroutine());
            StartCoroutine(RollCooldown());

            IEnumerator RollCooldown()
            {
                rollCooldown = true;
                yield return new WaitForSeconds(1);
                rollCooldown = false;
            }

            IEnumerator RollCoroutine()
            {
                speed = 0;
                rollActive = true;
                for (float i = 20f; i >= 8f; i /= 1.2f)
                {
                    Debug.Log("RollSpeed is = " + rollSpeed);
                    rollSpeed = i;
                    yield return new WaitForSeconds(.1f);
                }
                rollActive = false;
                speed = 10;
            }
        }
    }
    void ResetAnimatorBooleanValues()
    {
        animator.SetBool("IsMovingUp", false);
        animator.SetBool("IsMovingDown", false);
        animator.SetBool("IsMovingRight", false);
        animator.SetBool("IsMovingLeft", false);
        animator.SetBool("IsNotMovingUp", false);
        animator.SetBool("IsNotMovingDown", false);
        animator.SetBool("IsNotMovingRight", false);
        animator.SetBool("IsNotMovingLeft", false);
    }

    void playerControls()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerStrike();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PlayerShoot();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Block();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Roll();
        }

        //GetKeyDown ei toimi, koska jos liikut vaikka oikealla ja pidät nappia pohjassa ja sitten painat alaspäin, silti pitäen oikealle nappia pohjassa ja lasket irti alaspäin napista, niin alaspäin olevan napin arvot on ne arvot mitkä siihen jää, ei oikealle arvot, vaikka oikealle nappi on pohjassa.
        //Optimoi myöhemmin parempi ratkasu, esim listenerillä, että jos joku noist animaattorin booleista muuttuu, niin silloin päivittää spriten JA ton GetKeyDown koodin. Tän hetkinen GetKeyDown arvot Updatessa on raskas toteuttaa.

        //Ylöspäin
        /*
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            startRotation = new Vector3(0f, 0f, 45f);
            localPosition = new Vector2(-1, 1);
            blockLocalPosition = new Vector2(0.75f, 1.25f);
            blockStartRotation = new Vector3(0f, 0f, 60f);
            rollDirection = Vector2.up;
            threshold = -0.38f;
        }
        */
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) && rollActive == false)
        {
            isAttackingUp = true;
            isAttackingDown = false;
            isAttackingLeft = false;
            isAttackingRight = false;

            startRotation = new Vector3(0f, 0f, 45f);
            localPosition = new Vector2(-1, 1);
            blockLocalPosition = new Vector2(0.75f, 1.25f);
            blockStartRotation = new Vector3(0f, 0f, 60f);
            rollDirection = Vector2.up;
            threshold = -0.38f;
            ResetAnimatorBooleanValues();
            animator.SetBool("IsMovingUp", true);
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
        {
            ResetAnimatorBooleanValues();
            animator.SetBool("IsNotMovingUp", true);
        }

        //Alaspäin
        /*
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            startRotation = new Vector3(0f, 0f, 225f);
            localPosition = new Vector2(1, -1);
            blockLocalPosition = new Vector2(-0.75f, -1.25f);
            blockStartRotation = new Vector3(0f, 0f, 240f);
            rollDirection = Vector2.down;
            threshold = 0.92f;
        }
        */
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) && rollActive == false)
        {
            isAttackingUp = false;
            isAttackingDown = true;
            isAttackingLeft = false;
            isAttackingRight = false;

            startRotation = new Vector3(0f, 0f, 225f);
            localPosition = new Vector2(1, -1);
            blockLocalPosition = new Vector2(-0.75f, -1.25f);
            blockStartRotation = new Vector3(0f, 0f, 240f);
            rollDirection = Vector2.down;
            threshold = 0.92f;
            ResetAnimatorBooleanValues();
            animator.SetBool("IsMovingDown", true);
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            ResetAnimatorBooleanValues();
            animator.SetBool("IsNotMovingDown", true);
        }

        //Vasemmalle
        /*
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            startRotation = new Vector3(0f, 0f, 135f);
            localPosition = new Vector2(-1, -1);
            blockLocalPosition = new Vector2(-1, 0.5f);
            blockStartRotation = new Vector3(0f, 0f, 150f);
            rollDirection = Vector2.left;
            threshold = 0.38f;
        }
        */
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) && rollActive == false)
        {
            isAttackingUp = false;
            isAttackingDown = false;
            isAttackingLeft = true;
            isAttackingRight = false;

            startRotation = new Vector3(0f, 0f, 135f);
            localPosition = new Vector2(-1, -1);
            blockLocalPosition = new Vector2(-1, 0.5f);
            blockStartRotation = new Vector3(0f, 0f, 150f);
            rollDirection = Vector2.left;
            threshold = 0.38f;
            ResetAnimatorBooleanValues();
            animator.SetBool("IsMovingLeft", true);
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
        {
            ResetAnimatorBooleanValues();
            animator.SetBool("IsNotMovingLeft", true);
        }

        //Oikealle
        /*
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            startRotation = new Vector3(0f, 0f, -45f);
            localPosition = new Vector2(1, 1);
            blockLocalPosition = new Vector2(1, -0.5f);
            blockStartRotation = new Vector3(0f, 0f, -30f);
            rollDirection = Vector2.right;
            threshold = -0.92f;
        }
        */
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) && rollActive == false)
        {
            isAttackingUp = false;
            isAttackingDown = false;
            isAttackingLeft = false;
            isAttackingRight = true;

            startRotation = new Vector3(0f, 0f, -45f);
            localPosition = new Vector2(1, 1);
            blockLocalPosition = new Vector2(1, -0.5f);
            blockStartRotation = new Vector3(0f, 0f, -30f);
            rollDirection = Vector2.right;
            threshold = -0.92f;
            ResetAnimatorBooleanValues();
            animator.SetBool("IsMovingRight", true);
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
        {
            ResetAnimatorBooleanValues();
            animator.SetBool("IsNotMovingRight", true);
        }

        if (rollActive == true)
        {
            transform.Translate(rollDirection * rollSpeed * Time.deltaTime);
        }
    }
}