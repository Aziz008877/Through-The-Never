using System;
using UnityEngine;

public class EnemyMaterialApplier : MonoBehaviour
{
    [SerializeField] private EnemyType _enemyType;
    [SerializeField] private Material[] _allMaterials;
    [SerializeField] private Material[] _allRangedMaterials;
    [SerializeField] private Renderer _skinRenderer;
    [SerializeField] private MeleeMobAttack _meleeMobAttack;
    [SerializeField] private RangedMobAttack _rangedMobAttack;
    private void Start()
    {
        if (_enemyType == EnemyType.Melee)
        {
            switch (_meleeMobAttack.Tier)
            {
                case MeleeMobTier.Tier1_Blue:
                    _skinRenderer.material = _allMaterials[0];
                    break;
            
                case MeleeMobTier.Tier2_Cyan:
                    _skinRenderer.material = _allMaterials[1];
                    break;
            
                case MeleeMobTier.Tier3_Green:
                    _skinRenderer.material = _allMaterials[2];
                    break;
            
                case MeleeMobTier.Tier4_Red:
                    _skinRenderer.material = _allMaterials[3];
                    break;
            }
        }
        else if (_enemyType == EnemyType.Ranged)
        {
            switch (_rangedMobAttack.Tier)
            {
                case RangedMobTier.Tier1_Green:
                    _skinRenderer.material = _allRangedMaterials[0];
                    break;
                
                case RangedMobTier.Tier2_Orange:
                    _skinRenderer.material = _allRangedMaterials[1];
                    break;
                
                case RangedMobTier.Tier3_Purple:
                    _skinRenderer.material = _allRangedMaterials[2];
                    break;
            }
        }
    }
}

public enum EnemyType
{
    Melee,
    Ranged
}
