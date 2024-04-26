using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Collider weaponCollider;

    [SerializeField] int HP;
    [SerializeField] int targetSpeed;
    [SerializeField] float ViewCone;
    [SerializeField] float RoamDistance;
    [SerializeField] float RoamPauseTimer;
    [SerializeField] float animspeedTrans;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    bool isShooting;
    bool playerInRange;
    bool destinationChosen;
    Vector3 playerDirection;
    Vector3 startingPos;
    float angleToPlayer;
    float stoppingDistanceOrig;


    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        float animspeed = agent.velocity.normalized.magnitude;
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), animspeed, Time.deltaTime * animspeedTrans));

        if (playerInRange && !CanSeePlayer())
        {
            StartCoroutine(roam());
        }
        else if (!playerInRange)
        {
            StartCoroutine(roam());
        }
    }

    IEnumerator roam()
    {
        if (!destinationChosen && agent.remainingDistance < 0.05f)
        {
            destinationChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(RoamPauseTimer);

            Vector3 randomPos = Random.insideUnitSphere * RoamDistance;
            randomPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, RoamDistance, 1);
            agent.SetDestination(hit.position);

            destinationChosen = false;
        }
    }
    bool CanSeePlayer()
    {
        playerDirection = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, playerDirection.y + 1, playerDirection.z), transform.forward);
        //Debug.Log(angleToPlayer);
        Debug.DrawRay(headPos.position, playerDirection);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            //Debug.Log(hit.transform.name);

            if (hit.collider.CompareTag("Player") && angleToPlayer <= ViewCone)
            {
                agent.stoppingDistance = stoppingDistanceOrig;
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }
                return true;
            }
        }

        agent.stoppingDistance = 0;
        return false;
    }

    void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }

    void faceTarget()
    {
        Quaternion rota = Quaternion.LookRotation(new Vector3(playerDirection.x, transform.position.y, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rota, Time.deltaTime * targetSpeed);
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        WeaponCollideOff();
        animator.SetTrigger("Damage");
        StartCoroutine(flashRed());

        agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            GameManager.instance.updateWinCondition(-1);
            Destroy(gameObject);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void CreateBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    public void WeaponCollideOn()
    {
        weaponCollider.enabled = true;
    }
    public void WeaponCollideOff()
    {
        weaponCollider.enabled = false;
    }
}
