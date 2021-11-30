using UnityEngine;

class FlagNavigator : MonoBehaviour
{
    private FlagComponent _selectedFlag;

    public void Activate()
    {
        if (_selectedFlag == null) return;
        _selectedFlag.Activate();
    }

    public void SelectNext()
    {
        if (_selectedFlag == null || _selectedFlag.NextFlag == null) return;
        _selectedFlag = _selectedFlag.NextFlag;
        _selectedFlag.Select();
    }

    public void SelectPrevious()
    {
        if (_selectedFlag == null || _selectedFlag.PreviousFlag == null) return;
        _selectedFlag = _selectedFlag.PreviousFlag;
        _selectedFlag.Select();
    }
}
