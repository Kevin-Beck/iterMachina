using UnityEngine;

public abstract class FollowTarget : MonoBehaviour
{

    [SerializeField] public Transform target;

    void FixedUpdate()
    {
        if (target != null && (target.GetComponent<Rigidbody>() != null && !target.GetComponent<Rigidbody>().isKinematic))
            Follow(Time.deltaTime);
    }

    protected abstract void Follow(float deltaTime);

    public virtual void SetTarget(Transform newTransform)
    {
        target = newTransform;
    }
    public Transform Target { get { return this.target; } }
}
