using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/Passive Item")]
public class PassiveItemScriptableObject : ScriptableObject
{
    [SerializeField]
    float multiplier;
    public float Multiplier { get => multiplier; private set => multiplier = value; }
    [SerializeField]
    int level;
    public int Level { get => level; private set => level = value; }
    [SerializeField]
    GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }
    [SerializeField]
    Sprite icon;
    public Sprite Icon { get => icon; private set => icon = value; }
    [SerializeField]
    string description;
    public string Description { get => description; private set => description = value; }

    [SerializeField]
    string passiveName;

    public string PassiveName { get => passiveName; private set => passiveName = value; }

}
