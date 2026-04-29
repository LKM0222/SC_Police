using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 플레이어의 이동을 담당하는 스크립트입니다.
// 조이스틱을 활용해, 플레이어의 이동을 구현합니다.
// 플레이어가 이동할 때, 필요한 애니메이션도 출력합니다.

public class PlayerController : MonoBehaviour, IPointerUpHandler, IDragHandler, IPointerDownHandler
{
    [Header("JoyStick")]
    [Tooltip("JoyStick의 배경입니다.")]
    [SerializeField] private RectTransform joystick;

    [Tooltip("Joystick 안에 움직이는 레버입니다.")]
    [SerializeField] private RectTransform lever;

    [Tooltip("현재 터치 포인트를 알려주는 포인트입니다.")]
    [SerializeField] GameObject point;

    [Tooltip("레버가 배경의 얼마만큼까지 나갈 수 있는지 정하는 변수입니다.")]
    [SerializeField, Range(10f, 150f)] private float leverRange;

    [Header("Debug")]
    [Tooltip("현재 입력된 Vector 값입니다.")]
    [SerializeField] private Vector2 inputVector;

    [Header("Player")]
    [SerializeField] Player player;

    #region Life Cycle
    void Update()
    {
        if (GameManager.Instance.isInput && GameManager.Instance.canInput)
        {
            InputControlVector();
        }
    }
    #endregion

    #region Interface
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!GameManager.Instance.canInput) return; // 움직일 수 없는 상태라면 비활성화

        joystick.position = eventData.position;
        joystick.gameObject.SetActive(true);
        ContolJoystickLever(eventData);
        GameManager.Instance.isInput = true;
        point.SetActive(true);
    }
    public void OnDrag(PointerEventData eventData) // 드래그 중
    {
        if (!GameManager.Instance.canInput) return; // 움직일 수 없는 상태라면 비활성화
        ContolJoystickLever(eventData);
        point.transform.position = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        lever.anchoredPosition = Vector2.zero;
        joystick.gameObject.SetActive(false);
        GameManager.Instance.isInput = false;
        point.SetActive(false);
    }
    #endregion

    #region Method
    public void ContolJoystickLever(PointerEventData eventData)
    {
        if (!GameManager.Instance.canInput) return; // 움직일수 없는 상태라면 종료

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystick, eventData.position, eventData.pressEventCamera, out localPoint);

        Vector2 clamped = Vector2.ClampMagnitude(localPoint, leverRange);
        lever.anchoredPosition = clamped;
        inputVector = clamped / leverRange;
    }

    private void InputControlVector()
    {
        //입력값 전달.
        player.Move(inputVector);
        if (inputVector != Vector2.zero)
        {
            // player.charAnim.SetBool(player.charState_Walk, true);
        }
    }
    #endregion
}