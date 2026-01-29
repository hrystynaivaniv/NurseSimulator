using UnityEngine;

public class BaseUIElement : MonoBehaviour, IUIElement
{
    public virtual void Open()
    {
        return;
    }

    public virtual void Close()
    {
        return;
    }

    public virtual void Disable()
    {
        gameObject.SetActive(false);
        return;
    }

    public virtual void Enable()
    {
        gameObject.SetActive(true);
        return;
    }
}
