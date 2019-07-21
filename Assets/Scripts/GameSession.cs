using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] public int score;
    [SerializeField] int number_of_rendered_rooms = 5;
    [SerializeField] AstroidRoom room;

    private AstroidRoom[] AstroidRooms;
    private int last_room;

    private void Awake()
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = score.ToString();
        AstroidRooms = new AstroidRoom[number_of_rendered_rooms];

        // for (int i = 0; i < number_of_rendered_rooms; i++) {
        //    CreateNewRoom();
        // }
        
    }

    public void AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        scoreText.text = score.ToString();
    }

    public void ProcessPlayerDeath()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    // --- ROOM CREATION --- //

    public void CreateNewRoom()
    {
        print("Create a new room");
        if (!AstroidRooms[0])
        {
            initialRoom();
            last_room = 0;
            return;
        }
        print(last_room);
        AstroidRoom prevRoom = AstroidRooms[last_room];
        last_room++;
        if (last_room >= number_of_rendered_rooms)
        {
            last_room = 0;
        }

        int[] sides = PlanRoomSides(prevRoom);
        Vector3 origin = findRoomOrigin(prevRoom, System.Array.IndexOf(prevRoom.side, 2));

        room.x_origin = origin.x;
        room.y_origin = origin.y;
        room.side = sides;
        print(origin);
        AstroidRoom new_room = Instantiate(room, origin, Quaternion.identity); ;


        if (AstroidRooms[last_room])
        {

            destroyRoom(AstroidRooms[last_room]);
        }

        AstroidRooms[last_room] = new_room;


    }

    private void destroyRoom(AstroidRoom room)
    {
        for (int i = 0; i < room.transform.childCount; i++)
        {
            Destroy(room.transform.GetChild(i).gameObject);
        }
        Destroy(room.gameObject);
    }

    private void initialRoom()
    {
        room.x_origin = 0f;
        room.y_origin = 0f;
        room.side = PlanRoomSides(null);
        AstroidRoom new_room = Instantiate(room, new Vector3(0f, 0f, 0f), Quaternion.identity);
        AstroidRooms[0] = new_room;
    }

    private int[] PlanRoomSides(AstroidRoom prevRoom)
    {
        int[] new_room = new int[]{ 0, 0, 0, 0 }; //completely blocked off room
        int entrance = -2;

        //make the rooms line up
        if (prevRoom)
        {
            int where_is_entrence = System.Array.IndexOf(prevRoom.side, 2);

            if (where_is_entrence > -1)
            {
                entrance = where_is_entrence % 2 == 0 ? where_is_entrence + 1 : where_is_entrence - 1;
                new_room[entrance] = 1;
            }

        }

        int exit = -1 ;
        while (exit == entrance || exit == -1)
        {
            exit = Random.Range(0, 4);
        }
        new_room[exit] = 2;

        return new_room;

    }

    private Vector3 findRoomOrigin(AstroidRoom prevRoom, int entrance)
    {
        Vector3 origin = new Vector3(prevRoom.transform.position.x, prevRoom.transform.position.y, 0f);

        float dist = (prevRoom.height_of_wall + prevRoom.width_of_wall);
        if (entrance < 2)
        {
            //top bottom
            origin.y = entrance == 0 ? origin.y + dist : origin.y - dist;
        }
        else
        {
            //left right
            origin.x = entrance == 2 ? origin.x - (dist - prevRoom.width_of_wall) : origin.x + (dist - prevRoom.width_of_wall);
        }

        return origin;
    }
}
