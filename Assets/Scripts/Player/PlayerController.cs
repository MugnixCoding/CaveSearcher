using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public enum PlayerState
{
    None,
    Stand,
    Sprint,
    Crouch
}
public class PlayerController : MonoBehaviour
{
    #region Variable
    private CharacterController characterController;
    private FirstPersonCamera firstPersonCamera;

    [Header("Stand")]
    [SerializeField] private Vector3 standCenter = new Vector3(0, 1.5f, 0);
    [SerializeField] private float standHeight = 3f;
    [SerializeField] private float standSpeed = 5f;
    [SerializeField] private float standFootstepTimer = 1f;

    [Header("Sprint")]
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float sprintFootstepTimer = 0.5f;

    [Header("Crouch")]
    [SerializeField] private float headTopGap = 0.35f;
    [SerializeField] private Vector3 crouchCenter = new Vector3(0, 0.75f, 0);
    [SerializeField] private float crouchHeight = 1.5f;
    [SerializeField] private float crouchSpeed = 1f;
    [SerializeField] private float crouchTime = 0.25f;

    private Vector3 playerVelocity;
    private bool isWalking;
    private Vector3 moveDir;
    private float speed;
    private PlayerState currentState;// Charater's Current State
    private PlayerState targetState;// the state that Player wants to enter 
    private bool IsInAir = false;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = -9.8f;

    [Header("Footstep Audio")]
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioClip[] walkFootstep;
    [SerializeField] private AudioClip[] sprintFootstep;
    [SerializeField] private AudioClip[] jumpFootstep;
    [SerializeField] private AudioClip[] landFootstep;
    private float footstepTimer=0;

    [Header("Damage Audio")]
    [SerializeField] private AudioClip[] damageSound;
    [SerializeField] private AudioClip[] deathSound;
    #endregion
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        firstPersonCamera = GetComponent<FirstPersonCamera>();
        playerAudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        InputManager.Instance.OnJumpAction += InputManager_OnJumpAction;
        InputManager.Instance.OnCrouchAction += InputManager_OnCrouchAction;
        InputManager.Instance.OnCrouchCanceled += InputManager_OnCrouchCanceled;
        InputManager.Instance.OnSprintAction += InputManager_OnSprintAction;
        InputManager.Instance.OnSprintCanceled += InputManager_OnSprintCanceled;

        GetComponent<PlayerHealthState>().OnTakeDamage += PlayHurtedSound;
        GetComponent<PlayerHealthState>().OnDead += PlayDeathSound;

        playerVelocity = new Vector3();
        moveDir = new Vector3();
        speed = standSpeed;
        currentState = PlayerState.Stand;
        targetState = PlayerState.None;
        IsInAir = false;

