using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ToonKids
{

    public class animatorshow : MonoBehaviour
    {
        public GameObject[] Boys;
        public GameObject[] Girls;
        string[] animations;
        GameObject RandomBoy;
        GameObject RandomGirl;
        int animN;
        int set = 0;        
        bool rootON;
        public Texture[] texts;
        public GUIStyle newGUIStyle;
        public bool showUI;
        float Cturn;

        void Start()
        {
            if (transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);
            rootON = false;
            animN = 0;
            animations = new string[30] { "walk1","walk2","walkbackwards","strafeR", "strafeL", "stairsUP" , "stairsDOWN", "run", "runR", "runL", "runbackwards", "runstrafeR",
                                                    "runstrafeL", "runINOUT", "sprint", "brake", "runturn180", "jump", "runjumpIN","freefall","turnR45","turnR90","turnL45","turnL90",
                                                    "turn180","hitforward","fallforwardIN","fallbackwardsIN","crouchIN","pushIN" };

            RandomBoy = Instantiate(Boys[Random.Range(0, 5)]);
            RandomGirl = Instantiate(Girls[Random.Range(0, 5)]);
            RandomBoy.transform.position += transform.right * 0.2f;
            RandomGirl.transform.position += transform.right * -0.2f;


            RandomBoy.GetComponent<TKBoyPrefabMaker>().Getready();
            RandomBoy.GetComponent<TKBoyPrefabMaker>().Randomize();
            RandomGirl.GetComponent<TKGirlPrefabMaker>().Getready();
            RandomGirl.GetComponent<TKGirlPrefabMaker>().Randomize();


            RandomBoy.GetComponent<Animator>().applyRootMotion = false;
            RandomGirl.GetComponent<Animator>().applyRootMotion = false;

            RandomBoy.GetComponent<Playanimation>().enabled = false;
            RandomGirl.GetComponent<Playanimation>().enabled = false;

            RandomBoy.GetComponent<Animator>().Play("TK_" + animations[0]);
            RandomGirl.GetComponent<Animator>().Play("TK_" + animations[0]);

        }

        void Update()
        {
            if (Input.GetKeyDown("w")) changeset();

            if (Input.GetKeyDown("d"))
            {
                changecharacter();
                animN++;
                changeanimation();
            }
            if (Input.GetKeyDown("a"))
            {
                changecharacter();
                animN--;
                changeanimation();
            }
            if (Input.GetKeyDown("space")) changecharacter();
            if (Input.GetKeyDown("r")) activeroot();
            if (Input.GetKeyDown("x")) showUI = !showUI;
            if (Input.GetKeyDown("left")) { Cturn += 90; turncharacter(); }
            if (Input.GetKeyDown("right")) { Cturn -= 90; turncharacter(); }
        }

        void changeanimation()
        {
            if (animN > animations.Length - 1) animN = 0;
            else if (animN < 0) animN = animations.Length -1 ;

            RandomBoy.GetComponent<Playanimation>().enabled = false;
            RandomGirl.GetComponent<Playanimation>().enabled = false;
            RandomBoy.GetComponent<Animator>().Play("TK_" + animations[animN]);
            RandomGirl.GetComponent<Animator>().Play("TK_" + animations[animN]);
        }

        void changeset()
        {
            set++; if (set > 3) set = 0;
            if (set == 0) animations = new string[30] { "walk1","walk2","walkbackwards","strafeR", "strafeL", "stairsUP" , "stairsDOWN", "run", "runR", "runL", "runbackwards", "runstrafeR", "runstrafeL", "runINOUT", "sprint", "brake", "runturn180", "jump", "runjumpIN", "freefall", "turnR45","turnR90", "turnL45", "turnL90", "turn180", "hitforward", "fallforwardIN", "fallbackwardsIN", "crouchIN", "pushIN" };
            if (set == 1) animations = new string[13] { "idle1","idle2","idle3","idle4", "idle5", "idlehappy" , "idlesad", "idleafraid", "idleangry", "idleamazed", "idleembarrased", "idletired","idledizzy" };
            if (set == 2) animations = new string[18] { "talk1","talk2","clap","wave", "salute1", "salute2" , "laugh", "cry", "telloff", "scream", "sneeze", "grabUP", "grabDOWN", "victory1",
                                                    "victory2", "defeat1", "defeat2","throwcatch" };
            if (set == 3) animations = new string[5] { "lookback", "sitdownINOUT", "sitidle1", "sitidle2", "sitidle3" };
            
            animN = 0;
            changecharacter();
            changeanimation();
        }

        void changecharacter()
        {
            Destroy(RandomBoy);
            Destroy(RandomGirl);

            RandomBoy = Instantiate(Boys[Random.Range(0, 5)]);
            RandomGirl = Instantiate(Girls[Random.Range(0, 5)]);
            RandomBoy.transform.position += transform.right * 0.2f;
            RandomGirl.transform.position += transform.right * -0.2f;

            RandomBoy.GetComponent<TKBoyPrefabMaker>().Getready();
            RandomBoy.GetComponent<TKBoyPrefabMaker>().Randomize();

            RandomGirl.GetComponent<TKGirlPrefabMaker>().Getready();
            RandomGirl.GetComponent<TKGirlPrefabMaker>().Randomize();

            RandomBoy.GetComponent<Animator>().applyRootMotion = rootON;
            RandomGirl.GetComponent<Animator>().applyRootMotion = rootON;

            RandomBoy.GetComponent<Playanimation>().enabled = false;
            RandomGirl.GetComponent<Playanimation>().enabled = false;
            RandomBoy.GetComponent<Animator>().Play("TK_" + animations[animN]);
            RandomGirl.GetComponent<Animator>().Play("TK_" + animations[animN]);

            turncharacter();
        }

        void turncharacter()
        {
            RandomBoy.transform.rotation = Quaternion.Euler(new Vector3(0f, Cturn, 0f));
            RandomGirl.transform.rotation = Quaternion.Euler(new Vector3(0f, Cturn, 0f));
        }


        void activeroot()
        {
            rootON = !rootON;
            RandomBoy.GetComponent<Animator>().applyRootMotion = rootON;
            RandomGirl.GetComponent<Animator>().applyRootMotion = rootON;
        }

        void OnGUI()
        {
            if (showUI)
            {
                GUI.Label(new Rect(1300, 40, 300, 300), texts[0]);
                //GUI.Label(new Rect(320, 120, 256, 256), animations[animN], newGUIStyle);
                GUI.Label(new Rect(320, -100, 256, 256), animations[animN], newGUIStyle);

                if (set == 0) GUI.Label(new Rect(300, 40, 256, 128), texts[1]);
                if (set == 1) GUI.Label(new Rect(300, 40, 256, 128), texts[2]);
                if (set == 2) GUI.Label(new Rect(300, 40, 256, 128), texts[3]);
                if (set == 3) GUI.Label(new Rect(300, 40, 256, 128), texts[4]);
                if (set == 4) GUI.Label(new Rect(300, 40, 256, 128), texts[5]);
            }
        }
    }
}