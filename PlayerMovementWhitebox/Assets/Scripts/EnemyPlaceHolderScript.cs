using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaceHolderScript : MonoBehaviour
{
    // Not Meant To Have Functionality
    // Ingnore This Script It Is Only For Use As A Place Holder Script
    public enum EnemyType { Squirrle, Armadillo, Spiderling, Spider, Tree}
    EnemyType _type;
    public EnemyType type
    {
        get { return _type; }
        private set { _type = value; }
    }

}
