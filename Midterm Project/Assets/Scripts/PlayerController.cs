using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
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

    Vector3 moveDir;
    Vector3 velocity;
    bool isShooting;
    int timesJumped;
    float timeSinceDashStart;
    int HPOriginal;

    [SerializeField] GameObject spawnObjTemp;

    // Start is called before the first frame update
    void Start()
    {
        timeSinceDashStart = dashDuration + dashCooldown + 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Draw player line of fire for debug purposes
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.green);

        // Do player movement
        Movement();

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
        // Dash stuff
        timeSinceDashStart += Time.deltaTime;
        if (timeSinceDashStart < dashDuration)
        {
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            velocity.y = 0;
        }
        else
        {
            // Non-dash movement
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


    IEnumerator Shoot()
    {
        isShooting = true;

        // TODO: DO SHOOT THING HERE!!!

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f,0.5f)), out hit, shootDist))
        {
            // THE RAYCAST HIT SOMETHING
            Instantiate(spawnObjTemp, hit.point, transform.rotation); // temporary spawn something here
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    public void TakeDamage(int damage)
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
