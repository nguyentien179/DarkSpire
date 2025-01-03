using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItem : MonoBehaviour
{
    protected CharacterStat player;
    public PassiveItemScriptableObject passiveItemData;

    protected virtual void ApplyModifier()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<CharacterStat>();
        ApplyModifier();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
