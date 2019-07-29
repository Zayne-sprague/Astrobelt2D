using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{


    public int value;

    SpriteRenderer mySpriteRenderer;
    ProceduralGameManager game_manager;
    CapsuleCollider2D myCollider;


    // Start is called before the first frame update
    void Start()
    {

        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<CapsuleCollider2D>();
        game_manager = FindObjectOfType<ProceduralGameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (myCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            game_manager.coinPickedUp(value);
        }        

        Destroy(gameObject);
    }


}
