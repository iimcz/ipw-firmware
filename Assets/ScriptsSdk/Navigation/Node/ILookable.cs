using UnityEngine;

public interface ILookable
{
    bool CanActivate();
    void Activate();
    GameObject GameObject();
    Vector3 LookPosition();
}
