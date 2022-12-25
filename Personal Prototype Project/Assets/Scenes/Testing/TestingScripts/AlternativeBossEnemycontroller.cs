using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

// StateMachine Practice for Boss Enemy
enum CurrentState
{
    Idle,
    ApproachingPlayer,
    AttackingPlayer,
}
public class AlternativeBossEnemycontroller : MonoBehaviour
{
    CurrentState currentState;
    Animator animator;

    float speed;
    float playerDistance;
    float angle;
    
    bool bossStrikeCooldown;
    bool bossShootCooldown;

    bool bossPhase2;
    bool bossPhase2CirclingSwordsOnlyExecuteOnce;
    bool bossPhase3;
    bool bossPhase3CirclingSwordsOnlyExecuteOnce;

    void SetState(CurrentState state)
    {
        currentState = state;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetState(CurrentState.Idle);
        Debug.Log("CurrentState = " + currentState);
        speed = 5;
    }
    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case CurrentState.Idle: 
                // ^ This is a State Machine State. In this line it's Idle state.
                // Follow same format for the ones below, if it starts with CurrentState.something, it's state for the state machine.
                IdleBehaviour(); // Idle Behaviour
                break;
            case CurrentState.ApproachingPlayer:
                ApproachingPlayerBehaviour(); // Approaching Player Behaviour
                break;
            case CurrentState.AttackingPlayer: 
                AttackingPlayerBehaviour(); // Attacking Player Behaviour
                break;
            default: // Default Behaviour. I.e. If statemachine does not recognize the state, that something is trying to put in. (It's not in enum CurrentState)
                Debug.Log("This state '" + currentState + "' for state machine doesn't exist, or there is a typo in name(string) of the state, defaulting to Idle State");
                SetState(CurrentState.Idle);
                break;
        }
    }
    //State Machine Behaviours methods. Make sure these are at top of the list of methods and preferably under update. Make sure keep this list separate from other methods.

    void IdleBehaviour() //Idle Behaviour for the boss. I.e. Do nothing.
    {
        // If player is not nearby, do nothing. If Player comes near, switch to ApproachingPlayerBehaviour.
    }
    void ApproachingPlayerBehaviour() //Approaching Player Behaviour for the boss.
    {
        // If player is nearby, start approaching in order to attack. When close enough, switch to AttackingPlayerBehaviour
        MoveTowardsPlayer();
    }
    void AttackingPlayerBehaviour() //Attacking Player Behaviour for the boss.
    {
        // If player is nearby enough to attack, we attack player. If not close enough, we switch back to ApproachingPlayerBehaviour.
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
            animator.SetBool("IsMovingRight", true);
        }
        //North
        else if (angle >= 45 && angle <= 135)
        {
            ResetAnimatorBooleanValues();
            animator.SetBool("IsMovingUp", true);

        }
        //South
        else if (angle >= -135 && angle <= -45)
        {
            ResetAnimatorBooleanValues();
            animator.SetBool("IsMovingDown", true);
        }
        //West
        else if ((angle >= 135 && angle <= 180) || (angle <= -135 && angle >= -180))
        {
            ResetAnimatorBooleanValues();
            animator.SetBool("IsMovingLeft", true);
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
        animator.SetBool("IsMovingUp", false);
        animator.SetBool("IsMovingDown", false);
        animator.SetBool("IsMovingRight", false);
        animator.SetBool("IsMovingLeft", false);
        animator.SetBool("IsNotAttackingUp", false);
        animator.SetBool("IsNotAttackingDown", false);
        animator.SetBool("IsNotAttackingRight", false);
        animator.SetBool("IsNotAttackingLeft", false);
        animator.SetBool("IsAttackingUp", false);
        animator.SetBool("IsAttackingDown", false);
        animator.SetBool("IsAttackingRight", false);
        animator.SetBool("IsAttackingLeft", false);
    }
}
