using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// StateMachine Practice for Boss Enemy
enum CurrentState
{
    Idle,
    ApproachingPlayer,
    AttackingPlayer,
    Hold,
}
public class AlternativeBossEnemycontroller : MonoBehaviour
{
    CurrentState currentState;

    [SerializeField]GameObject boulderPrefab;

    Animator animator;

    float health;
    float speed;
    float targetDistance;
    float angle;
    
    bool bossSmashCooldown;
    bool bossBoulderThrowCooldown;

    bool holdingPosition;

    bool bossPhase2;
    bool bossPhase2CirclingSwordsOnlyExecuteOnce;
    bool bossPhase3;
    bool bossPhase3CirclingSwordsOnlyExecuteOnce;

    void SetState(CurrentState state) // Method we use to change states in the state machine.
    {
        currentState = state;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetState(CurrentState.Idle);
        animator = GetComponent<Animator>();
        //Debug.Log("CurrentState = " + currentState);
        speed = 5;
    }
    // Update is called once per frame
    void Update()
    {
        targetDistance = PlayerDistanceCalculation();
        switch (currentState)
        {
            case CurrentState.Idle: // <-- This is a State Machine State. In this line it's Idle state. Follow same format for the ones below, if it starts with CurrentState.something, it's state for the state machine.
                IdleBehaviour(); // Idle Behaviour
                break;
            case CurrentState.Hold: // Hold Behaviour
                HoldBehaviour();
                break;
            case CurrentState.ApproachingPlayer:
                ApproachingPlayerBehaviour(); // Approaching Player Behaviour
                break;
            case CurrentState.AttackingPlayer: 
                AttackingPlayerBehaviour(); // Attacking Player Behaviour
                break;
            default: // Default Behaviour. I.e. If statemachine does not recognize the state, it will default to this state that something is trying to put into it. (It's not in enum CurrentState?)
                Debug.Log("This state '" + currentState + "' for state machine doesn't exist, or there is a typo in name(string) of the state, defaulting to Idle State");
                SetState(CurrentState.Idle);
                break;
        }
    }
    //Start of State Machine Behaviours methods. Make sure these are at top of the list of methods and preferably under update. Make sure keep this list separate from other methods.

    void IdleBehaviour() // Idle Behaviour for the boss. Should be default state. I.e. Being Idle, not doing anything meaningful.
    {
        if (targetDistance >= 3.5 && targetDistance <= 30)  // If player is not nearby, do nothing. If Player comes near, switch to ApproachingPlayerBehaviour.
        {
            SetState(CurrentState.ApproachingPlayer);
        }
    }
    void HoldBehaviour() // Hold Behavior for the boss. Stops whatever boss is/was doing, in order to do something else. Like throwing a boulder. I.e Empty method.
    {

    }
    void ApproachingPlayerBehaviour() //Approaching Player Behaviour for the boss. If player is nearby, start approaching in order to attack. When close enough, switch to AttackingPlayerBehaviour
    {
        MoveTowardsPlayer();

        if (bossBoulderThrowCooldown == false)
        {
            BossBoulderThrow();
        }

        if (targetDistance <= 3.5)
        {
            SetState(CurrentState.AttackingPlayer);
        }
    }
    void AttackingPlayerBehaviour() //Attacking Player Behaviour for the boss. If player is nearby enough to attack, we attack player. If not close enough, we switch back to ApproachingPlayerBehaviour.
    {
        if (targetDistance >= 3.5)
        {
            SetState(CurrentState.ApproachingPlayer);
        }
    }

    // End of Behaviours, start of everything else.
    void MoveTowardsPlayer()
    {
        //Sets correct animation, when approaching player. ie. Walking towards the player, based on the angle between this and player.
        AngleCalculation();

        //East
        if (angle >= -45 && angle <= 45)
        {
            ResetAnimatorBooleanValues();
            animator.SetBool("Boss_Moving_Right", true);
        }
        //North
        else if (angle >= 45 && angle <= 135)
        {
            ResetAnimatorBooleanValues();
            animator.SetBool("Boss_Moving_Up", true);

        }
        //South
        else if (angle >= -135 && angle <= -45)
        {
            ResetAnimatorBooleanValues();
            animator.SetBool("Boss_Moving_Down", true);
        }
        //West
        else if ((angle >= 135 && angle <= 180) || (angle <= -135 && angle >= -180))
        {
            ResetAnimatorBooleanValues();
            animator.SetBool("Boss_Moving_Left", true);
        }

        //Physically Moves character towards player.
        Vector2 moveDirection = (PlayerController.playerTransform.position - transform.position).normalized;
        transform.Translate(speed * Time.deltaTime * moveDirection);
    }
    float AngleCalculation()
    {
        float selfPosX = transform.position.x;
        float selfPosY = transform.position.y;
        float playerPosX = PlayerController.playerTransform.position.x;
        float playerPosY = PlayerController.playerTransform.position.y;

        Vector2 Point_1 = new Vector2(selfPosX, selfPosY);
        Vector2 Point_2 = new Vector2(playerPosX, playerPosY);
        angle = Mathf.Atan2(Point_2.y - Point_1.y, Point_2.x - Point_1.x) * Mathf.Rad2Deg;
        return angle;
    }
    void ResetAnimatorBooleanValues()
    {
        animator.SetBool("Boss_Moving_Right", false);
        animator.SetBool("Boss_Moving_Down", false);
        animator.SetBool("Boss_Moving_Up", false);
        animator.SetBool("Boss_Moving_Left", false);
        //animator.SetBool("IsNotAttackingUp", false);
        //animator.SetBool("IsNotAttackingDown", false);
        //animator.SetBool("IsNotAttackingRight", false);
        //animator.SetBool("IsNotAttackingLeft", false);
        //animator.SetBool("IsAttackingUp", false);
        //animator.SetBool("IsAttackingDown", false);
        //animator.SetBool("IsAttackingRight", false);
        //animator.SetBool("IsAttackingLeft", false);
    }

