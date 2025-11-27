using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private List<string> m_OwnedKeys = new List<string>();

    AudioSource m_AudioSource;
    Animator m_Animator;
    public InputAction MoveAction;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;
    public float speedDuration = 2.0f;
    public float speedCooldown = 4.0f;

    // Part of  minor mod (freezing from fear.) -Emily
    public bool isPanicking;
    public bool onCooldown;
    public int fightPanic; 
    //public GameObject panicCanvas;
    public GameObject panicText;
    public GameObject speedIcon;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_Rigidbody = GetComponent<Rigidbody>();
        MoveAction.Enable();
        m_Animator = GetComponent<Animator>();

        // Part of  minor mod (freezing from fear.) -Emily
        isPanicking = false;
        fightPanic = 0;
        //panicCanvas.SetActive(false);
        panicText.SetActive(false);
        speedIcon.SetActive(false);
        float panicTime = Random.Range(15, 20);
        InvokeRepeating("StartPanic", panicTime, panicTime);
    }

    void FixedUpdate()
    {
        m_Animator.SetBool("IsPanicking", isPanicking);

        var pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVeritalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVeritalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        SpeedBoost();

        if (isPanicking == true) // (all of this) is part of  minor mod (freezing from fear.) -Emily
        {
            Debug.Log("Too scared!");
            //panicCanvas.SetActive(true);
            panicText.SetActive(true);
            isWalking = false;

            if (m_AudioSource.isPlaying) {
                m_AudioSource.Stop();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (fightPanic < 3)
                {
                    fightPanic += 1;
                }
                else {
                    isPanicking = false;
                    fightPanic = 0;

                }
            }
        }
        else 
        {
            //panicCanvas.SetActive(false);
            panicText.SetActive(false);

            // Original walk script below
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);

            m_Rigidbody.MoveRotation(m_Rotation);
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * walkSpeed * Time.deltaTime);

            if (isWalking)
            {
                if (!m_AudioSource.isPlaying)
                {
                    m_AudioSource.Play();
                }
            }
            else
            {
                m_AudioSource.Stop();
            }
        }
    }

    // Part of  minor mod (freezing from fear.) -Emily
    public void StartPanic() {
        if (isPanicking == false)
        {
            isPanicking = true;
        }
        else {
            Debug.Log("Already Panicking!");
        }
    }

    IEnumerator SpeedPowerDown() 
    {
        onCooldown = true;
        speedIcon.SetActive(true);
        yield return new WaitForSeconds(speedDuration);
        speedIcon.SetActive(false);
        walkSpeed = 1f;
        yield return new WaitForSeconds(speedCooldown);
        onCooldown = false;
        
    }

    //The below Sections are for speed boost implementation
    private void SpeedBoost() 
    {
        if (Input.GetKey(KeyCode.LeftShift) && onCooldown == false)
        {
            walkSpeed = 3f;
            StartCoroutine(SpeedPowerDown());
        }
        
    }

    // Below is part of the door & key stuff. I couldn't get this working, don't know why.
    public void AddKey(string keyName)
    {
        m_OwnedKeys.Add(keyName);
    }

    public bool OwnKey(string keyName)
    {
        return m_OwnedKeys.Contains(keyName);
    }
}