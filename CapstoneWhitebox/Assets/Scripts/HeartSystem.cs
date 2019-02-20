using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartSystem : MonoBehaviour {
    public int StartHeart = 3;
    public int CurrentHeart;

    private int MaxHeartCap = 10;
    private int MaxHealth;
    private int HeartHits = 2;

    public Image[] HeartImage;
    public Sprite[] HeartSprite;

	// Use this for initialization
	void Start () {
        CurrentHeart = StartHeart * HeartHits;
        MaxHealth = MaxHeartCap * HeartHits;
        HealthCheck();
	}
	
    void HealthCheck()
    {
        for (int i = 0; i < MaxHeartCap; i++)
        {
            if (StartHeart <= i)
            {
                HeartImage[i].enabled = false;
            }
            else
            {
                HeartImage[i].enabled = true;
            }
        }
    }
	
    void HealthUpdate()
    {
        bool empty = false;
        int i = 0;

        foreach (Image image in HeartImage)
        {
            if (empty)
            {
                image.sprite = HeartSprite[0];
            }
            else
            {
                i++;
                if (CurrentHeart>= i * HeartHits)
                {
                    image.sprite = HeartSprite[HeartSprite.Length - 1];
                }
                else
                {
                    int CurrentHeartHits = (int)(HeartHits - (HeartHits * i - CurrentHeart));
                    int HealthPerImage = HeartHits / (HeartSprite.Length - 1);
                    int ImageIndex = CurrentHeartHits / HealthPerImage;
                    image.sprite = HeartSprite[ImageIndex];
                    empty = true;
                }
            }
        }
    }
}
