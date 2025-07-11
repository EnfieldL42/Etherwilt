using UnityEngine;
using UnityEngine.Animations.Rigging;

public class aimreset : MonoBehaviour
{
    public RigBuilder[] rig; // Assign in Inspector or via script
    public bool changetarget = false;

    void Update()
    {
        if(changetarget)
        {
            changetarget = false;
            SetAimTarget();
        }
    }

    public void SetAimTarget()
    {
        foreach (var rigBuilder in rig)
        {
            if (rigBuilder != null)
                rigBuilder.Build();
        }
    }
}
