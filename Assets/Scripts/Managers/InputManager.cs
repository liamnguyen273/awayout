using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

public class InputManager : Singleton<InputManager>
{
    public Joystick movementJoystick;
    public ButtonInputUI shootingInput;
    public ButtonInputUI buttonInputUISkill0;
    public ButtonInputUI buttonInputUISkill1;
    public ButtonInputUI buttonInputUISkill2;
    public ButtonInputUI buttonInputUISkill3;

    public void HideUI()
    {
        movementJoystick.gameObject.SetActive(false);
        shootingInput.gameObject.SetActive(false);
        buttonInputUISkill0.gameObject.SetActive(false);
        buttonInputUISkill1.gameObject.SetActive(false);
        buttonInputUISkill2.gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        movementJoystick.gameObject.SetActive(true);
        shootingInput.gameObject.SetActive(true);
        buttonInputUISkill0.gameObject.SetActive(true);
        buttonInputUISkill1.gameObject.SetActive(true);
        buttonInputUISkill2.gameObject.SetActive(true);
    }
}
