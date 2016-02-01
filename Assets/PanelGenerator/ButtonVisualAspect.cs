using UnityEngine;
using System.Collections;

public enum TypeButtonVisualAspect { Size1x1 = 1, Size2x1 = 2, Size1x2 = 3, Size2x2 = 4 };

public class ButtonVisualAspect : MonoBehaviour
{
    public TypeButtonVisualAspect typeButton = TypeButtonVisualAspect.Size1x1;
}