        firstPersonCamera.SetCameraHeight(standHeight);
    }
    void Update()
    {
        Vector2 inputVector = InputManager.Instance.GetMovementVectorNormalized();
        moveDir.x = inputVector.x;
        moveDir.z = inputVector.y;
        isWalking = moveDir != Vector3.zero;

        playerVelocity.y += gravity * Time.deltaTime;
        if (characterController.isGrounded && playerVelocity.y<0)
        {
            playerVelocity.y = -2f;
        }
        if (!characterController.isGrounded)
        {
            IsInAir = true;
        }
        if (IsInAir)
        {
            if (characterController.isGrounded)
            {
                playerAudioSource.PlayOneShot(landFootstep[Random.Range(0, landFootstep.Length - 1)]);
                IsInAir = false;
            }
        }
        PlayFootStepSound();
    }
    private void FixedUpdate()
    {
        characterController.Move(transform.TransformDirection(moveDir) * speed * Time.deltaTime);
        characterController.Move(playerVelocity * Time.deltaTime);
    }


    #region Input Action
    private void InputManager_OnJumpAction(object sender, System.EventArgs e)
    {
        Jump();
    }
    private void InputManager_OnCrouchAction(object sender, System.EventArgs e)
    {
        Crouch();
    }
    private void InputManager_OnCrouchCanceled(object sender, System.EventArgs e)
    {
        Stand();
    }

    private void InputManager_OnSprintAction(object sender, System.EventArgs e)
    {
        Sprint();
    }
    private void InputManager_OnSprintCanceled(object sender, System.EventArgs e)
    {
        if (currentState != PlayerState.Sprint)
        {
            return;
        }
        Stand();
    }
    #endregion

    #region Movement
    public void Jump()
    {
        if (characterController.isGrounded)
        {
            playerAudioSource.PlayOneShot(jumpFootstep[Random.Range(0, jumpFootstep.Length - 1)]);
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -1.0f * gravity);
        }
    }
    public void Crouch()
    {
        targetState = PlayerState.Crouch;
        StartCoroutine(ChangeHeight(crouchHeight, crouchCenter, crouchSpeed, PlayerState.Crouch));
    }
    public void Stand()
    {
        targetState = PlayerState.Stand;
        StartCoroutine(ChangeHeight(standHeight, standCenter,standSpeed, PlayerState.Stand));
    }
    public void Sprint()
    {
        if (currentState == PlayerState.Crouch)
        {
            return;
        }
        footstepTimer = 0;
        currentState =PlayerState.Sprint;
        speed = sprintSpeed;
    }
    IEnumerator ChangeHeight(float targetHeight,Vector3 targetCenter,float targetSpeed,PlayerState stateChecker)
    {
        float elapsedTime = 0;
        float currentHeight = characterController.height;
        Vector3 currentCenter = characterController.center;

        if (stateChecker == PlayerState.Stand)
        {
            while (elapsedTime < crouchTime && targetState == stateChecker)
            {
                if (!Physics.Raycast(GetCurrentTop(), Vector3.up, headTopGap))
                {
                    characterController.height = Mathf.Lerp(currentHeight, targetHeight, elapsedTime / crouchTime);
                    characterController.center = Vector3.Lerp(currentCenter, targetCenter, elapsedTime / crouchTime);
                    firstPersonCamera.SetCameraHeight(Mathf.Lerp(currentHeight, targetHeight, elapsedTime / crouchTime));
                    elapsedTime += Time.deltaTime;
                }
                yield return null;
            }
        }
        else
        {
            while (elapsedTime < crouchTime && targetState == stateChecker)
            {
                characterController.height = Mathf.Lerp(currentHeight, targetHeight, elapsedTime / crouchTime);
                characterController.center = Vector3.Lerp(currentCenter, targetCenter, elapsedTime / crouchTime);
                firstPersonCamera.SetCameraHeight(Mathf.Lerp(currentHeight, targetHeight, elapsedTime / crouchTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        if (targetState == stateChecker)
        {
            speed = targetSpeed;
            currentState = targetState;
            characterController.height = targetHeight;
            characterController.center = targetCenter;
            firstPersonCamera.SetCameraHeight(targetHeight);
            targetState = PlayerState.None;
        }
    }
    #endregion

    #region Audio
    private void PlayFootStepSound()
    {
        if (!characterController.isGrounded)
        {
            return;
        }
        if (moveDir == Vector3.zero)
        {
            return;
        }
        footstepTimer -= Time.deltaTime;
        if (footstepTimer<=0)
        {
            switch (currentState)
            {
                case PlayerState.Stand:
                    playerAudioSource.PlayOneShot(walkFootstep[Random.Range(0, walkFootstep.Length - 1)]);
                    footstepTimer = standFootstepTimer;
                    break;
                case PlayerState.Sprint:
                    playerAudioSource.PlayOneShot(sprintFootstep[Random.Range(0, sprintFootstep.Length - 1)]);
                    footstepTimer = sprintFootstepTimer;
                    break;
            }
        }

    }
    private void PlayHurtedSound(object sender, DamageEventArgs e)
    {
        playerAudioSource.PlayOneShot(damageSound[Random.Range(0, damageSound.Length - 1)]);
    }
    private void PlayDeathSound(object sender,EventArgs e)
    {
        playerAudioSource.PlayOneShot(deathSound[Random.Range(0, deathSound.Length - 1)]);
    }
    #endregion

    private Vector3 GetCurrentTop()
    {
        return new Vector3(transform.position.x, transform.position.y + characterController.height, transform.position.z);
    }

    public bool IsWalking()
    {
        return isWalking;
    }
    public PlayerState GetPlayerState()
    {
        return currentState;
    }
}
