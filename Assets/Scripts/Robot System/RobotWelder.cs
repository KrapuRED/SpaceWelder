using UnityEngine;

public class RobotWelder : Robot
{
    [Header("Robot Welder Config")]
    [SerializeField] private bool _isWelding = false;

    [Header("Rail")]
    [SerializeField] private Transform _checkRailPoint;
    [SerializeField] private LayerMask _layerRail;

    public bool IsWelding => _isWelding;

}
