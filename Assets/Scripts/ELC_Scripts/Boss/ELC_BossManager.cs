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
    [HideInInspector]
    public ELC_SwitchCamFocus CamScript;
    public GameObject RynGO;
    public GameObject MushSpawner;
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
    public float InvisibilityShaderTime;
    private SpriteRenderer SpriteRend;
    private Animator anims;
    [HideInInspector]
    public FLC_BossDynamicMusicFonctions music;
    public bool isStunned;

    private void Awake()
    {
        anims = this.GetComponent<Animator>();
        SpriteRend = this.gameObject.GetComponent<SpriteRenderer>();
        CamScript = this.GetComponent<ELC_SwitchCamFocus>();
        BossMoves = this.GetComponent<ELC_BossMoves>();
        BossAttacks = this.GetComponent<ELC_BossAttacks>();
        BossHealth = this.GetComponent<ELC_BossHealth>();
        BossMoves.Target = RynGO.transform.position;
        BossMoves.TargetGO = RynGO;
        BossAttacks.Rays = RaysList;
        canAttack = true;
        CurrentPhase = 0;
        music = GetComponentInChildren<FLC_BossDynamicMusicFonctions>();
    }

    public void Attack(Vector3 TargetDir)
    {
        anims.SetBool("Growl", true);
        anims.SetBool("AttackPhase", true);
        BossMoves.CanMove = false;
        BossAttacks.TargetDirection = TargetDir;
        BossAttacks.PrepareAttack();
    }

    public void SwitchPhase()
    {
        CurrentPhase++;
        switch (CurrentPhase)
        {
            case 1:
                StartCoroutine(Intro());
                break;
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

    public IEnumerator Intro()
    {
        BossMoves.distToStopNearTarget = BossMoves.BasicDistToStopNearPlayer;
        IsInSwitchPhase = true;
        BossMoves.CanMove = false;
        BossHealth.CurrentHealth = BossHealth.FirstPhaseHealth;
        CamScript.SwitchCamFocus(this.transform, false);
        yield return new WaitForSeconds(1);
        Debug.Log("Agrou");
        yield return new WaitForSeconds(GrowlAnimationTime);
        music.MusicsStart();
        StartCoroutine(BossAttacks.Fade());
        Debug.Log("Invisibilité");
        yield return new WaitForSeconds(InvisibilityShaderTime);
        Debug.Log("début phase 1");
        IsInSwitchPhase = false;
        BossMoves.CanMove = true;
        CamScript.CancelCamFocus();

    }

    public IEnumerator SecondPhaseSwitch()
    {
        BossMoves.distToStopNearTarget = BossMoves.DashDistToStopNearPlayer;
        StartCoroutine(BossAttacks.Fade(true));
        BossAttacks.CancelInvoke();
        BossAttacks.EndAttack();
        BossAttacks.StopAllCoroutines();
        BossAttacks.enabled = false;
        canAttack = false;
        IsInSwitchPhase = true;
        //SpriteRend.enabled = true;
        BossHealth.CurrentHealth = BossHealth.SecondPhaseHealth;
        BossMoves.FollowPlayer = false;
        CamScript.SwitchCamFocus(this.transform, false);

        BossMoves.CanMove = false;
        yield return new WaitForSeconds(1);


        //Joue l'animation de grognement
        anims.SetBool("Growl", true);
        Debug.Log("Graou");
        yield return new WaitForSeconds(GrowlAnimationTime);
        anims.SetBool("isGrowling", false);
        BossMoves.CanMove = true;
        BossMoves.isGoingToPreciseLocation = true;
        BossMoves.Target = MapCenter.position;
        music.SwitchMusicPart(2);
    }

    public IEnumerator ThirdPhaseSwitch()
    {
        BossAttacks.CancelInvoke();
        BossAttacks.EndAttack();
        BossAttacks.StopAllCoroutines();
        BossAttacks.enabled = false;
        BossHealth.HaveShield = false;
        BossHealth.ShieldGO.GetComponent<Animator>().SetBool("Dissolve", true);
        canAttack = false;
        CamScript.SwitchCamFocus(this.transform, false);
        IsInSwitchPhase = true;
        BossHealth.CurrentHealth = BossHealth.ThirdPhaseHealth;
        BossMoves.FollowPlayer = false;
        BossMoves.CanMove = false;
        yield return new WaitForSeconds(1);

        BossHealth.ShieldGO.GetComponent<Animator>().SetBool("Dissolve", false);
        BossHealth.ShieldGO.GetComponent<ParticleSystem>().Stop();

        anims.SetBool("Growl", true);
        //Joue l'animation de grognement
        Debug.Log("Graou");
        yield return new WaitForSeconds(GrowlAnimationTime);
        anims.SetBool("isGrowling", false);
        BossMoves.CanMove = true;
        BossMoves.isGoingToPreciseLocation = true;
        BossMoves.Target = Spawn.position;
        music.SwitchMusicPart(3);
    }

    public IEnumerator SecondAndThirdPhaseSwitch2()
    {
        //Apparition bouclier + grognement
        anims.SetBool("Growl", true);
        if (CurrentPhase == 2)
        {
            Debug.Log("Graou + shield");
            BossHealth.HaveShield = true;
        }
        BossMoves.CanMove = false;
        yield return new WaitForSeconds(GrowlAnimationTime);
        anims.SetBool("isGrowling", false);
        if (CurrentPhase == 2) BossMoves.CanMove = true;
        Debug.Log("Phase " + CurrentPhase);
        if (CurrentPhase == 3) BossAttacks.StartCoroutine("RayPhase");
        BossMoves.FollowPlayer = true;
        BossMoves.Target = BossMoves.TargetGO.transform.position;
        IsInSwitchPhase = false;
        BossAttacks.EndAttack();
        BossAttacks.enabled = true;
        
        CamScript.CancelCamFocus();
        canAttack = true;
    }

    public IEnumerator End()
    {
        //Bullshit du boss qui donne envie de chialax parceque ce jeu était trop émotionnellement rude et que cela va créer un manque chez le joueur qui va pas pouvoir dormir pendant des nuits et va chercher à aller aider Ryn en allant dans le monde des esprits comme ces débiles qui foncent dans les poteaux de toutes les gares pour aller à Poudlard.
        Debug.Log("Adieu");
        //Shader de dissolve
        yield return new WaitForSeconds(1); //mettre le temps de dissolve

        Debug.Log("C'est la fin.");

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
