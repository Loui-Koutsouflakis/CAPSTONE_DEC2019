//Luke F 08/06

//kyle E added more button functionality for the Xbox Controller
//Kyle E added show HUD from the Hudmanageer script.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Player Scripts/Input Handler", 0)]
public class PlayerInputHandler : MonoBehaviour
{
private bool isToggled = false;

    //we can parent this to the player or have it as a seperate game object. Need to consider that the controls can be changed before a player
    //exists
    public PlayerController playerController;
    public float horizontal;
    public float vertical;
    #region XBox Controller

    public Command _AButton = new Command();
    public Command _BButton = new Command();
    public Command _XButton = new Command();
    public Command _YBUtton = new Command();

    public Command _BackButton = new Command();

    public Command _LeftBumperDown = new Command();
    public Command _LeftBumperUp = new Command();

    public Command _RightBumperDown = new Command();
    public Command _RightBumperUp = new Command();

    public Command _LeftTriggerDown = new Command();
    public Command _LeftTriggerUp = new Command();

    public Command _RightTriggerDown = new Command();
    public Command _RightTriggerUp = new Command();
    public Command _RightTrigger = new Command();
    
    //etc...
    #endregion

    //This will be the fuction that Ben or whomever can use to change the buttons in the main menu
    public Command SetButton(Command action)
    {
        return action;
    }

    private void InitializeDefaultControls()
    {
        //mapping the defaultControls
        JumpCommand jump = new JumpCommand();
        _AButton = SetButton(jump);

        InitCrouchCommand crouch = new InitCrouchCommand();
        _LeftTriggerDown = SetButton(crouch);
        //_RightBumperDown = SetButton(crouch);
        DeCrouchCommand deCrouch = new DeCrouchCommand();
        _LeftTriggerUp = SetButton(deCrouch);
        //_RightBumperUp = SetButton(deCrouch);

        InitiateGrappleCommand grap = new InitiateGrappleCommand();
        _RightBumperDown = SetButton(grap);

        DeatchGrappleCommand dGrap = new DeatchGrappleCommand();
        _RightBumperUp = SetButton(dGrap);

        SpeedUpGrappleCommand speedUp = new SpeedUpGrappleCommand();
        _RightTrigger = SetButton(speedUp);

        HudShow h_Show = new HudShow();
        _BackButton = SetButton(h_Show);
    }

    private void Update()
    {

        horizontal = Input.GetAxis("HorizontalJoy") + Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("VerticalJoy") + Input.GetAxis("Vertical");

        if (Input.GetButtonDown("AButton"))
            _AButton.Execute(playerController);

        #region Left and Right Bumpers
        if (Input.GetButtonDown("RightBumper"))
            _RightBumperDown.Execute(playerController);

        if (Input.GetButtonUp("RightBumper"))
            _RightBumperUp.Execute(playerController);

        if (Input.GetButtonDown("LeftBumper"))//p for testing
            _LeftBumperDown.Execute(playerController);

        if (Input.GetButtonUp("LeftBumper"))
            _LeftBumperUp.Execute(playerController);
        #endregion

        #region Left Trigger
        if (Input.GetAxisRaw("LeftTrigger") != 0)
            if (!isToggled)
            {
                isToggled = true;
                _LeftTriggerDown.Execute(playerController);
            }
        if (Input.GetAxisRaw("LeftTrigger") == 0)
            if (isToggled)
            {
                isToggled = false;
                _LeftTriggerUp.Execute(playerController);
            }

        if (Input.GetKeyDown(KeyCode.LeftShift))
            _LeftTriggerDown.Execute(playerController);
        if (Input.GetKeyUp(KeyCode.LeftShift))
            _LeftTriggerUp.Execute(playerController);
        #endregion

        #region Right Trigger
        if (Input.GetAxis("RightTrigger") != 0)
            _RightTrigger.Execute(playerController);

        if (Input.GetButtonDown("Back"))
            _BackButton.Execute(playerController);
        #endregion

        ////for debugging-- pause with tab
        if (Input.GetKeyDown(KeyCode.Tab))
            Debug.Break();
    }

    private void Start()
    {
        InitializeDefaultControls();
    }
}


public class Command
{
    public virtual void Execute(PlayerController playCont) { }
}

public class HudShow : Command {
    public override void Execute(PlayerController playCont)
    {
        playCont.ShowHud();
    }
}
public class JumpCommand : Command
{
    public override void Execute(PlayerController playCont)
    {
        playCont.Jump();
    }
}

public class InitiateGrappleCommand : Command
{
    public override void Execute(PlayerController playCont)
    {
        playCont.Grapple();
    }
}

public class DeatchGrappleCommand : Command
{
    public override void Execute(PlayerController playCont)
    {
        playCont.DetatchGrapple();
    }
}

public class InitCrouchCommand : Command
{
    public override void Execute(PlayerController playCont)
    {
        playCont.Crouch();
    }
}

public class DeCrouchCommand : Command
{
    public override void Execute(PlayerController playCont)
    {
        playCont.deCrouch();
    }
}

public class SpeedUpGrappleCommand : Command
{
    public override void Execute(PlayerController playCont)
    {
        playCont.SpeedUp();
    }
}