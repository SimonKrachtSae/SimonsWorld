using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreeperAi : MonoBehaviour
{
    private int health = 50;
    private MeshRenderer meshRenderer;
    private Material material;
    private Shader shader;
    
    private GameObject PlayerPref;

    private State currentState;

    private WanderState wanderState;
    private FollowState followState;
    private IdleState idleState;
    int stateNr;
    private bool destinationReached = false;
    //0 = wanderState
    //1 = idleState
    //2 = followState

    private float explosionTimer = 5;

    PathFinding pathFinding;

    [SerializeField] private AudioSource fireSound;
    [SerializeField] private AudioSource explosionSound;
    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        material = meshRenderer.material;
        shader = material.shader;
        PlayerPref = Player.Instance.gameObject;
        pathFinding = GetComponent<PathFinding>();
        wanderState = new WanderState(1, pathFinding, this);
        followState = new FollowState(3, PlayerPref,pathFinding);
        idleState = new IdleState( this.gameObject, this);
        currentState = wanderState;
        currentState.StartState();
    }
    private void Update()
    {
        Explosion();
        if(!destinationReached)
        {
            stateNr = 0;
        }
        if(destinationReached)
        {
            stateNr = 1;
        }
        if(PlayerIsInRange())
        {
            stateNr = 2;
        }
        switch(stateNr)
        {
            case 0:
                if(!(currentState is WanderState))
                {
                    SwitchCurrentState(wanderState);
                }
                break;
            case 1:
                if (!(currentState is IdleState))
                {
                    SwitchCurrentState(idleState);
                }
                break;
            case 2:
                if (!(currentState is FollowState))
                {
                    SwitchCurrentState(followState);
                }
                break;
        }

        currentState.RunState();
    }
    private void SwitchCurrentState(State nextState)
    {
        
        currentState.EndState();
        currentState = nextState;
        currentState.StartState();
    }
    private bool PlayerIsInRange()
    {
        if (PlayerPref == null)
            return false;

        if((PlayerPref.transform.position - transform.position).magnitude < 10)
        {
            return true;
        }
        return false;
    }
    public void SetDestinationReached(bool b)
    {
        destinationReached = b;
    }
    private void Explosion()
    {
        float explosionRadius = 2.0f;
        if(PlayerIsInRange())
        {
            fireSound.Play();
            explosionTimer -= Time.deltaTime;
            if(Mathf.RoundToInt(explosionTimer) % 2 != 0)
            {

                meshRenderer.material.color = Color.red;
            }
            else
            {
                meshRenderer.material.color = Color.green;
            }
            if(explosionTimer <= 0)
            {
                explosionSound.Play();
                World world = World.Instance;
                int scale = world.GetScale();
                for(int x = 0; x < scale; x++)
                {
                    for (int y = 0; y < scale; y++)
                    {
                        for (int z = 0; z < scale; z++)
                        {
                            if(world.GetWorldCubeAtIndex(x,y,z) != null)
                            {
                                if((world.GetWorldCubeAtIndex(x, y, z).transform.position - transform.position).magnitude < explosionRadius)
                                {
                                    world.DestroyBlock(world.GetWorldCubeAtIndex(x, y, z));
                                }
                            }
                        }
                    }
                }
                if((Player.Instance.transform.position - transform.position).magnitude < 4)
                {
                    Player.Instance.AddDamage(50);
                }

                Destroy(this.gameObject);
            }
        }
        else
        {
            meshRenderer.material.color = Color.green;
            explosionTimer = 6;
        }
    }
    public void AddDamage(int amount)
    {
        health -= amount;
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}

