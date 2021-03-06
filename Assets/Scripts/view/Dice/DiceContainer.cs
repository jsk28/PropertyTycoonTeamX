using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

///DICE IMPLEMENTATION NEEDS REFACTORING
namespace View{
    /// <summary>
    /// Extends <see cref=" MonoBehaviour"/>.<br/>
    /// Script attached to DiceContainer prefab.<br/>
    /// Script used control all dice in the dice and read the values from it.
    /// </summary>
public class DiceContainer : MonoBehaviour
{
    float move_speed = 20.0f;   // how fast dice is following cursor
    Vector3 previous_frame_pos; // parameter used to calculate dice velocity
    Dice[] dice;                // list for references to dice monobehaviour
    Vector3 init_pos;           // initial position of the container
    public Light greenLight;
    [System.NonSerialized] public bool start_roll; 
    void Awake()
    {
        init_pos = transform.position;
        dice = GetComponentsInChildren<Dice>();
        start_roll = false;
    }

    public static DiceContainer Create(Transform parent)
    {
        return Instantiate(Asset.DiceContainerPrefab,parent).GetComponent<DiceContainer>();
    }

    void OnMouseDown()
    {
        Cursor.SetCursor(Asset.GrabTextureCursor,Vector2.zero,CursorMode.Auto); // on click change cursor to 'closed hand'
        greenLight.gameObject.SetActive(false);
    }
    void OnMouseDrag()
    {   // when dragging keep recalculating velocity and move towards cursor
        previous_frame_pos = transform.position;
        Vector3 targetPos = getTargetPos();
        if(transform.position.y < 1.5f || Mathf.Abs(transform.position.x) > 18.3f || Mathf.Abs(transform.position.z) > 10.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position,Vector3.up*10.0f,move_speed * Time.smoothDeltaTime);
        } else {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, move_speed * Time.smoothDeltaTime);
        }
    }
    void OnMouseUp()
    {   // on mouse button release change cursor to 'poiniting hand'
        Cursor.SetCursor(Asset.FingerTextureCursor,Vector2.zero,CursorMode.Auto);
        foreach (Dice d in dice)    // for each dice assign velcity
        {
            d.roll((transform.position - previous_frame_pos)/Time.smoothDeltaTime);
        }
        start_roll = true;
        GetComponent<BoxCollider>().enabled = false;
        enabled = false;    // disable the container
    }

    Vector3 getTargetPos()
    {       // create a plane perpendicular to the camera's forward vector
        Plane plane = new Plane(Camera.main.transform.forward,transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    // get the ray going from camera towards cursor

        float point = 0f;
        Vector3 targetPos = transform.position;
        if(plane.Raycast(ray, out point))
        {       // get the point on the plane where cursor is pointing
            targetPos = ray.GetPoint(point);
        }
        return targetPos;
    }
    /// <summary>
    /// Resets all dice to the initial position.
    /// The dice rotation is pseudorandom.
    /// </summary>
    public void reset()
    {
        greenLight.gameObject.SetActive(true);
        transform.position = init_pos;
        foreach(Dice d in dice)
        {
            d.reset();
        }
        start_roll = false;
        GetComponent<BoxCollider>().enabled = true;
        enabled = true;
        gameObject.SetActive(true);
    }
    /// <returns> The sum of dice results.</returns>
    public int get_result()
    {
        int result = 0;
        foreach(Dice d in dice)
        {
            result += d.get_value();
        }
        return result;
    }

    /// <returns>Bool value of dice show the same value. </returns>
    public bool is_double()
    {
        int i = 0;
        int[] dice_values = new int[2];
        foreach (Dice d in dice)
        {
            dice_values[i] = d.get_value();
            i++;
        }

        return (dice_values[0] == dice_values[1]);
    }

    /// <returns> True if at least one dice is rolling.</returns>
    public bool areRolling()
    {
        foreach(Dice d in dice)
        {
            if(d.isRolling()) return true;
        }
        return false;
    }

    /// <returns>The mid-point of all the dice. </returns>
    public Vector3 position()
    {
        Vector3 point = Vector3.zero;
        foreach(Dice d in dice)
        {
            point += d.transform.position;
        }
        return point/dice.Length;
    }
    /// <returns>Distance of the dice from the mid-point. See <see cref="DiceContainer.position"/></returns>
    public float av_distance()
    {
        return (position()-dice[0].transform.position).magnitude;
    }

    /// <returns>True if any dice cooridinate 'y' is below or equal 0. See <see cref="Transform.position"/></returns>
    public bool belowBoard()
    {
        foreach(Dice d in dice)
        {
            if(d.transform.position.y <= 0) { return true; }
        }
        return false;
    }
/// <summary>
/// 'Throws' all dice with some pseudorandom initial velocity.
/// </summary>
    public void random_throw()
    {
        greenLight.gameObject.SetActive(false);
        Vector3 dice_velocity = new Vector3(Random.Range(-15f,15f),Random.Range(-1f,1f),Random.Range(-10f,10f));
        foreach (Dice d in dice)    // for each dice assign velcity
        {
            d.roll(dice_velocity);
        }
        start_roll = true;
        GetComponent<BoxCollider>().enabled = false;
        enabled = false;    // disable the container
    }
}
}
