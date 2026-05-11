using System;
using UnityEngine;

public class LockControl : MonoBehaviour
{
    public static event Action UnLocked = delegate { };
    
    private int[] _result;
    private int[] _correctCombination;

    private void Start()
    {
        _result = new int[] { 0, 0, 0 };
        
        _correctCombination = new int[] { 3, 7, 9 };
    }

    private void CheckResults(string wheelName, int number)
    {
        switch (wheelName)
        {
            case "NumberWheel_1": _result[0] = number; break;
            case "NumberWheel_2": _result[1] = number; break;
            case "NumberWheel_3": _result[2] = number; break;
        }

        if (_result[0] == _correctCombination[0] && 
            _result[1] == _correctCombination[1] && 
            _result[2] == _correctCombination[2])
        {
            UnLocked();
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        RotateWheel.Rotated += CheckResults;
    }

    private void OnDisable()
    {
        RotateWheel.Rotated -= CheckResults;
    }
}