using UnityEngine;

[System.Serializable]
public struct DummyItem
{
    [SerializeField] private int _targetTraceCount;

    #region "Properties"
    public int TargetTraceCount => _targetTraceCount;
    #endregion
}
