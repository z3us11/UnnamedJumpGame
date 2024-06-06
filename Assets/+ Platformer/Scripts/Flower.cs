using DG.Tweening;
using Platformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public FlowerColor flowerColor;

    bool collectedFlower = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(!collectedFlower)
            {
                collision.transform.parent.GetComponent<PlayerController>().flowerCollection.OnFlowerCollected(flowerColor);
                CollectFlower();
                collectedFlower = true;
            }
        }
    }

    private void CollectFlower()
    {
        //transform.parent = null;
        Sequence tween = DOTween.Sequence();
        tween.Append(transform.DOLocalMoveY(1.75f, 0.5f));
        tween.Append(transform.DOPunchScale(Vector3.one * 0.5f, 0.5f));
        tween.Append(transform.DOScale(0, 0.25f));
        tween.OnComplete(()=>gameObject.SetActive(false));
    }
}

public enum FlowerColor
{
    Cyan,
    Magenta,
    Yellow
}