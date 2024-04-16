using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GMR : MonoBehaviour
{
    Collectible collect;
    PlayerManager playerManager;
    public TextMeshProUGUI bluePointText;

    public  int blueScore = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        collect = GetComponent<Collectible>();
        playerManager = GetComponent<PlayerManager>();
        bluePointText.text = blueScore.ToString();
    }

  

    // Update is called once per frame
    void Update()
    {
        
    }

   

    
}
