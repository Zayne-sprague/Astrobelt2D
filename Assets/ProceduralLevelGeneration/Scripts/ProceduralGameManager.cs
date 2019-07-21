using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGameManager : MonoBehaviour
{

    [SerializeField] LevelCreator lvlCreator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void level_complete()
    {
        lvlCreator.createNewRoom();
        return;
    }
}
