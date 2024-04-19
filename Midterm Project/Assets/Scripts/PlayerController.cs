using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    public CharacterController controller;
    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] int maxJumps;
    [SerializeField] float gravity;

    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int gunDamage;

    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;
    [SerializeField] float dashFovChangeIntensity; // 0 to 1
    [SerializeField] float dashFovChangeLerpSpeed; // how fast to change the fov

    Vector3 moveDir;
    Vector3 velocity;
    bool isShooting;
    int timesJumped;
    float timeSinceDashStart;
    int HPOriginal;
    float camFovOriginal; // camera's original fov
    float camFovTarget; // camera's target fov

    [SerializeField] GameObject spawnObjTemp;

    // Start is called before the first frame update
    void Start()
    {
        HPOriginal = HP;
        updatePlayerUI();
        timeSinceDashStart = dashDuration + dashCooldown + 1;
        camFovOriginal = Camera.main.fieldOfView;
        camFovTarget = camFovOriginal;
    }

    // Update is called once per frame
    void Update()
    {
        // Draw player line of fire for debug purposes
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.green);

        // Do player movement
        Movement();

        // Do camera fov update
        UpdateCameraFov();

        // Do shoot coroutine
        if (Input.GetButton("Shoot") && !isShooting)
        {
            StartCoroutine(Shoot());
        }

        // Do dash coroutine
        if (Input.GetButtonDown("Dash") && timeSinceDashStart >= dashDuration + dashCooldown)
        {
            timeSinceDashStart = 0;
        }

        if (Input.GetButtonUp("Dash") && timeSinceDashStart <= dashDuration)
        {
            timeSinceDashStart = dashDuration;
        }
    }

    /// <summary>
    /// Does a tick of player movement
    /// </summary>
    void Movement()
    {
        timeSinceDashStart += Time.deltaTime;
        if (timeSinceDashStart < dashDuration)
        {
            ///// Dash stuff /////
            
            // Fov stuff
            camFovTarget = Mathf.Lerp(camFovOriginal, 179, dashFovChangeIntensity);

            // Movement
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            velocity.y = 0;
        }
        else
        {
            ///// Non-dash movement /////

            // Fov stuff
            camFovTarget = camFovOriginal;

            // Movement
            if (controller.isGrounded)
            {
                timesJumped = 0;
                velocity.y = 0;
            }

            moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
            controller.Move(moveDir * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && timesJumped < maxJumps)
            {
                timesJumped++;
                velocity.y = jumpForce;
            }

            velocity.y -= (gravity / 2) * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            velocity.y -= (gravity / 2) * Time.deltaTime;
        }
    }

    /// <summary>
    /// Does a tick of camera fov updating to match the target fov
    /// </summary>
    void UpdateCameraFov()
    {
        // Lerp the camera fov to the target fov
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, camFovTarget, Mathf.Clamp(dashFovChangeLerpSpeed * Time.deltaTime, 0.0f, 1.0f));
    }


    IEnumerator Shoot()
    {
        isShooting = true;

        // TODO: DO SHOOT THING HERE!!!

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f,0.5f)), out hit, shootDist))
        {
            // THE RAYCAST HIT SOMETHING
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null && hit.transform != transform) // make sure you didn't hit yourself, too!
            {
                dmg.takeDamage(shootDamage);
            }


            //Instantiate(spawnObjTemp, hit.point, transform.rotation); // temporary spawn something here
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    public void takeDamage(int damage)
    {
        HP -= damage;
        updatePlayerUI();
        StartCoroutine(flashDamage());

        if(HP <= 0)
        {
            GameManager.instance.gameOver();
        }
    }

    void updatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
    }

    IEnumerator flashDamage()
    {
        GameManager.instance.damageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.damageFlash.SetActive(false);
    }
}
