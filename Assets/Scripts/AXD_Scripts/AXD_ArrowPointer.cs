using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_ArrowPointer : MonoBehaviour
{
    [SerializeField]
    public GameObject arrowObject;
    public ELC_CharacterManager characterManager;
    public RectTransform rt;
    private bool isOffScreen;
    public float arrowScaleMultiplicator;
    public float borderSize;
    private Vector2 toPoint;
    private Vector2 fromPosition;
    private Vector2 direction;
    private Vector2 targetPositionScreenPoint;
    

    private void Start()
    {
        rt = arrowObject.GetComponent<RectTransform>();
        rt.localScale = new Vector2(rt.localScale.x*arrowScaleMultiplicator,rt.localScale.y*arrowScaleMultiplicator);
    }


    private void FixedUpdate()
    {
        if (characterManager.RynMove.currentCharacter)
        {
            toPoint = characterManager.spiritMove.transform.position;
            fromPosition = Camera.main.transform.position;
            
            direction = (toPoint - fromPosition).normalized;
        }
        else
        {
            toPoint = characterManager.RynGO.transform.position;
            fromPosition = Camera.main.transform.position;
            direction = (toPoint - fromPosition).normalized;
        }

        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) % 360 + 90;
        rt.localEulerAngles = new Vector3(0, 0, angle);

        targetPositionScreenPoint = Camera.main.WorldToScreenPoint(toPoint);
        isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width-borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height-borderSize;
        if (isOffScreen)
        {
            if (!arrowObject.activeSelf)
            {
                arrowObject.SetActive(true);
            }
            Vector2 cappedTargetScreenPosition = targetPositionScreenPoint;
            if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
            if (cappedTargetScreenPosition.x >= Screen.width - borderSize) cappedTargetScreenPosition.x = Screen.width - borderSize;
            if (cappedTargetScreenPosition.y <= borderSize) cappedTargetScreenPosition.y = borderSize;
            if (cappedTargetScreenPosition.y >= Screen.height - borderSize) cappedTargetScreenPosition.y = Screen.height - borderSize;

            Vector2 pointerWorldPosition = Camera.main.ScreenToWorldPoint(cappedTargetScreenPosition);
            rt.position = pointerWorldPosition;
            rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y, 0f);

        }
        else

            if (arrowObject.activeSelf)
            {
                arrowObject.SetActive(false);
            }
        }
    }

