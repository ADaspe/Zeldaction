using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_BossManager : MonoBehaviour
{
    [HideInInspector]
    public ELC_BossMoves BossMoves;
    [HideInInspector]
    public ELC_BossAttacks BossAttacks;
    [HideInInspector]
    public ELC_BossHealth BossHealth;
    public GameObject RynGO;
    public Transform Spawn;
    public Transform MapCenter;
    public bool isAttacking;
    public bool canAttack;
    public Vector3 LastDir = Vector3.zero;
    public int CurrentPhase = 1;
    public LayerMask ObstaclesMask;
    public ELC_BossRay[] RaysList;
    public bool IsInSwitchPhase;
    public float GrowlAnimationTime;

    private void Awake()
    {
        BossMoves = this.GetComponent<ELC_BossMoves>();
        BossAttacks = this.GetComponent<ELC_BossAttacks>();
        BossHealth = this.GetComponent<ELC_BossHealth>();
        BossMoves.Target = RynGO.transform.position;
        BossMoves.TargetGO = RynGO;
        BossAttacks.Rays = RaysList;
        canAttack = true;
        CurrentPhase = 1;
    }

    public void Attack(Vector3 TargetDir)
    {
        BossMoves.CanMove = false;
        BossAttacks.TargetDirection = TargetDir;
        BossAttacks.PrepareAttack();
    }

    public void SwitchPhase()
    {
        CurrentPhase++;
        switch (CurrentPhase)
        {
            case 2:
                StartCoroutine(SecondPhaseSwitch());
                break;
            case 3:
                StartCoroutine(ThirdPhaseSwitch());
                break;
            default:
                break;
        }
    }

    public IEnumerator SecondPhaseSwitch()
    {
        IsInSwitchPhase = true;
        BossHealth.CurrentHealth = BossHealth.SecondPhaseHealth;
        BossMoves.FollowPlayer = false;

        //La caméra se centre sur le boss
        BossMoves.CanMove = false;
        yield return new WaitForSeconds(1);
        BossMoves.CanMove = true;

        //Joue l'animation de grognement
        Debug.Log("Graou");
        yield return new WaitForSeconds(GrowlAnimationTime);

        BossMoves.isGoingToPreciseLocation = true;
        BossMoves.Target = MapCenter.position;
    }

    public IEnumerator SecondAndThirdPhaseSwitch2()
    {
        //Apparition bouclier + grognement
        Debug.Log("Graou + shield");
        BossMoves.CanMove = false;
        yield return new WaitForSeconds(GrowlAnimationTime);
        if(CurrentPhase == 2) BossMoves.CanMove = true;
        Debug.Log("Phase " + CurrentPhase);
        if (CurrentPhase == 3) BossAttacks.StartCoroutine("RayPhase");
        IsInSwitchPhase = false;
        BossMoves.FollowPlayer = true;
    }

    public IEnumerator ThirdPhaseSwitch()
    {
        IsInSwitchPhase = true;
        BossHealth.CurrentHealth = BossHealth.ThirdPhaseHealth;
        BossMoves.FollowPlayer = false;

        //La caméra se centre sur le boss
        BossMoves.CanMove = false;
        yield return new WaitForSeconds(1);
        BossMoves.CanMove = true;

        //Joue l'animation de grognement
        Debug.Log("Graou");
        yield return new WaitForSeconds(GrowlAnimationTime);

        BossMoves.isGoingToPreciseLocation = true;
        BossMoves.Target = Spawn.position;
    }




    public void ReachedLocation()
    {
        switch (CurrentPhase)
        {
            case 2:
                StartCoroutine(SecondAndThirdPhaseSwitch2());
                break;
            case 3:
                StartCoroutine(SecondAndThirdPhaseSwitch2());
                break;

                
            default:
                break;
        }
    }




}
