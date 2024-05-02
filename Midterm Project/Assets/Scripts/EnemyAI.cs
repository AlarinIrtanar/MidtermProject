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

    [SerializeField] bool isMelee; // force the enemy to be melee
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    bool isShooting;
    bool playerInRange;
    bool destinationChosen;
    Vector3 playerDirection;
    Vector3 startingPos;
    float angleToPlayer;
    float stoppingDistanceOrig;

    public WaveSpawner mySpawner; // the spawner the enemy came from

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;
        if (!mySpawner)
        { 
            GameManager.instance.updateWinCondition(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float animspeed = agent.velocity.normalized.magnitude;
        animator.SetFloat("Speed", Mathf.MoveTowards(animator.GetFloat("Speed"), animspeed, Time.deltaTime * animspeedTrans));

        if (playerInRange && !CanSeePlayer())
        {
            StartCoroutine(Roam());
        }
        else if (!playerInRange)
        {
            StartCoroutine(Roam());
        }
    }

    IEnumerator Roam()
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
                    if (!isMelee || (transform.position - GameManager.instance.player.transform.position).magnitude <= 2.0f)
                    {
                        StartCoroutine(Attack());
                    }
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
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

    void FaceTarget()
    {
        Quaternion rota = Quaternion.LookRotation(new Vector3(playerDirection.x, transform.position.y, playerDirection.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rota, Time.deltaTime * targetSpeed);
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        WeaponCollideOff();
        animator.SetTrigger("Damage");
        StartCoroutine(FlashRed());

        agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            // DEATH
            GameManager.instance.updateWinCondition(-1);
            if (mySpawner) mySpawner.UpdateEnemyNumber();
            Destroy(gameObject);
        }
    }
    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    IEnumerator Attack()
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
