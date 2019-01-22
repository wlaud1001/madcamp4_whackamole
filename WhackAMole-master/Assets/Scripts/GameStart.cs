using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{

    public Image mole;
    public Image pipe;
    private int active; //active가 1이면 올라가고 2면 내려감
    Transform t_mole;
    Transform t_pipe;

    // Start is called before the first frame update
    void Start()
    {

        t_mole = mole.transform;
        t_pipe = pipe.transform;

        t_mole.position = new Vector3(t_mole.position.x, t_pipe.position.y + 180, t_mole.position.z);
        active = 1; 

    }

    // Update is called once per frame
    void Update()
    {

        if (active == 1)
        {
            //GoUp
            float up = t_mole.position.y;
            //Debug.Log(b_moles[i].transform.position.y);

            up += 100f * Time.deltaTime;
            t_mole.position = new Vector3(t_mole.position.x, up, t_mole.position.z);

        }

        else if (active == 2)
        {
            float down = t_mole.position.y;

            //Debug.Log(b_moles[i].transform.position.y);

            down -= 100f * Time.deltaTime;
            t_mole.position = new Vector3(t_mole.position.x, down, t_mole.position.z);
        }

        if (t_mole.position.y - t_pipe.position.y > 180)
            active = 2;

        if(t_mole.position.y < t_pipe.position.y +20)
        {
            active = 1;
        }
    }

    /*
    private IEnumerator GoUp()
    {

        float up = t_mole.position.y;
        //Debug.Log(b_moles[i].transform.position.y);
        yield return new WaitForSeconds(0);
        up += 100f * Time.deltaTime;
        t_mole.position = new Vector3(t_mole.position.x, up, t_mole.position.z);

    }


    private IEnumerator GoDown()
    {

        float down = t_mole.position.y;

        //Debug.Log(b_moles[i].transform.position.y);
        yield return new WaitForSeconds(0);
        down -= 140f * Time.deltaTime;
        t_mole.position = new Vector3(t_mole.position.x, down, t_mole.position.z);

    }
    */
    public void startPressed()
    {

        //load back to Game scene when tapped
        SceneManager.LoadSceneAsync(GameScenes.unetpractice.ToString());



    }

}
