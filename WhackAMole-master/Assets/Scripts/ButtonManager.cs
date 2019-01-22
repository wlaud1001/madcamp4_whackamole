using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{

    public ScoreManager scoreManager;
    public TimeManager timeManager;

    //When you add a Tooltip, and mouse over this var in the Editor, it will show the assigned text
    [Tooltip("connect all the game buttons to this")]
    public GameObject[] b_moles;//By giving the '[]' assingment to the GameObject var we have created an array! Arrays are one type of Data structure
    public GameObject[] b_create;

    public Sprite dead;
    public Sprite alive;
    public Sprite timer;
    public Sprite bomb;
    public Sprite explosion;
    public Sprite star;
    public AudioClip a_whack;
    public AudioClip a_bomb;
    public AudioClip a_timer;
    private AudioSource as_bomb;
    private AudioSource as_whack;
    private AudioSource as_timer;


    Vector3[] moles_pos = new Vector3[9];
    private Sprite[] sprites = new Sprite[2];


    private int[] active = new int[9];


    //let's start this out with .5 seconds at first, then it will get randomly set to a new time.
    float randTime = .5f;

    //by setting this public we can modify it in unity and change it at runtime too so we can see what feels right.
    // private float hideTime = 0.45f;
    private float hideTime = 0.7f;


    // Use this for initialization
    void Start()
    {
        this.as_bomb = this.gameObject.AddComponent<AudioSource>();
        this.as_whack = this.gameObject.AddComponent<AudioSource>();
        this.as_timer = this.gameObject.AddComponent<AudioSource>();

        this.as_bomb.clip = this.a_bomb;
        this.as_whack.clip = this.a_whack;
        this.as_timer.clip = this.a_timer;

        this.as_bomb.loop = false;
        this.as_whack.loop = false;
        this.as_timer.loop = false;


        //Your first "For Loop"! Here we iterate thru our 'buttons' array and set each one to inactive at the start of the game.
        for (int i = 0; i < b_moles.Length; i++)
        {

            b_moles[i].transform.position = new Vector3(b_moles[i].transform.position.x, b_create[i].transform.position.y, b_moles[i].transform.position.z);
            moles_pos[i] = b_moles[i].transform.position;
            //b_moles[i].SetActive(false);
            active[i] = 0;

        }
        sprites[0] = alive;
        sprites[1] = dead;


        //we also set a CoRoutine, which is a delayed event to start showing our random buttons which starts the game
        StartCoroutine(ShowButton());
    }


    void Update()
    {


        //we grab the button at the numbered postion "randBtn" in the "buttons[]" array and see if it is active

        for (int i = 0; i < 9; i++)
        {
            /*
            if (b_moles[i].transform.position.y < b_create[i].transform.position.y)
            {
                b_moles[i].SetActive(false);
                active[i] = 0;
            }
            */


            if (active[i] == 0)
                continue;
            else if (active[i] == 1)
            {
                float up = b_moles[i].transform.position.y;
                Transform mole = b_moles[i].transform;
                //Debug.Log(b_moles[i].transform.position.y);

                up += 150f * Time.deltaTime;
                b_moles[i].transform.position = new Vector3(mole.position.x, up, mole.position.z);

            }

            else if (active[i] == 2)
            {

                StartCoroutine(GoDown(i));

            }


            //Debug.Log(b_moles[i].activeInHierarchy);
            //Debug.Log("!!");

        }

    }

    private IEnumerator GoDown(int index)
    {
        if (b_moles[index].transform.position.y > b_create[index].transform.position.y)
        {

            float down = b_moles[index].transform.position.y;
            Transform mole = b_moles[index].transform;
            //Debug.Log(b_moles[i].transform.position.y);
            yield return new WaitForSeconds(0);
            down -= 170f * Time.deltaTime;
            b_moles[index].transform.position = new Vector3(mole.position.x, down, mole.position.z);

        }
        else
        {
            active[index] = 0;
            b_moles[index].GetComponent<Image>().sprite = alive;
            //CmdAlive(b_moles[index]);

        }
    }
    

    //This is what we will hook up to the buttons
    public void PressedBtn_mole()
    {
        //allow the player to score as long as the game is still going
        if (timeManager.gameOver == false)
        {

            //Grab a reference to the object that we pressed so we can do some stuff with it
            GameObject myMole = EventSystem.current.currentSelectedGameObject;
            string name = myMole.name;
            int index = int.Parse(name);

            if (active[index] == 0)
                return;

            if (myMole.GetComponent<Image>().sprite == dead || myMole.GetComponent<Image>().sprite == explosion || myMole.GetComponent<Image>().sprite == star)
                return;

            if (myMole.GetComponent<Image>().sprite == alive)
            { 
                myMole.GetComponent<Image>().sprite = dead;
                scoreManager.UpdateScore(10, true);
                this.as_whack.Play();
            }
            else if(myMole.GetComponent<Image>().sprite == bomb)
            {
                myMole.GetComponent<Image>().sprite = explosion;
                BombClicked();
                this.as_bomb.Play();
            }
            else if(myMole.GetComponent<Image>().sprite == timer)
            {
                myMole.GetComponent<Image>().sprite = star;
                timeManager.PlusTime();
                this.as_timer.Play();
            }


            //change the color to red & set text so that we know we hit it
            //myMole.GetComponentInChildren<Text>().text = "HIT!";
            //myMole.GetComponentInChildren<Image>().color = Color.red;

            active[index] = 2;

        }

    }


    /*
    public void PressedBtn_create()
    {


        if (timeManager.gameOver == false)
        {

            GameObject myCreate = EventSystem.current.currentSelectedGameObject;


            string name = myCreate.name;
            int index = int.Parse(name) - 10;


            if (active[index] == 0)
            {

                b_moles[index].transform.position = moles_pos[index];

                active[index] = 1;
                //b_moles[index].SetActive(true);
                StartCoroutine(HideButton(b_moles[index], randTime));
            }

        }
    }
    */

    //this is on repeat, until the game is over
    IEnumerator ShowButton()
    {
        //     yield return new WaitForSeconds(0);
        yield return new WaitForSeconds(randTime);
        //allow the coroutine to happen as long as the game is still going
        if (timeManager.gameOver == false)
        {
            int selectItem = Random.Range(0, 9);
            int index = Random.Range(0, b_moles.Length);
            if (active[index] == 0)
            {
                switch (selectItem)
                {
                    case 0:// 폭탄
                        Debug.Log("폭탄 call");
                        b_moles[index].GetComponent<Image>().sprite = bomb;
                        break;
                    case 1://타이머
                        Debug.Log("타이머 call");
                        b_moles[index].GetComponent<Image>().sprite = timer;
                        break;
                    default: // 두더지
                        Debug.Log("두더지 call");
                        b_moles[index].GetComponent<Image>().sprite = alive;
                        break;
                }
                b_moles[index].transform.position = moles_pos[index];
                active[index] = 1;
                StartCoroutine(HideButton(b_moles[index], randTime));
            }

            StartCoroutine(ShowButton());
        }


    }
  
    /// <summary>
    /// Hides the button after a delay from being Shown
    /// Called from "ShowButton" coroutine
    /// </summary>
    /// <returns>The button.</returns>
    /// <param name="myMole">My button.</param>
    IEnumerator HideButton(GameObject myMole, float delay)
    {
        yield return new WaitForSeconds(hideTime);



        //let's make sure it wasn't hit first because we don't want two coroutines on the same button
        if (myMole.GetComponent<Image>().sprite == alive)//green, then it hasn't been hit, so we can hide it
        {
            //set to inactive so that it can get called again!
            //player didn't hit this in time, so we reduce their score
            scoreManager.UpdateScore(5, false);
            string name = myMole.name;
            int index = int.Parse(name);
            active[index] = 2;
            /*
            if(b_moles[index].transform.position.y > b_create[index].transform.position.y)
            {

                //continue;
            }
            */
            //yield return new WaitForSeconds(hideTime+randTime);
            //myMole.gameObject.SetActive(false);

        }
        else if(myMole.GetComponent<Image>().sprite == timer || myMole.GetComponent<Image>().sprite == bomb)
        {
            string name = myMole.name;
            int index = int.Parse(name);
            active[index] = 2;
        }




    }


    public void BombClicked()
    {
        //allow the player to score as long as the game is still going
        if (timeManager.gameOver == false)
        {
            scoreManager.DowndateScore();
        }
    }      
}
