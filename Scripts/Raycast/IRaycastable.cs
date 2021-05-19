using UnityEngine;

namespace RPG.Raycast
{
    public interface IRaycastable
    {
        CursorType GetCursorType();
        bool HandleRaycast(GameObject callingController);
    }
}