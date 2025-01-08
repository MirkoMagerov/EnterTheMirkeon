
using UnityEngine;

public interface IInteractuable
{
    public abstract void SetOutline(bool enable);
    public abstract void OnTriggerEnter2D(Collider2D other);
    public abstract void OnTriggerExit2D(Collider2D other);
}
