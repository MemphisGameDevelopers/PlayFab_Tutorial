using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using System;

public class Player : NetworkBehaviour
{
    NetworkVariableString username = new NetworkVariableString();

    public override void NetworkStart()
    {
        username.OnValueChanged += OnNameChange;
    }

    private void OnNameChange(string previousValue, string newValue)
    {
        //Update UI
    }
}
