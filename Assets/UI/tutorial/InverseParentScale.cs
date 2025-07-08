using UnityEngine;

public class InverseParentScale : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 parentScale = transform.parent.lossyScale;
        transform.localScale = new Vector3(
            1 / parentScale.x,
            1 / parentScale.y,
            1
        );
    }
}
