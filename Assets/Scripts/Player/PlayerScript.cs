using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {
    //major components
    private CharacterController controller;
    private Animator animator;
    private Transform cam;
    private GameManager gm;
    private ObjectiveChanger objectiveChanger;
    private Transform playerTrans;
    private LayerMask groundMask;
    private BoxCollider colliderLegs;
    

    //story elements
    private Transform grill;
    private Transform lb;
    private Transform football;
    private Transform g7;
    private Transform g9;
    private Transform PlayerPosC1;
    private Animator transition;
    private bool kickedBall = false;
    private Interaction interaction;
    private bool check1 = false;
    private bool check2 = false;
    private bool check3 = false;
    private Interaction teddy;


    //singleton
    //private static PlayerScript instance = null;

    //inventory
    private Inventory inventory;
    private bool openInventory = false;
    public Item waterGun;
    public Item raceCar;
    public Item racket;
    public Item footballItem;
    public Item littleTeddy;

    public Enemy neighbor;

    //movement
    Vector3 velocity;
    private bool isGrounded;
    private bool jumpNotReady;
    private float jumpHeight = .75f;
    private float gravity = -9.81f;
    private float speed = 2f;
    private float walkSpeed = 2f;
    private float runSpeed = 4f;
    private float SmoothTurn = 0.1f;
    private float TurnSmoothVelocity;
    private bool crouchingToggle = false;
    public bool isStealth;
    public bool isInCombat;
    public bool IsImmobile;

    //interact
    private Interaction ClosestTarget;
    private GameObject InteractText;
    private GameObject textBubble;
    private List<GameObject> ListOfInteractables;
    private bool inDialog;
    private GameObject keybinds;

    //HP System
    public int MaxHp = 8;
    public int HP;
    public bool invincibility;
    private GameObject[] FullHearts = new GameObject[4];
    private GameObject[] HalfHearts = new GameObject[4];
    private GameObject[] EmptyHearts = new GameObject[4];
    private GameObject H1E;
    private GameObject H2E;
    private GameObject H3E;
    private GameObject H4E;
    private GameObject H1H;
    private GameObject H2H;
    private GameObject H3H;
    private GameObject H4H;
    private GameObject H1F;
    private GameObject H2F;
    private GameObject H3F;
    private GameObject H4F;
    private GameObject HEE;
    private GameObject HEH;
    private GameObject HEF;
    private GameObject ghearts;



    //stamina System
    public float Stamina;
    private float maxStamina;
    private bool isStaminaBuff;
    private float staminaRate = 1f;  //rate at which stamina is depleated
    private float staminabuffTimer = 60f;
    private float recoveryTimer = 1f;
    private bool animationHasStarted = false;
    private bool animationAttStarted1 = false;
    private bool animationAttStarted2 = false;
    private bool animationAttStarted3 = false;
    private Slider staminaSlider;

    //combat system
    public int attackDamage;
    public int bonusAttackDamage = 0;
    public int buffAttackDamage = 0;
    private bool isEnegyBuff;
    private float energyBuffTimer = 60f;
    private Transform attackpoint;
    private float attackRange = 0.3f;
    private LayerMask enemyLayers;
    private LayerMask bushLayer;
    private Weapon equipedWeapon;
    private bool hit1 = true;
    private bool hit2 = true;
    private bool hit3 = true;
    private bool hit4 = true;

    //spawnpoint
    private bool isDead;
    private float respawnTimer;
    private Respawn respawner;
    public string spawnPointName;
    private Transform spawnPoint;

    //input booleans
    private bool forwardPressed;
    private bool runPressed;
    private bool backwardPressed;
    private bool leftPressed;
    private bool rightPressed;
    private bool jumpPressed;
    private bool attackPressed;
    private bool crouchPressed;
    private bool rollPressed;
    private bool inventoryPressed;

    //bools to see what animation is currently playing
    private bool idleAnimation;
    private bool walkAnimation;
    private bool runningAnimation;
    public bool crouchWalkAnimation;
    private bool attackAnimation1;
    private bool attackAnimation2;
    private bool attackAnimation3;
    private bool attackAnimation4;
    private bool deathAnimation;
    private bool hitAnimation;
    private bool rollAnimation;
    private bool jumpAnimation;


    //hash codes for  the animator 
    private int isWalkingHash;
    private int isRunningHash;
    private int isJumpingHash;
    private int isAttack1Hash;
    private int isAttack2Hash;
    private int isAttack3Hash;
    private int isCrouchingHash;
    private int isHitHash;
    private int isDeadHash;
    private int isRollingHash;

    public AudioManager audioManager;

    /*private void Awake() {
        Singleton();
    }*/

    private void Start() {
        initiate();

    }
    private void FixedUpdate() {
        groundCheck();
    }
    void Update() {
        movement();
        checkCurrentAnimationPlaying();
        animatePlayer();
        inputManager();

        updateHealth();
        updateDamage();
        RestartGame();
        Die();

        updateStamina();
        recoverStamina(3);
        setStaminaSlider(Stamina);
        StartCoroutine(Interact());

        ToggleKeybinds();

        attack();
        updateWeapon();

        toggleInventory();
        BuffPlayer();

        StartCoroutine(StoryChanger());

        SoundManager();

        ToggleInvincibility();

    }

    /*//Singleton pattern
    private void Singleton() {
        if (instance == null) {
            instance = this;
        } else if (instance != null) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }*/



    //for initialization 
    private void initiate() {
        Cursor.lockState = CursorLockMode.Locked;

        inventory = GetComponent<Inventory>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cam = GameObject.Find("Main Camera").GetComponent<Transform>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        objectiveChanger = GameObject.Find("GameManager").GetComponent<ObjectiveChanger>();
        audioManager = GameObject.Find("GameManager").GetComponent<AudioManager>();
        InteractText = GameObject.Find("InteractText");
        textBubble = GameObject.Find("TextBubble");
        staminaSlider = GameObject.Find("StaminaBar").GetComponent<Slider>();
        attackpoint = GameObject.Find("AttackPoint").GetComponent<Transform>();
        grill = GameObject.Find("grill").GetComponent<Transform>();
        playerTrans = GetComponent<Transform>();
        lb = GameObject.Find("LittleBro").GetComponent<Transform>();
        football = GameObject.Find("Football").GetComponent<Transform>();
        g7 = GameObject.Find("Goal7").GetComponent<Transform>();
        transition = GameObject.Find("Crossfade").GetComponent<Animator>();
        PlayerPosC1 = GameObject.Find("Goal8").GetComponent<Transform>();
        g9 = GameObject.Find("Goal9").GetComponent<Transform>();
        interaction = GameObject.Find("monolog").GetComponent<Interaction>();
        respawner = GameObject.Find("GameManager").GetComponent<Respawn>();
        keybinds = GameObject.Find("KeybindsImg");
        teddy = GameObject.Find("teddyInBag_interact").GetComponent<Interaction>();
        colliderLegs = GameObject.Find("mixamorig:Hips").GetComponent<BoxCollider>();
        ghearts = GameObject.Find("InvincibilityHearts");

        waterGun = GameObject.Find("waterGun").GetComponent<Item>();
        raceCar = GameObject.Find("ToyRacecar").GetComponent<Item>();
        racket = GameObject.Find("BadmintonRacket").GetComponent<Item>();
        footballItem = GameObject.Find("Football(Item)").GetComponent<Item>();
        littleTeddy = GameObject.Find("littleTeddy").GetComponent<Item>();

        neighbor = GameObject.Find("Neighbor").GetComponent<Enemy>();

        ListOfInteractables = new List<GameObject>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        isAttack1Hash = Animator.StringToHash("isAttack1");
        isAttack2Hash = Animator.StringToHash("isAttack2");
        isAttack3Hash = Animator.StringToHash("isAttack3");
        isHitHash = Animator.StringToHash("isHit");
        isDeadHash = Animator.StringToHash("isDead");
        isCrouchingHash = Animator.StringToHash("isCrouching");
        isRollingHash = Animator.StringToHash("isRolling");
        gameObject.layer = LayerMask.NameToLayer("Player");
        enemyLayers = LayerMask.GetMask("Enemy");
        bushLayer = LayerMask.GetMask("Bush");
        groundMask = LayerMask.GetMask("Ground");

        FillHeartsArrays();
        setAttackDamage(1);
        setMaxStamina(10);
        setStamina(maxStamina);
        HP = MaxHp;

        setSpawnPointName("L0S1");
        setSpawnPoint(GameObject.Find(getSpawnPointName()).GetComponent<Transform>());

        keybinds.SetActive(false);
    }
    /*
     void Spawnpoint() {
         spawnpoint = GameObject.Find(CurrentSpawnpoint).GetComponent<Transform>();
         transform.position = new Vector3(spawnpoint.position.x, spawnpoint.position.y, spawnpoint.position.z);
         transform.transform.Rotate(spawnpoint.eulerAngles.x, spawnpoint.eulerAngles.y, spawnpoint.eulerAngles.z);
     }
    */

    //checks the current input of the player
    private void ToggleInvincibility() {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            if (Input.GetKeyDown(KeyCode.U)) {
                audioManager.Play("InteractSound", false, 0.1f, 1);
                invincibility = !invincibility;
            }
        }
        if (invincibility) {
            ghearts.SetActive(true);
            for(int i = 0; i< FullHearts.Length; i++) {
                FullHearts[i].GetComponent<Image>().enabled = false;
                HalfHearts[i].GetComponent<Image>().enabled = false;
                EmptyHearts[i].GetComponent<Image>().enabled = false;
            }
        } else {
            ghearts.SetActive(false);
            for (int i = 0; i < FullHearts.Length; i++) {
                FullHearts[i].GetComponent<Image>().enabled = true;
                HalfHearts[i].GetComponent<Image>().enabled = true;
                EmptyHearts[i].GetComponent<Image>().enabled = true;
            }
        }
    }
    private void inputManager() {
        if (!IsImmobile) {
            forwardPressed = Input.GetKey(KeyCode.W);
            runPressed = Input.GetKey(KeyCode.LeftShift);
            backwardPressed = Input.GetKey(KeyCode.S);
            leftPressed = Input.GetKey(KeyCode.A);
            rightPressed = Input.GetKey(KeyCode.D);
            jumpPressed = Input.GetKeyDown(KeyCode.Space);
            attackPressed = Input.GetMouseButtonDown(0);
            crouchPressed = Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl);
            rollPressed = Input.GetMouseButtonDown(1);
            inventoryPressed = Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.B);
        }
    }

    //checks which animation is running 
    private void checkCurrentAnimationPlaying() {
        idleAnimation = animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle");
        walkAnimation = animator.GetCurrentAnimatorStateInfo(0).IsTag("Walk");
        runningAnimation = animator.GetCurrentAnimatorStateInfo(0).IsTag("Run");
        crouchWalkAnimation = animator.GetCurrentAnimatorStateInfo(0).IsTag("CrouchWalk");
        deathAnimation = animator.GetCurrentAnimatorStateInfo(0).IsTag("Death");
        hitAnimation = animator.GetCurrentAnimatorStateInfo(0).IsTag("Hit");

        attackAnimation1 = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack1");
        attackAnimation2 = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack2");
        attackAnimation3 = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack3");
        attackAnimation4 = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack4");
        rollAnimation = animator.GetCurrentAnimatorStateInfo(0).IsTag("Rolling");
        jumpAnimation = animator.GetCurrentAnimatorStateInfo(0).IsTag("Jump");
    }

    //ability for player to move around in the world
    private void movement() {
        //Gets Axis from Input Manager
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        //prevents from using y-axis (Normalized so you wont move faster when pressing W && D/A)
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //player can't move while hit or death animation is playing
        if (!hitAnimation && !deathAnimation && !inDialog && !IsImmobile) {
            //move
            if (direction.magnitude >= 0.1f) {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVelocity, SmoothTurn);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                //move a little faster while rolling
                if (rollAnimation) {

                    controller.Move(moveDir.normalized * speed * 1.5f * Time.deltaTime);
                }
                //move slower when attacking
                else if (attackAnimation1 || attackAnimation2 || attackAnimation3 || attackAnimation4) {
                    controller.Move(moveDir.normalized * speed * 0.3f * Time.deltaTime);
                }
                //normal move
                else {
                    controller.Move(moveDir.normalized * speed * Time.deltaTime);
                }
            }

            // jump
            if (Input.GetButtonDown("Jump") && isGrounded && !rollAnimation && getStamina() > 0 && !openInventory && !attackAnimation1 && !attackAnimation2 && !attackAnimation3 && !attackAnimation4) {
                StartCoroutine("Jump");
            }
        }

        //gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }
    }

    //method for checking if the player is grounded
    void groundCheck() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.07f + 0.02f)) {
            isGrounded = true;
        } else {
            isGrounded = false;
        }
    }

    //jump method
    IEnumerator Jump() {
        animator.SetBool(isJumpingHash, true);
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        yield return new WaitForSeconds(0.2f);
        animator.SetBool(isJumpingHash, false);
        setCrouchToggle(false);
    }

    private void animatePlayer() {
        if (inDialog) {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isRunningHash, false);
            animator.SetBool(isCrouchingHash, false);
        }
        if (!inDialog) {
            //walking animation
            if ((forwardPressed || backwardPressed || leftPressed || rightPressed)) {
                animator.SetBool(isWalkingHash, true);
            } else {
                animator.SetBool(isWalkingHash, false);
            }

            //running animation
            if ((forwardPressed || backwardPressed || leftPressed || rightPressed) && runPressed && getStamina() > 0) {
                animator.SetBool(isRunningHash, true);
                setCrouchToggle(false);
                speed = runSpeed;
            } else {
                animator.SetBool(isRunningHash, false);
                speed = walkSpeed;
            }

            //crouching animation
            if (crouchPressed && !openInventory) {
                crouchingToggle = !crouchingToggle;
            }

            if (crouchingToggle) {
                animator.SetBool(isCrouchingHash, true);

                //Corrects hitbox and speed
                controller.height = 0.1f;
                controller.center = new Vector3(0f, 0.05f, 0f);
                speed = 1;
            } else if (!crouchingToggle) {
                animator.SetBool(isCrouchingHash, false);

                //Corrects hitbox and speed
                controller.height = 0.14f;
                controller.center = new Vector3(0f, 0.075f, 0f);
            }


            //attack animation
            //attack 1
            if (attackPressed && !attackAnimation1 && !attackAnimation2 && !attackAnimation3 && Stamina > 0 && !openInventory) {
                animator.SetBool(isAttack1Hash, true);
                crouchingToggle = false;
            }


            //attack 2
            if (attackPressed && attackAnimation1 && Stamina > 0 && !openInventory) {
                animator.SetBool(isAttack2Hash, true);
            }



            //attack 3
            if (attackPressed && attackAnimation2 && Stamina > 0 && !openInventory) {
                animator.SetBool(isAttack3Hash, true);
            }


            if (!attackPressed && attackAnimation1) {
                animator.SetBool(isAttack1Hash, false);

            }
            if (!attackPressed && attackAnimation2) {
                animator.SetBool(isAttack2Hash, false);

            }
            if (!attackPressed && attackAnimation3) {
                animator.SetBool(isAttack3Hash, false);

            }

            //jump attack animation
            if (attackAnimation4 && Stamina > 0) {
                animator.SetBool(isAttack1Hash, false);
            }

            //die animation
            if (HP <= 0 && !hitAnimation) {
                animator.SetBool(isDeadHash, true);
                isDead = true;
                if (deathAnimation) {
                    animator.SetBool(isDeadHash, false);
                }
            }

            //roll animation
            if (rollPressed && Stamina > 0 && !rollAnimation && !openInventory) {
                animator.SetBool(isRollingHash, true);
                setCrouchToggle(false);
            }
            else {
                animator.SetBool(isRollingHash, false);
            }

            //hit animation
            if (hitAnimation) {
                animator.SetBool(isHitHash, false);
            }
        }
    }


    private void attack() {
        Collider[] hitEnemies = Physics.OverlapSphere(attackpoint.position, attackRange, enemyLayers);

        if (attackAnimation1 && hit1) {
            foreach (Collider enemy in hitEnemies) {
                hit1 = false;
                enemy.GetComponent<Enemy>().takeDamage(attackDamage + bonusAttackDamage + buffAttackDamage);

            }
        }
        if (attackAnimation2 && hit2) {
            foreach (Collider enemy in hitEnemies) {
                hit2 = false;
                enemy.GetComponent<Enemy>().takeDamage(attackDamage + bonusAttackDamage + buffAttackDamage);
            }
        }
        if (attackAnimation3 && hit3) {
            foreach (Collider enemy in hitEnemies) {
                hit3 = false;
                enemy.GetComponent<Enemy>().takeDamage(attackDamage + bonusAttackDamage + buffAttackDamage + 1);
            }
        }
        if (attackAnimation4 && hit4) {
            foreach (Collider enemy in hitEnemies) {
                hit4 = false;
                enemy.GetComponent<Enemy>().takeDamage(attackDamage + bonusAttackDamage + buffAttackDamage + 1);
            }
        }
        if (!attackAnimation1 && !attackAnimation2 && !attackAnimation3 && !attackAnimation4 && !hitAnimation) {
            hit1 = true;
            hit2 = true;
            hit3 = true;
            hit4 = true;
        }


        //hitting bush
        Collider[] hitBush = Physics.OverlapSphere(attackpoint.position, attackRange, bushLayer);
        if (attackAnimation1 && hit1 && equipedWeapon != null) {
            if (equipedWeapon.name == "woodenSword_weapon") {
                foreach (Collider bush in hitBush) {
                    hit1 = false;
                    bush.GetComponent<GreatBush>().takeDamage();
                }
            }
        }
        if (attackAnimation2 && hit2 && equipedWeapon != null) {
            if (equipedWeapon.name == "woodenSword_weapon") {
                foreach (Collider bush in hitBush) {
                    hit2 = false;
                    bush.GetComponent<GreatBush>().takeDamage();
                }
            }
        }

        if (attackAnimation3 && hit3 && equipedWeapon != null) {
            if (equipedWeapon.name == "woodenSword_weapon") {
                foreach (Collider bush in hitBush) {
                    hit3 = false;
                    bush.GetComponent<GreatBush>().takeDamage();
                }
            }
        }
        if (attackAnimation4 && hit4 && equipedWeapon != null) {
            if (equipedWeapon.name == "woodenSword_weapon") {
                foreach (Collider bush in hitBush) {
                    hit4 = false;
                    bush.GetComponent<GreatBush>().takeDamage();
                }
            }
        }
        if (!attackAnimation1 && !attackAnimation2 && !attackAnimation3 && !attackAnimation4 && !hitAnimation) {
            hit1 = true;
            hit2 = true;
            hit3 = true;
            hit4 = true;
        }
    }

    private void OnDrawGizmosSelected() {
        if (attackpoint == null) {
            return;
        }
        Gizmos.DrawWireSphere(attackpoint.position, attackRange);
    }



    private void updateWeapon() {
        if (equipedWeapon == null) {
            setBonusAttackDamage(0);
        }
        else if (equipedWeapon != null) {
            setBonusAttackDamage(equipedWeapon.getDamage());
        }
    }




    public void takeDamage(int damage) {
        if (!rollAnimation) {
            if (!deathAnimation) {
                animator.SetBool(isHitHash, true);
            }
            if (HP > 0) {
                HP -= damage;
            }
        }
    }



    void RestartGame() {
        if (Input.GetKey(KeyCode.R) && !isDead) {
            respawnTimer += Time.deltaTime;
            if (respawnTimer > 2f) {
                respawner.respawnPlayer();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Spikes") {
            respawner.respawnPlayer();
        }
        if (other.tag == "StoryChanger") {
            switch (gm.StoryNumber) {
                case float sn when gm.StoryNumber <= 0.09f:
                    gm.StoryNumber = 1.00f;
                    teddy.setStartInteraction(true);
                    Destroy(other.gameObject);
                    break;
                case 1.00f:
                    gm.StoryNumber = 1.01f;
                    teddy.setStartInteraction(true);
                    Destroy(other.gameObject);
                    break;
                case 1.01f:
                    gm.StoryNumber = 1.02f;
                    teddy.setStartInteraction(true);
                    Destroy(other.gameObject);
                    break;
            }

        }
    }

    private bool deathSound;

    public void Die() {
        if (HP <= 0) {
            if (!deathSound) {
                audioManager.Play("PlayerDeathSound", false, 0.1f, 1);
                deathSound = true;
            }
            respawner.respawnPlayer();
        }

        if (HP == 8) {
            deathSound = false;
        }
    }

    public void heal(int health) {
        HP += health;
        if (HP >= MaxHp) {
            HP = MaxHp;
        }
    }


    public void StaminaBuff() {
        isStaminaBuff = true;
    }

    public void EnergyBuff() {
        isEnegyBuff = true;
    }

    private void updateDamage() {
        if (!isEnegyBuff) {
            buffAttackDamage = 0;
        }
        else if (isEnegyBuff) {
            buffAttackDamage = 2;
            if (energyBuffTimer > 0 && isEnegyBuff) {
                energyBuffTimer -= Time.deltaTime;
                if (energyBuffTimer <= 0) {
                    isEnegyBuff = false;
                    energyBuffTimer = 60f;
                }
            }
        }
    }

    
    private void updateHealth() {
        //First
        if (HP == 8) {
            FullHearts[3].gameObject.SetActive(true);
            FullHearts[2].gameObject.SetActive(true);
            FullHearts[1].gameObject.SetActive(true);
            FullHearts[0].gameObject.SetActive(true);

            HalfHearts[3].gameObject.SetActive(false);
            HalfHearts[2].gameObject.SetActive(false);
            HalfHearts[1].gameObject.SetActive(false);
            HalfHearts[0].gameObject.SetActive(false);
        }
        if (HP == 7) {
            FullHearts[3].gameObject.SetActive(false);
            FullHearts[2].gameObject.SetActive(true);
            FullHearts[1].gameObject.SetActive(true);
            FullHearts[0].gameObject.SetActive(true);

            HalfHearts[3].gameObject.SetActive(true);
            HalfHearts[2].gameObject.SetActive(false);
            HalfHearts[1].gameObject.SetActive(false);
            HalfHearts[0].gameObject.SetActive(false);
        }
        if (HP == 6) {
            FullHearts[3].gameObject.SetActive(false);
            FullHearts[2].gameObject.SetActive(true);
            FullHearts[1].gameObject.SetActive(true);
            FullHearts[0].gameObject.SetActive(true);

            HalfHearts[3].gameObject.SetActive(false);
            HalfHearts[2].gameObject.SetActive(false);
            HalfHearts[1].gameObject.SetActive(false);
            HalfHearts[0].gameObject.SetActive(false);
        }
        if (HP == 5) {
            FullHearts[3].gameObject.SetActive(false);
            FullHearts[2].gameObject.SetActive(false);
            FullHearts[1].gameObject.SetActive(true);
            FullHearts[0].gameObject.SetActive(true);

            HalfHearts[3].gameObject.SetActive(false);
            HalfHearts[2].gameObject.SetActive(true);
            HalfHearts[1].gameObject.SetActive(false);
            HalfHearts[0].gameObject.SetActive(false);
        }
        if (HP == 4) {
            FullHearts[3].gameObject.SetActive(false);
            FullHearts[2].gameObject.SetActive(false);
            FullHearts[1].gameObject.SetActive(true);
            FullHearts[0].gameObject.SetActive(true);

            HalfHearts[3].gameObject.SetActive(false);
            HalfHearts[2].gameObject.SetActive(false);
            HalfHearts[1].gameObject.SetActive(false);
            HalfHearts[0].gameObject.SetActive(false);
        }
        if (HP == 3) {
            FullHearts[3].gameObject.SetActive(false);
            FullHearts[2].gameObject.SetActive(false);
            FullHearts[1].gameObject.SetActive(false);
            FullHearts[0].gameObject.SetActive(true);

            HalfHearts[3].gameObject.SetActive(false);
            HalfHearts[2].gameObject.SetActive(false);
            HalfHearts[1].gameObject.SetActive(true);
            HalfHearts[0].gameObject.SetActive(false);
        }
        if (HP == 2) {
            FullHearts[3].gameObject.SetActive(false);
            FullHearts[2].gameObject.SetActive(false);
            FullHearts[1].gameObject.SetActive(false);
            FullHearts[0].gameObject.SetActive(true);

            HalfHearts[3].gameObject.SetActive(false);
            HalfHearts[2].gameObject.SetActive(false);
            HalfHearts[1].gameObject.SetActive(false);
            HalfHearts[0].gameObject.SetActive(false);
        }
        if (HP == 1) {
            FullHearts[3].gameObject.SetActive(false);
            FullHearts[2].gameObject.SetActive(false);
            FullHearts[1].gameObject.SetActive(false);
            FullHearts[0].gameObject.SetActive(false);

            HalfHearts[3].gameObject.SetActive(false);
            HalfHearts[2].gameObject.SetActive(false);
            HalfHearts[1].gameObject.SetActive(false);
            HalfHearts[0].gameObject.SetActive(true);
        }
        if (HP <= 0) {
            FullHearts[3].gameObject.SetActive(false);
            FullHearts[2].gameObject.SetActive(false);
            FullHearts[1].gameObject.SetActive(false);
            FullHearts[0].gameObject.SetActive(false);

            HalfHearts[3].gameObject.SetActive(false);
            HalfHearts[2].gameObject.SetActive(false);
            HalfHearts[1].gameObject.SetActive(false);
            HalfHearts[0].gameObject.SetActive(false);
        }
    }



    private void FillHeartsArrays() {
        H1E = GameObject.Find("H1 empty");
        H2E = GameObject.Find("H2 empty");
        H3E = GameObject.Find("H3 empty");
        H4E = GameObject.Find("H4 empty");
        H1H = GameObject.Find("H1 half");
        H2H = GameObject.Find("H2 half");
        H3H = GameObject.Find("H3 half");
        H4H = GameObject.Find("H4 half");
        H1F = GameObject.Find("H1 full");
        H2F = GameObject.Find("H2 full");
        H3F = GameObject.Find("H3 full");
        H4F = GameObject.Find("H4 full");

        //extra hearts
        HEE = GameObject.Find("HE empty");
        HEH = GameObject.Find("HE half");
        HEF = GameObject.Find("HE full");


        EmptyHearts[0] = H1E;
        EmptyHearts[1] = H2E;
        EmptyHearts[2] = H3E;
        EmptyHearts[3] = H4E;

        HalfHearts[0] = H1H;
        HalfHearts[1] = H2H;
        HalfHearts[2] = H3H;
        HalfHearts[3] = H4H;

        FullHearts[0] = H1F;
        FullHearts[1] = H2F;
        FullHearts[2] = H3F;
        FullHearts[3] = H4F;
    }






    //interaction
    private IEnumerator Interact() {
        if (ListOfInteractables.Count != 0) {
            ClosestTarget = GetClosestEnemy().GetComponent<Interaction>();

            if (DistanceToClosestTarget() <= 1.5f && !ClosestTarget.getStartInteraction() && ClosestTarget.isActiveAndEnabled) {
                InteractText.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E)) {
                    yield return new WaitForSeconds(0.01f);
                    ClosestTarget.setStartInteraction(true);
                }
            } else {
                InteractText.SetActive(false);
                textBubble.SetActive(false);
            }
        }
        else {
            InteractText.SetActive(false);
            textBubble.SetActive(false);
        }
        if (ListOfInteractables.Count == 0) {
            foreach (GameObject interactable in GameObject.FindGameObjectsWithTag("Interactable")) {
                ListOfInteractables.Add(interactable);
            }
        }

        yield return null;
    }
    //finds the distance to the closest interactable target
    public float DistanceToClosestTarget() {
        float dist;
        dist = Vector3.Distance(GetClosestEnemy().transform.position, transform.position);

        return dist;
    }
    //finds the closest interactable target
    void ToggleKeybinds() {
        if (Input.GetKeyDown(KeyCode.F1) && !keybinds.activeInHierarchy) {
            keybinds.SetActive(true);
        } else if (Input.GetKeyDown(KeyCode.F1) && keybinds.activeInHierarchy) {
            keybinds.SetActive(false);
        }
    }
    private IEnumerator StoryChanger() {

        if (spawnPointName.Contains("L0")) {
            colliderLegs.enabled = true;
        }
        else {
            colliderLegs.enabled = false;
        }


        if (gm.StoryNumber == 0 && Vector3.Distance(playerTrans.position, grill.position) < 3f && Vector3.Distance(lb.position, grill.position) < 3f) {
            objectiveChanger.SetStoryNumber(1);
            gm.StoryNumber = 0.01f;
            gm.CheckStory = true;
        }
        if (gm.StoryNumber == 0.01f && isStealth) {
            objectiveChanger.SetStoryNumber(2);
            gm.StoryNumber = 0.02f;
            gm.CheckStory = true;
        }
        if (gm.StoryNumber == 0.03f && Vector3.Distance(playerTrans.position, football.position) < 1) {
            gm.StoryNumber = 0.04f;
            objectiveChanger.SetStoryNumber(4);
            Debug.Log("Look at me");
            gm.CheckStory = true;
        }
        if (gm.StoryNumber == 0.04f && Vector3.Distance(playerTrans.position, g7.position) < 10 && Vector3.Distance(football.position, g7.position) < 10) {
            objectiveChanger.SetStoryNumber(5);
            gm.StoryNumber = 0.05f;
            gm.CheckStory = true;
        }
        if (gm.StoryNumber == 0.05f) {
            playerTrans.transform.LookAt(lb.transform);
            lb.transform.LookAt(playerTrans.transform);
            football.LookAt(g9.transform);

            if (Vector3.Distance(playerTrans.position, PlayerPosC1.position) > 1) {

                transition.SetBool("Start", true);

                yield return new WaitForSeconds(1);

                playerTrans.transform.position = new Vector3(PlayerPosC1.position.x, PlayerPosC1.position.y, PlayerPosC1.position.z);
                football.transform.position = new Vector3(PlayerPosC1.position.x - 1, PlayerPosC1.position.y, PlayerPosC1.position.z);


                yield return new WaitForSeconds(1f);
                transition.SetBool("Start", false);
                yield return new WaitForSeconds(1.5f);
                objectiveChanger.SetStoryNumber(6);
                gm.StoryNumber = 0.06f;
                gm.CheckStory = true;
            }

        }
        if (gm.StoryNumber == 0.06f) {
            playerTrans.transform.LookAt(lb.transform);
            lb.transform.LookAt(playerTrans.transform);
            if (Vector3.Distance(football.position, g9.transform.position) > 1) {
                football.LookAt(g9.transform);
            }

            if (Input.GetKeyDown(KeyCode.E) && !kickedBall) {
                kickedBall = true;
                football.gameObject.GetComponent<Rigidbody>().AddForce(football.transform.forward * 15);
                yield return new WaitForSeconds(2f);
                transition.SetBool("Start", true);
                yield return new WaitForSeconds(1);
                transition.SetBool("Start", false);
                objectiveChanger.SetStoryNumber(7);
                gm.StoryNumber = 0.07f;
                gm.CheckStory = true;
            }

        }
        if (gm.StoryNumber == 0.09f && !check1) {
            check1 = true;
            transition.SetBool("Start", true);
            yield return new WaitForSeconds(1);
            transition.SetBool("Start", false);
            yield return new WaitForSeconds(1);
            if (!check3) {
                check3 = true;
                interaction.setStartInteraction(true);
            }
            
            
        }

        if (gm.StoryNumber == 1f && !check2) {
            teddy.setStartInteraction(true);
            check2 = true;
            while (teddy.getStartInteraction()) yield return null;

        }
        if (gm.StoryNumber == 1.01f) {
            setSpawnPointName("L1S2");
            setSpawnPoint(GameObject.Find(getSpawnPointName()).GetComponent<Transform>());
        }
        if (gm.StoryNumber == 1.02f) {
            setSpawnPointName("L1S3");
            setSpawnPoint(GameObject.Find(getSpawnPointName()).GetComponent<Transform>());
        }
        if (gm.StoryNumber >= 2f && gm.StoryNumber < 3f) {
            setSpawnPointName("L2S1");
            setSpawnPoint(GameObject.Find(getSpawnPointName()).GetComponent<Transform>());
        }


    }
    public GameObject GetClosestEnemy() {
        GameObject ClosestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject potentialTarget in ListOfInteractables) {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                ClosestTarget = potentialTarget;
            }
        }
        return ClosestTarget;
    }
    //look at target
    public void lookAtTarget(Transform target) {
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rotate = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * 2);
    }


    public void loseStaminaPeriodically() {
        if (Stamina > 0) {
            Stamina -= staminaRate * 0.5f * Time.deltaTime;
        }
        if (Stamina < 0) {
            setStamina(0);
        }
    }


    public void loseStaminaInstantly(float staminaCost) {
        float staminaLeft = Stamina - (staminaCost * staminaRate);
        if (staminaLeft <= 0) {
            setStamina(0);
        }
        else if (staminaLeft > 0) {
            Stamina -= (staminaCost * staminaRate);
        }
    }

    //recover stamina passively when not doing any major action
    private void recoverStamina(float staminaRecoveryRate) {
        if (runningAnimation || rollAnimation || jumpAnimation || attackAnimation1 || attackAnimation2 || attackAnimation3 || attackAnimation4) {
            recoveryTimer = 1f;
        }
        if (recoveryTimer < 0) {
            recoveryTimer = 0;
        }
        if (!runningAnimation && Stamina < maxStamina) {
            recoveryTimer -= Time.deltaTime;
            if (recoveryTimer <= 0) {
                Stamina += staminaRecoveryRate * Time.deltaTime;
            }
        }
        if (Stamina > maxStamina) {
            Stamina = maxStamina;
        }
    }



    private void updateStamina() {
        //change stamina color
        if (!isStaminaBuff && !waterGun.isInInventory) {
            staminaRate = 1f;
            staminaSlider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, (staminaSlider.value / staminaSlider.maxValue));
        }
        else if (!isStaminaBuff && waterGun.isInInventory) {
            staminaRate = 0.5f;
            staminaSlider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, (staminaSlider.value / staminaSlider.maxValue));
        }
        else if (isStaminaBuff) {
            staminaSlider.fillRect.GetComponent<Image>().color = Color.cyan;
            staminaRate = 0.3f;


            if (staminabuffTimer > 0 && isStaminaBuff) {
                staminabuffTimer -= Time.deltaTime;
                if (staminabuffTimer <= 0) {
                    isStaminaBuff = false;
                    staminabuffTimer = 60f;
                }
            }
        }


        if (walkAnimation || idleAnimation) {
            animationHasStarted = false;
            animationAttStarted1 = false;
            animationAttStarted2 = false;
            animationAttStarted3 = false;
        }

        if (runningAnimation) {
            loseStaminaPeriodically();
        }
        if (jumpAnimation && !animationHasStarted) {
            animationHasStarted = true;
            loseStaminaInstantly(0.5f);
        }
        if (attackAnimation1 && !animationAttStarted1) {
            animationAttStarted1 = true;
            loseStaminaInstantly(1);
        }
        if (attackAnimation2 && !animationAttStarted2) {
            animationAttStarted2 = true;
            loseStaminaInstantly(1);
        }
        if (attackAnimation3 && !animationAttStarted3) {
            animationAttStarted3 = true;
            loseStaminaInstantly(1);
        }
        if (attackAnimation4 && !animationAttStarted1) {
            animationAttStarted1 = true;
            loseStaminaInstantly(1);
        }
        if (rollAnimation && !animationHasStarted) {
            animationHasStarted = true;
            loseStaminaInstantly(1);
        }
    }



    public void setStaminaSlider(float Stamina) {
        staminaSlider.value = Stamina;
    }

    public void setMaxStaminaSlider(float maxStamina) {
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = maxStamina;
    }


    public void toggleInventory() {
        if (inventoryPressed && inventory.getGotBackPack()) {
            openInventory = !openInventory;
        }
    }

    private void BuffPlayer() {
        if (raceCar.isInInventory) {
            runSpeed = 5.5f;
        }
        if (racket.isInInventory) {
            attackDamage = 2;
        }
        if (waterGun.isInInventory) {
            //see 'updateStamina()'
        }
        if (footballItem.isInInventory) {
            neighbor.setAttackDamage(1);
        }
    }





    //bools for only playing sounds once
    public bool walkSound;
    public bool runSound;
    public bool crouchSound;
    public bool punch1Sound;
    public bool punch2Sound;
    public bool punch3Sound;
    public bool punch4Sound;
    public bool hitSound;


    private void SoundManager() {
        //walking sound
        if (walkAnimation && !walkSound) {
            audioManager.Stop("FootstepsCrouch");
            audioManager.Stop("FootstepsRun");
            audioManager.Play("FootstepsWalk", true, 0.01f, 1);
            walkSound = true;
        }
        else if (!walkAnimation) {
            audioManager.Stop("FootstepsWalk");
            walkSound = false;
        }

        //running sound
        if (runningAnimation && !runSound) {
            audioManager.Stop("FootstepsCrouch");
            audioManager.Stop("FootstepsWalk");
            audioManager.Play("FootstepsRun", true, 0.03f, 1.5f);
            runSound = true;
        }
        else if (!runningAnimation) {
            audioManager.Stop("FootstepsRun");
            runSound = false;
        }
        
        //crouch sound
        if(crouchWalkAnimation && !crouchSound) {
            audioManager.Stop("FootstepsRun");
            audioManager.Stop("FootstepsWalk");
            audioManager.Play("FootstepsCrouch", true, 0.01f, 0.75f);
            crouchSound = true;
        }
        else if (!crouchWalkAnimation) {
            audioManager.Stop("FootstepsCrouch");
            crouchSound = false;
        }




        if((attackAnimation1) && !punch1Sound) {
            audioManager.Play("Punch1", true, 0.02f, 0.75f);
            punch1Sound = true;
        }
        else if (!attackAnimation1) {
            audioManager.Stop("Punch1");
            punch1Sound = false;
        }

        if(attackAnimation2 && !punch2Sound) {
            audioManager.Play("Punch2", true, 0.02f, 0.9f);
            punch2Sound = true;
        }
        else if (!attackAnimation2) {
            audioManager.Stop("Punch2");
            punch2Sound = false;
        }

        if (attackAnimation3 && !punch3Sound) {
            audioManager.Play("Punch3", true, 0.02f, 0.65f);
            punch3Sound = true;
        }
        else if (!attackAnimation3) {
            audioManager.Stop("Punch3");
            punch3Sound = false;
        }

        if (attackAnimation4 && !punch4Sound) {
            audioManager.Play("Punch4", true, 0.02f, 0.6f);
            punch4Sound = true;
        }
        else if (!attackAnimation4) {
            audioManager.Stop("Punch4");
            punch4Sound = false;
        }


        if (hitAnimation && !hitSound) {
            audioManager.Play("PlayerHit", false, 0.1f, Random.Range(2, 3));
            hitSound = true;
        }
        else if (!hitAnimation) {
            audioManager.Stop("PlayerHit");
            hitSound = false;
        }


    }


    //getters and setters
    public Animator getAnimator() {
        return animator;
    }

    public Inventory getInventory() {
        return inventory;
    }

    public float getStamina() {
        return Stamina;
    }
    public void setStamina(float Stamina) {
        this.Stamina = Stamina;
    }

    public float getMaxStamina() {
        return maxStamina;
    }
    public void setMaxStamina(float maxStamina) {
        this.maxStamina = maxStamina;
    }

    public bool getIsStaminaBuff() {
        return isStaminaBuff;
    }

    public bool getIsEnegyBuff() {
        return isEnegyBuff;
    }

    public int getAttackDamage() {
        return attackDamage;
    }
    public void setAttackDamage(int attackDamage) {
        this.attackDamage = attackDamage;
    }

    public int getBonusAttackDamage() {
        return bonusAttackDamage;
    }
    public void setBonusAttackDamage(int bonusAttackDamage) {
        this.bonusAttackDamage = bonusAttackDamage;
    }

    public Weapon getEquipedWeapon() {
        return equipedWeapon;
    }

    public void setEquipedWeapon(Weapon equipedWeapon) {
        this.equipedWeapon = equipedWeapon;
    }


    public bool getInDialog() {
        return inDialog;
    }
    public void setInDialog(bool inDialog) {
        this.inDialog = inDialog;
    }

    public bool getCrouchToggle() {
        return crouchingToggle;
    }

    public void setCrouchToggle(bool crouchingToggle) {
        this.crouchingToggle = crouchingToggle;
    }


    public bool getIsStealth() {
        return isStealth;
    }
    public void setIsStealth(bool isStealth) {
        this.isStealth = isStealth;
    }


    public bool getIsInCombat() {
        return isInCombat;
    }
    public void setIsInCombar(bool isInCombat) {
        this.isInCombat = isInCombat;
    }

    public string getSpawnPointName() {
        return spawnPointName;
    }

    public void setSpawnPointName(string spawnPointName) {
        this.spawnPointName = spawnPointName;
    }

    public Transform getSpawnPoint() {
        return spawnPoint;
    }

    public void setSpawnPoint(Transform spawnPoint) {
        this.spawnPoint = spawnPoint;
    }


    public bool getOpenInventory() {
        return openInventory;
    }
    public void setOpenInventory(bool openInventory) {
        this.openInventory = openInventory;
    }

    public bool getIsDead() {
        return isDead;
    }

    public void setIsDead(bool isDead) {
        this.isDead = isDead;
    }

    public void setRespawnTimer(float respawnTimer) {
        this.respawnTimer = respawnTimer;
    }

    public List<GameObject> getListOfInteractables() {
        return ListOfInteractables;
    }
}





