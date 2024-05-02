using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Player Stuff -----")]
    public CharacterController controller;
    [SerializeField] AudioSource aud;

    [Header("----- Player Stats -----")]
    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] int maxJumps;
    [SerializeField] float gravity;

    [Header("----- Guns -----")]
    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [SerializeField] GameObject gunModel;
    int shootDamage;
    float shootRate;
    int shootDist;

    [Header("----- Dash Properties -----")]
    [SerializeField] float camFovOriginal;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;
    [SerializeField] float dashFovChangeIntensity; // 0 to 1
    [SerializeField] float dashFovChangeLerpSpeed; // how fast to change the fov


    [Header("----- Audio -----")]
    [SerializeField] AudioClip audioAmmoRefill;
    [SerializeField][Range(0, 1)] float audioAmmoRefillVol;
    [SerializeField] AudioClip[] audStep;
    [SerializeField][Range(0, 1)] float audStepVol;
    [SerializeField] float stepSize;
    [SerializeField] AudioClip[] audJump;
    [SerializeField][Range(0, 1)] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField][Range(0, 1)] float audHurtVol;
    [SerializeField] AudioClip audDash;
    [SerializeField][Range(0,1)] float audDashVol;
    [SerializeField] AudioClip[] audGunSelect;
    [SerializeField][Range(0, 1)] float audGunSelectVol;

    Vector3 moveDir;
    Vector3 velocity;
    bool isShooting;
    int timesJumped;
    float timeSinceDashStart;
    int HPOriginal;
    float camFovTarget; // camera's target fov
    int selectedGun; // which gun we have equiped in our inventory
    float curStepDist; // used to know when to play another step sound

    [SerializeField] GameObject spawnObjTemp;

    // Start is called before the first frame update
    void Start()
    {
        HPOriginal = HP;
        spawnPlayer();
        timeSinceDashStart = dashDuration + dashCooldown + 1;
        camFovTarget = camFovOriginal;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isPaused)
        {
            // Draw player line of fire for debug purposes
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.green);

            // Do gun update
            SelectGun();

            // Do player movement
            Movement();

            // Do camera fov update
            UpdateCameraFov();

            // Do shoot coroutine
            if (Input.GetButton("Shoot") && !isShooting && gunList.Count > 0 && gunList[selectedGun].ammoCurrent > 0)
            {
                StartCoroutine(Shoot());
            }

            // Do dash coroutine
            if (Input.GetButtonDown("Dash") && timeSinceDashStart >= dashDuration + dashCooldown)
            {
                timeSinceDashStart = 0;
                aud.PlayOneShot(audDash, audDashVol);
            }

            if (Input.GetButtonUp("Dash") && timeSinceDashStart <= dashDuration)
            {
                timeSinceDashStart = dashDuration;
            }
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
                aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            }

            velocity.y -= (gravity / 2) * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            velocity.y -= (gravity / 2) * Time.deltaTime;

            // Do step sound stuff yay
            if (controller.isGrounded)
            {
                curStepDist += moveDir.magnitude * speed * Time.deltaTime;
                if (curStepDist >= stepSize)
                {
                    aud.PlayOneShot(audStep[Random.Range(0,audStep.Length)], audStepVol);
                    curStepDist = 0;
                }
            }
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

        gunList[selectedGun].ammoCurrent--;
        GameManager.instance.ammoCurrentText.text = gunList[selectedGun].ammoCurrent.ToString("F0");
        UpdatePlayerUI();

        aud.PlayOneShot(gunList[selectedGun].shootSound, gunList[selectedGun].shootSoundVolume);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
        {
            // THE RAYCAST HIT SOMETHING
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null && hit.transform != transform) // make sure you didn't hit yourself, too!
            {
                // hit a hitable obj
                dmg.TakeDamage(shootDamage);
            }
            else
            {
                // Hit a non-hitable obj
                Instantiate(gunList[selectedGun].hitEffect, hit.point, Quaternion.identity);
            }


            //Instantiate(spawnObjTemp, hit.point, transform.rotation); // temporary spawn something here
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    public void TakeDamage(int damage)
    {
        HP -= damage;
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        UpdatePlayerUI();
        StartCoroutine(FlashDamage());

        if (HP <= 0)
        {
            GameManager.instance.gameOver();
        }
    }

    void UpdatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
        
    }

    IEnumerator FlashDamage()
    {
        GameManager.instance.damageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.damageFlash.SetActive(false);
    }

    /// <summary>
    /// Gives a gun to the player
    /// </summary>
    /// 

    public bool RefillAmmo()
    {
        bool gainedAmmo = false;
        if (gunList.Count > 0)
        {
            for (int i = 0; i < gunList.Count; i++)
            {
                if (gunList[i].ammoCurrent != gunList[i].ammoMax)
                {
                    gainedAmmo = true;
                    gunList[i].ammoCurrent = gunList[i].ammoMax;
                    GameManager.instance.ammoCurrentText.text = gunList[selectedGun].ammoCurrent.ToString("F0");
                }
            }
            if (gainedAmmo)
            {
                aud.PlayOneShot(audioAmmoRefill, audioAmmoRefillVol);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    public void GiveGun(GunStats gun)
    {
        gunList.Add(gun);
        selectedGun = gunList.Count - 1;
        GameManager.instance.ammoCurrentText.text = gunList[selectedGun].ammoCurrent.ToString("F0");
        GameManager.instance.ammoMaxText.text = gunList[selectedGun].ammoMax.ToString("F0");
        ChangeGun();

        //shootDamage = gun.shootDamage;
        //shootDist = gun.shootDist;
        //shootRate = gun.shootRate;

        //gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        //gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    /// <summary>
    /// Handles selecting a gun (Scrolling to change selected gun)
    /// </summary>
    void SelectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1) // scrolling up
        {
            selectedGun++;
            ChangeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0) // scrolling down
        {
            selectedGun--;
            ChangeGun();
        }
    }

    /// <summary>w
    /// Updates the gun to whatever gun you have selected
    /// </summary>
    void ChangeGun()
    {
        if (gunList.Count > 0)
        {
            shootDamage = gunList[selectedGun].shootDamage;
            shootDist = gunList[selectedGun].shootDist;
            shootRate = gunList[selectedGun].shootRate;

            gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
            gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

            GameManager.instance.ammoCurrentText.text = gunList[selectedGun].ammoCurrent.ToString("F0");
            GameManager.instance.ammoMaxText.text = gunList[selectedGun].ammoMax.ToString("F0");

            aud.PlayOneShot(audGunSelect[Random.Range(0, audGunSelect.Length)], audGunSelectVol);

            UpdatePlayerUI();
        }
    }
    public void spawnPlayer()
    {
        HP = HPOriginal;
        UpdatePlayerUI();

        controller.enabled = false;
        transform.position = GameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }
}
