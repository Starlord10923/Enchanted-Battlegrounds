using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float horizontalAxis;
    float verticalAxis;
    public float moveSpeed = 10f;
    public float rotationSpeed = 20f;
    public float jumpForce = 10f;
    public bool isJumping = false;
    public bool idle = true;
    public bool isAttacking = false;
    public bool isDefending = false;
    public float reviveCost = 12.5f;
    CharacterController characterController;
    Animator playerAnimator;
    UnitAttributes unitAttributes;
    public Weapon weaponEquipped;
    public bool isPaused = false;
    public int maxSoldierUnits = 6;
    public int currentSoldierUnits = 0;
    public AudioSource audioSourceRun;
    public AudioSource audioSource;
    public AudioClip jumpClip;
    public AudioClip attackClip;
    public AudioClip potionClip;

    void Start(){
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
        unitAttributes = GetComponent<UnitAttributes>();
        weaponEquipped = GetComponentInChildren<Weapon>();
        audioSourceRun = GetComponents<AudioSource>()[0];
        audioSource = GetComponents<AudioSource>()[1];
    }

    void Update(){
        horizontalAxis = Input.GetAxis("Horizontal");
        verticalAxis = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if(!isPaused)
            RotateOnMouse(mouseX,mouseY);

        if(Input.GetKeyDown(KeyCode.Space) && !isJumping)
            Jump();

        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(1)) && !isAttacking && !isDefending)
            Attack();

        if (Input.GetKeyDown(KeyCode.X) && !isAttacking && !isDefending)
            Defend();
        if (Input.GetKeyUp(KeyCode.X))
            playerAnimator.SetBool("HoldDefend", false);

        if (Input.GetKeyDown(KeyCode.C))
            ReviveUnit();

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject.Find("GameEndCanvas").GetComponent<MenuManager>().PauseGame();
            isPaused = !isPaused;
        }

        idle = !isJumping && !isDefending && horizontalAxis == 0 && verticalAxis == 0;
        playerAnimator.SetFloat("HorizontalAxis", horizontalAxis);
        playerAnimator.SetFloat("VerticalAxis", verticalAxis);
        playerAnimator.SetBool("Idle", idle);
    }

    void FixedUpdate(){
        MovePlayer();
    }
    void RotateOnMouse(float mouseX,float mouseY){
        float minPitchAngle = -20.0f;
        float maxPitchAngle = 20.0f;

        Quaternion horizontalRotation = Quaternion.Euler(0, mouseX * rotationSpeed, 0);
        Quaternion verticalRotation = Quaternion.Euler(-mouseY * rotationSpeed, 0, 0);

        Quaternion targetRotation = transform.rotation * horizontalRotation * verticalRotation;

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.1f);

        float currentPitch = transform.eulerAngles.x;

        if (currentPitch > 180)
            currentPitch -= 360; // Convert to the range -180 to 180 degrees
        currentPitch = Mathf.Clamp(currentPitch, minPitchAngle, maxPitchAngle);

        transform.rotation = Quaternion.Euler(currentPitch, transform.eulerAngles.y, 0f);
    }
    void MovePlayer(){
        Vector3 direction = new(horizontalAxis, 0f, verticalAxis);
        Vector3 worldDirection = transform.TransformDirection(direction);
        Vector3 moveVector = worldDirection.normalized * moveSpeed;
        Vector3 jumpVector = isJumping ? Vector3.up * jumpForce : Vector3.zero;
        characterController.SimpleMove(moveVector+jumpVector);
        RunSound();
    }
    void Jump(){
        isJumping = true;
        playerAnimator.SetTrigger("Jump");
        StartCoroutine(HandleJumpWithDelay());
    }
    void Attack(){
        float attackActionTime = 0.9f;
        isAttacking = true;
        playerAnimator.SetTrigger("Attack1");
        playerAnimator.SetBool("inBattle",true);
        StartCoroutine(AnimationLayerTransition(attackActionTime));
        if (unitAttributes.currentMana >= unitAttributes.fireballManaCost)
        {
            weaponEquipped.Invoke("FireBall", 0.5f);
            unitAttributes.SpendMana(unitAttributes.fireballManaCost);
            audioSource.clip = attackClip;
            audioSource.Play();
        }
    }
    void Defend(){
        float defendActionTime = 5.0f;
        isDefending = true;
        playerAnimator.SetBool("Defend",true);
        playerAnimator.SetBool("HoldDefend",true);
        playerAnimator.SetBool("inBattle",true);
        StartCoroutine(AnimationLayerTransition(defendActionTime));
    }
    IEnumerator HandleJumpWithDelay(){
        audioSource.clip = jumpClip;
        audioSource.Play();

        float jumpVelocity = Mathf.Sqrt(2 * jumpForce * Mathf.Abs(Physics.gravity.y));
        float time = 0f;

        while (time < jumpVelocity / jumpForce){
            characterController.Move(Vector3.up * (jumpForce * Time.deltaTime));
            time += Time.deltaTime;
            yield return null;
        }

        // yield return new WaitForSeconds(13/6);
        isJumping = false;
    }
    IEnumerator AnimationLayerTransition(float transitionTime){
        playerAnimator.SetLayerWeight(0, 0);
        playerAnimator.SetLayerWeight(1, 1);

        yield return new WaitForSeconds(transitionTime); // 1.0f is Attack1 Animation clip Length

        playerAnimator.SetLayerWeight(0, 1);
        playerAnimator.SetLayerWeight(1, 0);

        isAttacking = false;
        isDefending = false;
        playerAnimator.SetBool("Defend",false);
        playerAnimator.SetBool("inBattle",false);

    }
    public void RunSound(){
        if (!audioSourceRun.isPlaying && !idle)
            audioSourceRun.Play();
        else if (idle)
            audioSourceRun.Stop();
    }
    public void PlayPotionSound(){
        audioSource.clip = potionClip;
        audioSource.Play();
    }
    public void GetHitAction(bool isDead,Transform attacker){
        if (attacker.GetComponent<SoldierUnit>())
            attacker.GetComponent<SoldierUnit>().HitSound();

        StopAllActionCoroutines();
        playerAnimator.SetTrigger("GetHit");
        playerAnimator.SetBool("inBattle",true);
        if (isDead)
            DieAction();
        else
            StartCoroutine(AnimationLayerTransition(1.0f));
    }
    public void DieAction(){
        playerAnimator.SetBool("isDead",true);
        playerAnimator.SetTrigger("isDeadTrigger");
        StartCoroutine(AnimationLayerTransition(3.0f));

        GameObject.Find("GameManager").GetComponent<GameManager>().Invoke("PlayerDead",0.4f);
    }
    public void StopAllActionCoroutines(){
        StopCoroutine(nameof(AnimationLayerTransition));
    }
    void ReviveUnit()
    {
        GameObject coffin = GameObject.FindWithTag("Coffin");
        if (coffin != null && unitAttributes.currentMana >= reviveCost && currentSoldierUnits<maxSoldierUnits)
        {
            CoffinRevival coffinRevival = coffin.GetComponent<CoffinRevival>();
            StartCoroutine(coffinRevival.ReviveCoroutine(tag));
            unitAttributes.SpendMana(reviveCost);
            Destroy(coffin, 0.07f);
            currentSoldierUnits++;
        }
    }
}

//Handle the issue with Hold Defend Animation time