using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ToonKids
{
    public class Spawner : MonoBehaviour
    {

        public GameObject[] characters;
        float randomTime;
        float timeCounter;
        public float deviation;
        int coin;



        void Update()
        {
            if (timeCounter > randomTime)
            {
                coin = Random.Range(0, characters.Length);
                GameObject newcharacter = Instantiate(characters[coin], transform.position + (transform.right * Random.Range(-1f, 1f)), transform.rotation * Quaternion.Euler(Vector3.up * Random.Range(-deviation, deviation)));
                if (newcharacter.GetComponent<TKGirlPrefabMaker>()!= null)
                {
                    newcharacter.GetComponent<TKGirlPrefabMaker>().Getready();
                    newcharacter.GetComponent<TKGirlPrefabMaker>().Randomize();
                    newcharacter.GetComponent<Animator>().applyRootMotion = true;
                    newcharacter.GetComponent<Animator>().Play("TK_walk1");
                }
                else
                {
                    newcharacter.GetComponent<TKBoyPrefabMaker>().Getready();
                    newcharacter.GetComponent<TKBoyPrefabMaker>().Randomize();
                    newcharacter.GetComponent<Animator>().applyRootMotion = true;
                    newcharacter.GetComponent<Animator>().Play("TK_walk1");
                }

                randomTime = Random.Range(1, 4);
                timeCounter = 0f;
            }
            timeCounter += Time.deltaTime;
        }
    }
}
