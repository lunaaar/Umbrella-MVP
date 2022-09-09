using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerController", menuName = "InputController/PlayerController")]
public class PlayerController : InputController
{
    public override bool RetrieveJumpInput()
    {
        return Input.GetButtonDown("Jump");

    }

    public override float RetrieveMoveInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public override bool RetrieveSwingInput()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }

    public override bool RetrieveGlideInput()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }
}