    void BossBoulderThrow() // Boss Boulder Throw ability and Animation bundled into one.
    {
        StartCoroutine(BossBoulderThrowCooldown());
        ResetAnimatorBooleanValues();
        AngleCalculation();

        // Total Animation Length = 1 second
        float timeUntilThrowFrame = 0.6f; // To make sure boulder is instantiated at the same time as boss's animation "throws" the boulder.
        float timeUntilAnimationEnds = 0.4f; // Run the rest of the animation, that is left.

        //East
        if (angle >= -45 && angle <= 45)
        {
            IEnumerator AnimationRunTime()
            {
                SetState(CurrentState.Hold);
                animator.SetBool("Boss_Throwing_Right", true);
                yield return new WaitForSeconds(timeUntilThrowFrame);
                BoulderThrow();
                yield return new WaitForSeconds(timeUntilAnimationEnds);
                SetState(CurrentState.ApproachingPlayer);
                animator.SetBool("Boss_Throwing_Right", false);
            }
            StartCoroutine(AnimationRunTime());
        }
        //North
        else if (angle >= 45 && angle <= 135)
        {
            IEnumerator AnimationRunTime()
            {
                SetState(CurrentState.Hold);
                animator.SetBool("Boss_Throwing_Up", true);
                yield return new WaitForSeconds(timeUntilThrowFrame);
                BoulderThrow();
                yield return new WaitForSeconds(timeUntilAnimationEnds);
                SetState(CurrentState.ApproachingPlayer);
                animator.SetBool("Boss_Throwing_Up", false);
            }
            StartCoroutine(AnimationRunTime());
        }
        //South
        else if (angle >= -135 && angle <= -45)
        {
            IEnumerator AnimationRunTime()
            {
                SetState(CurrentState.Hold);
                animator.SetBool("Boss_Throwing_Down", true);
                yield return new WaitForSeconds(timeUntilThrowFrame);
                BoulderThrow();
                yield return new WaitForSeconds(timeUntilAnimationEnds);
                SetState(CurrentState.ApproachingPlayer);
                animator.SetBool("Boss_Throwing_Down", false);
            }
            StartCoroutine(AnimationRunTime());
        }
        //West
        else if ((angle >= 135 && angle <= 180) || (angle <= -135 && angle >= -180))
        {
            IEnumerator AnimationRunTime()
            {
                SetState(CurrentState.Hold);
                animator.SetBool("Boss_Throwing_Left", true);
                yield return new WaitForSeconds(timeUntilThrowFrame);
                BoulderThrow();
                yield return new WaitForSeconds(timeUntilAnimationEnds);
                SetState(CurrentState.ApproachingPlayer);
                animator.SetBool("Boss_Throwing_Left", false);
                
            }
            StartCoroutine(AnimationRunTime());
        }
        void BoulderThrow()
        {
            float selfPosX = transform.position.x;
            float selfPosY = transform.position.y;
            float playerPosX = PlayerController.playerTransform.position.x;
            float playerPosY = PlayerController.playerTransform.position.y;

            Vector2 Point_1 = new Vector2(selfPosX, selfPosY);
            Vector2 Point_2 = new Vector2(playerPosX, playerPosY);
            float rotation = Mathf.Atan2(Point_2.y - Point_1.y, Point_2.x - Point_1.x) * Mathf.Rad2Deg;

            //Miksi vitussa t�ss� pit�� rotationiin laittaa -90, ett� toi kaava pit�� paikkansa, ku PlayerMiekassa sit� ei tarvi laittaa. wtf? :D
            Vector3 projectileStartRotation = new Vector3(0f, 0f, rotation - 90);
            Quaternion quaternion = Quaternion.Euler(projectileStartRotation);

            Instantiate(boulderPrefab, new Vector2 (transform.position.x, transform.position.y + 3), quaternion); // We want boulder to launch from top of the sprite. Since sprite is throwing boulder "over" itself.
        }
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;
    }
    float PlayerDistanceCalculation()
    {
        targetDistance = Vector2.Distance(PlayerController.playerTransform.position, transform.position);
        return targetDistance;
    }
    IEnumerator BossBoulderThrowCooldown()
    {
        bossBoulderThrowCooldown = true;
        yield return new WaitForSeconds(3);
        bossBoulderThrowCooldown = false;
    }
    IEnumerator BossSmashCooldown()
    {
        bossSmashCooldown = true;
        yield return new WaitForSeconds(1);
        bossSmashCooldown = false;
    }
}
