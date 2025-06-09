using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArrowUI_anim : MonoBehaviour
{
    void Start()
    {
        // Asumimos que este script está en el objeto de la flecha (Image con RectTransform)
        RectTransform rt = GetComponent<RectTransform>();

        rt.DOAnchorPosY(rt.anchoredPosition.y + 10f, 0.5f)
          .SetLoops(-1, LoopType.Yoyo)
          .SetEase(Ease.InOutSine);
    }
}
