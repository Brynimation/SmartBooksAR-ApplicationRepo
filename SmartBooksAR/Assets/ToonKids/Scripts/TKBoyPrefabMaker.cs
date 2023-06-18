using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[SelectionBase]

    public class TKBoyPrefabMaker : MonoBehaviour
    {
        public bool allOptions;
        int hair;
        int chest;
        int legs;
        int feet;
        int skintone;
        public bool glassesactive;
        public bool hatactive;
        GameObject GOhead;
        GameObject GOheadsimple;
        GameObject[] GOfeet;
        GameObject[] GOhair;
        GameObject[] GOchest;
        GameObject[] GOlegs;
        GameObject GOglasses;
        public Object[] MATSkins;
        public Object[] MATHairA;
        public Object[] MATHairB;
        public Object[] MATHairC;
        public Object[] MATHairD;
        public Object[] MATHairE;
        public Object[] MATHairF;
        public Object[] MATHairG;
        public Object[] MATEyes;
        public Object[] MATGlasses;
        public Object[] MATTshirt;
        public Object[] MATShirt;
        public Object[] MATSweater;
        public Object[] MATLegs;
        public Object[] MATFeetA;
        public Object[] MATFeetB;
        public Object[] MATHat;
        public Object[] MATTeeth;
        Material headskin;


        void Start()
        {
            allOptions = false;
        }

        public void Getready()
        {
            //load models
            GOhead = transform.Find("HEAD").gameObject as GameObject;
            GOheadsimple = transform.Find("HEADsimple").gameObject as GameObject;
            GOheadsimple.SetActive(false);

            GOhair = new GameObject[8];
            GOchest = new GameObject[7];
            GOlegs = new GameObject[6];
            GOfeet = new GameObject[4];

            string[] hairnames = new string[8] { "TKBHairA", "TKBHairB", "TKBHairC", "TKBHairD", "TKBHairE", "TKBHairF", "TKBHairG", "HatA" };
            string[] chestnames = new string[7] { "CHEST", "TKShirtL", "TKShirtS", "TKSweater", "TKTshirtLA", "TKTshirtLB", "TKTshirtS" };
            string[] legnames = new string[6] { "LEGS", "TKLegsLA", "TKLegsLB", "TKLegsLC", "TKLegsSA", "TKLegsSB" };
            string[] feetnames = new string[4] { "FEET", "TKFeetAL", "TKFeetAS", "TKFeetB" };
            for (int forAUX = 0; forAUX < 8; forAUX++) GOhair[forAUX] = transform.Find(hairnames[forAUX]).gameObject as GameObject;
            for (int forAUX = 0; forAUX < 7; forAUX++) GOchest[forAUX] = transform.Find(chestnames[forAUX]).gameObject as GameObject;
            for (int forAUX = 0; forAUX < 6; forAUX++) GOlegs[forAUX] = transform.Find(legnames[forAUX]).gameObject as GameObject;
            for (int forAUX = 0; forAUX < 4; forAUX++) GOfeet[forAUX] = transform.Find(feetnames[forAUX]).gameObject as GameObject;
            GOglasses = transform.Find("ROOT/TK/TK Pelvis/TK Spine/TK Spine1/TK Spine2/TK Neck/TK Head/Glasses").gameObject as GameObject;


            if (GOfeet[0].activeSelf && GOfeet[1].activeSelf && GOfeet[2].activeSelf) Randomize();
            else
            {
                for (int forAUX = 0; forAUX < GOhair.Length; forAUX++) { if (GOhair[forAUX].activeSelf) hair = forAUX; }
                while (!GOchest[chest].activeSelf) chest++;
                while (!GOlegs[legs].activeSelf) legs++;
                while (!GOfeet[feet].activeSelf) feet++;
                if (hair > 6) hatactive = true;
            }
        }
        void ResetSkin()
        {
            string[] allskins = new string[10] { "TKBoyA0", "TKBoyB0", "TKBoyC0", "TKBoyD0", "TKBoyE0", "TKGirlA0", "TKGirlB0", "TKGirlC0", "TKGirlD0", "TKGirlE0" };
            Material[] AUXmaterials;

            int materialcount = GOhead.GetComponent<Renderer>().sharedMaterials.Length;

            //ref head material
            AUXmaterials = GOhead.GetComponent<Renderer>().sharedMaterials;
            materialcount = GOhead.GetComponent<Renderer>().sharedMaterials.Length;
            for (int forAUX2 = 0; forAUX2 < materialcount; forAUX2++)
                for (int forAUX3 = 0; forAUX3 < allskins.Length; forAUX3++)
                    for (int forAUX4 = 1; forAUX4 < 6; forAUX4++)
                    {
                        if (AUXmaterials[forAUX2].name == allskins[forAUX3] + forAUX4)
                        {
                            headskin = AUXmaterials[forAUX2];
                        }
                    }
            //chest
            for (int forAUX = 0; forAUX < GOchest.Length; forAUX++)
            {
                AUXmaterials = GOchest[forAUX].GetComponent<Renderer>().sharedMaterials;
                materialcount = GOchest[forAUX].GetComponent<Renderer>().sharedMaterials.Length;
                for (int forAUX2 = 0; forAUX2 < materialcount; forAUX2++)
                    for (int forAUX3 = 0; forAUX3 < allskins.Length; forAUX3++)
                        for (int forAUX4 = 1; forAUX4 < 6; forAUX4++)
                        {
                            if (AUXmaterials[forAUX2].name == allskins[forAUX3] + forAUX4)
                            {
                                AUXmaterials[forAUX2] = headskin;
                                GOchest[forAUX].GetComponent<Renderer>().sharedMaterials = AUXmaterials;
                            }
                        }
            }
            //legs
            for (int forAUX = 0; forAUX < GOlegs.Length; forAUX++)
            {
                AUXmaterials = GOlegs[forAUX].GetComponent<Renderer>().sharedMaterials;
                materialcount = GOlegs[forAUX].GetComponent<Renderer>().sharedMaterials.Length;
                for (int forAUX2 = 0; forAUX2 < materialcount; forAUX2++)
                    for (int forAUX3 = 0; forAUX3 < allskins.Length; forAUX3++)
                        for (int forAUX4 = 1; forAUX4 < 6; forAUX4++)
                        {
                            if (AUXmaterials[forAUX2].name == allskins[forAUX3] + forAUX4)
                            {
                                AUXmaterials[forAUX2] = headskin;
                                GOlegs[forAUX].GetComponent<Renderer>().sharedMaterials = AUXmaterials;
                            }
                        }
            }
            //feet
            for (int forAUX = 0; forAUX < GOfeet.Length; forAUX++)
            {
                AUXmaterials = GOfeet[forAUX].GetComponent<Renderer>().sharedMaterials;
                materialcount = GOfeet[forAUX].GetComponent<Renderer>().sharedMaterials.Length;
                for (int forAUX2 = 0; forAUX2 < materialcount; forAUX2++)
                    for (int forAUX3 = 0; forAUX3 < allskins.Length; forAUX3++)
                        for (int forAUX4 = 1; forAUX4 < 6; forAUX4++)
                        {
                            if (AUXmaterials[forAUX2].name == allskins[forAUX3] + forAUX4)
                            {
                                AUXmaterials[forAUX2] = headskin;
                                GOfeet[forAUX].GetComponent<Renderer>().sharedMaterials = AUXmaterials;
                            }
                        }
            }
        }
        public void Deactivateall()
        {
            for (int forAUX = 0; forAUX < GOhair.Length; forAUX++) GOhair[forAUX].SetActive(false);
            for (int forAUX = 0; forAUX < GOchest.Length; forAUX++) GOchest[forAUX].SetActive(false);
            for (int forAUX = 0; forAUX < GOlegs.Length; forAUX++) GOlegs[forAUX].SetActive(false);
            for (int forAUX = 0; forAUX < GOfeet.Length; forAUX++) GOfeet[forAUX].SetActive(false);
            GOglasses.SetActive(false);
            glassesactive = false;
        }
        public void Activateall()
        {
            for (int forAUX = 0; forAUX < GOhair.Length; forAUX++) GOhair[forAUX].SetActive(true);
            for (int forAUX = 0; forAUX < GOchest.Length; forAUX++) GOchest[forAUX].SetActive(true);
            for (int forAUX = 0; forAUX < GOlegs.Length; forAUX++) GOlegs[forAUX].SetActive(true);
            for (int forAUX = 0; forAUX < GOfeet.Length; forAUX++) GOfeet[forAUX].SetActive(true);
            GOglasses.SetActive(true);
            glassesactive = true;
        }
        public void Menu()
        {
            allOptions = !allOptions;
        }
        public void Glasseson()
        {
            glassesactive = !glassesactive;
            GOglasses.SetActive(glassesactive);
        }
        public void HatOn()
        {
            if (!hatactive)
            {
                GOhair[hair].SetActive(false);
                hair = 7;
                GOhair[hair].SetActive(true);
                hatactive = true;
            }
            else
            {
                GOhair[hair].SetActive(false);
                hair = 0;
                GOhair[hair].SetActive(true);
                hatactive = false;
            }
        }

        //models
        public void Nexthair()
        {
            GOhair[hair].SetActive(false);
            if (hatactive) hair = 0;
            hatactive = false;
            if (hair < GOhair.Length - 2) hair++;
            else hair = 0;
            GOhair[hair].SetActive(true);
        }
        public void Prevhair()
        {
            GOhair[hair].SetActive(false);
            if (hatactive) hair = GOhair.Length - 2;
            hatactive = false;
            if (hair > 0) hair--;
            else hair = GOhair.Length - 2;
            GOhair[hair].SetActive(true);
        }
        public void Nextchest()
        {
            GOchest[chest].SetActive(false);
            if (chest < GOchest.Length - 1) chest++;
            else chest = 0;
            GOchest[chest].SetActive(true);
        }
        public void Prevchest()
        {
            GOchest[chest].SetActive(false);
            chest--;
            if (chest < 0) chest = GOchest.Length - 1;
            GOchest[chest].SetActive(true);
        }
        public void Nextlegs()
        {
            GOlegs[legs].SetActive(false);
            if (legs < GOlegs.Length - 1) legs++;
            else legs = 0;
            GOlegs[legs].SetActive(true);
        }
        public void Prevlegs()
        {
            GOlegs[legs].SetActive(false);
            if (legs > 0) legs--;
            else legs = GOlegs.Length - 1;
            GOlegs[legs].SetActive(true);
        }
        public void Nextfeet()
        {
            GOfeet[feet].SetActive(false);
            if (feet < GOfeet.Length - 1) feet++;
            else feet = 0;
            GOfeet[feet].SetActive(true);
        }
        public void Prevfeet()
        {
            GOfeet[feet].SetActive(false);
            if (feet > 0) feet--;
            else feet = GOfeet.Length - 1;
            GOfeet[feet].SetActive(true);
        }

        //materials
        public void Nextskincolor(int todo)
        {
            ChangeMaterials(MATSkins, todo);
        }
        public void Nextglasses(int todo)
        {
            ChangeMaterials(MATGlasses, todo);
        }
        public void Nexteyescolor(int todo)
        {
            ChangeMaterials(MATEyes, todo);
        }
        public void Nextteethcolor(int todo)
        {
        GOhead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(6, 50f);
        GOhead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(9, 11f);
        GOhead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(11, 57f);
        GOhead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(14, 65f);
        GOhead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(15, 33f);
        ChangeMaterials(MATTeeth, todo);
        }
        public void CloseMouth()
    {
        GOhead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(6, 0f);
        GOhead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(9, 0f);
        GOhead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(11, 0f);
        GOhead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(14, 0f);
        GOhead.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(15, 0f);        
    }
        public void Nexthaircolor(int todo)
        {
            ChangeMaterials(MATHairA, todo);
            ChangeMaterials(MATHairB, todo);
            ChangeMaterials(MATHairC, todo);
            ChangeMaterials(MATHairD, todo);
            ChangeMaterials(MATHairE, todo);
            ChangeMaterials(MATHairF, todo);
            ChangeMaterials(MATHairG, todo);
        }
        public void Nexthatcolor(int todo)
        {
            if (hatactive) ChangeMaterials(MATHat, todo);
        }
        public void Nextchestcolor(int todo)
        {
            if (chest < 3) ChangeMaterials(MATShirt, todo);
            if (chest == 3) ChangeMaterials(MATSweater, todo);
            if (chest > 3) ChangeMaterials(MATTshirt, todo);
        }
        public void Nextlegscolor(int todo)
        {
            ChangeMaterials(MATLegs, todo);
        }
        public void Nextfeetcolor(int todo)
        {
            if (feet == 1 || feet == 2) ChangeMaterials(MATFeetA, todo);
            if (feet == 3) ChangeMaterials(MATFeetB, todo);
        }

        public void Nude()
        {
            GOchest[chest].SetActive(false);
            GOlegs[legs].SetActive(false);
            GOfeet[feet].SetActive(false);
            chest = 0; legs = 0; feet = 0;
            GOchest[0].SetActive(true);
            GOlegs[0].SetActive(true);
            GOfeet[0].SetActive(true);
        }
        public void Resetmodel()
        {
            Activateall();
            ChangeMaterials(MATHat, 3);
            ChangeMaterials(MATSkins, 3);
            ChangeMaterials(MATHairA, 3);
            ChangeMaterials(MATHairB, 3);
            ChangeMaterials(MATHairC, 3);
            ChangeMaterials(MATHairD, 3);
            ChangeMaterials(MATHairE, 3);
            ChangeMaterials(MATHairF, 3);
            ChangeMaterials(MATHairG, 3);
            ChangeMaterials(MATGlasses, 3);
            ChangeMaterials(MATEyes, 3);
            ChangeMaterials(MATTshirt, 3);
            ChangeMaterials(MATShirt, 3);
            ChangeMaterials(MATSweater, 3);
            ChangeMaterials(MATLegs, 3);
            ChangeMaterials(MATFeetA, 3);
            ChangeMaterials(MATFeetB, 3);
            Menu();
        }
        public void Randomize()
        {
            Deactivateall();
            ResetSkin();
            //models
            hair = Random.Range(0, 8);
            GOhair[hair].SetActive(true);
            if (hair == 7) hatactive = true; else hatactive = false;
            chest = Random.Range(1, GOchest.Length); GOchest[chest].SetActive(true);
            legs = Random.Range(1, GOlegs.Length); GOlegs[legs].SetActive(true);
            feet = Random.Range(1, GOfeet.Length); GOfeet[feet].SetActive(true);

            if (Random.Range(0, 4) > 2)
            {
                glassesactive = true;
                GOglasses.SetActive(true);
                ChangeMaterials(MATGlasses, 2);
            }
            else glassesactive = false;

            //materials
            ChangeMaterials(MATEyes, 2);
            ChangeMaterials(MATTeeth, 2);
            for (int forAUX = 0; forAUX < (Random.Range(0, 4)); forAUX++) Nexthaircolor(0);
            for (int forAUX = 0; forAUX < (Random.Range(0, 13)); forAUX++) Nextfeetcolor(0);
            for (int forAUX = 0; forAUX < (Random.Range(0, 25)); forAUX++) Nextlegscolor(0);
            for (int forAUX = 0; forAUX < (Random.Range(0, 12)); forAUX++) Nexthatcolor(0);
            for (int forAUX = 0; forAUX < (Random.Range(0, 17)); forAUX++) Nextchestcolor(0);
            for (int forAUX = 0; forAUX < (Random.Range(0, 5)); forAUX++) Nextskincolor(0);

        }
        public void CreateCopy()
        {
            GameObject newcharacter = Instantiate(gameObject, transform.position, transform.rotation);
            for (int forAUX = 23; forAUX > 0; forAUX--)
            {
                if (!newcharacter.transform.GetChild(forAUX).gameObject.activeSelf) DestroyImmediate(newcharacter.transform.GetChild(forAUX).gameObject);
            }
            if (!GOglasses.activeSelf) DestroyImmediate(newcharacter.transform.Find("ROOT/TK/TK Pelvis/TK Spine/TK Spine1/TK Spine2/TK Neck/TK Head/Glasses").gameObject as GameObject);
            DestroyImmediate(newcharacter.GetComponent<TKBoyPrefabMaker>());
        }
        public void FIX()
        {
            GameObject newcharacter = Instantiate(gameObject, transform.position, transform.rotation);
            for (int forAUX = 23; forAUX > 0; forAUX--)
            {
                if (!newcharacter.transform.GetChild(forAUX).gameObject.activeSelf) DestroyImmediate(newcharacter.transform.GetChild(forAUX).gameObject);
            }
            if (!GOglasses.activeSelf) DestroyImmediate(newcharacter.transform.Find("ROOT/TK/TK Pelvis/TK Spine/TK Spine1/TK Spine2/TK Neck/TK Head/Glasses").gameObject as GameObject);
            DestroyImmediate(newcharacter.GetComponent<TKBoyPrefabMaker>());
            DestroyImmediate(gameObject);
        }


        void ChangeMaterial(GameObject GO, Object[] MAT, int todo)
        {
            bool found = false;
            int MATindex = 0;
            int subMAT = 0;
            Material[] AUXmaterials;
            AUXmaterials = GO.GetComponent<Renderer>().sharedMaterials;
            int materialcount = GO.GetComponent<Renderer>().sharedMaterials.Length;

            for (int forAUX = 0; forAUX < materialcount; forAUX++)
                for (int forAUX2 = 0; forAUX2 < MAT.Length; forAUX2++)
                {
                    if (AUXmaterials[forAUX].name == MAT[forAUX2].name)
                    {
                        subMAT = forAUX;
                        MATindex = forAUX2;
                        found = true;
                    }
                }
            if (found)
            {
                if (todo == 0) //increase
                {
                    MATindex++;
                    if (MATindex > MAT.Length - 1) MATindex = 0;
                }
                if (todo == 1) //decrease
                {
                    MATindex--;
                    if (MATindex < 0) MATindex = MAT.Length - 1;
                }
                if (todo == 2) //random value
                {
                    MATindex = Random.Range(0, MAT.Length);
                }
                if (todo == 3) //reset value
                {
                    MATindex = 0;
                }
                if (todo == 4) //penultimate
                {
                    MATindex = MAT.Length - 2;
                }
                if (todo == 5) //last one
                {
                    MATindex = MAT.Length - 1;
                }
                AUXmaterials[subMAT] = MAT[MATindex] as Material;
                GO.GetComponent<Renderer>().sharedMaterials = AUXmaterials;
            }
        }
        void ChangeMaterials(Object[] MAT, int todo)
        {
            for (int forAUX = 0; forAUX < GOhair.Length; forAUX++) ChangeMaterial(GOhair[forAUX], MAT, todo);
            ChangeMaterial(GOhead, MAT, todo);
            ChangeMaterial(GOglasses, MAT, todo);
            ChangeMaterial(GOheadsimple, MAT, todo);
            for (int forAUX = 0; forAUX < GOchest.Length; forAUX++) ChangeMaterial(GOchest[forAUX], MAT, todo);
            for (int forAUX = 0; forAUX < GOlegs.Length; forAUX++) ChangeMaterial(GOlegs[forAUX], MAT, todo);
            for (int forAUX = 0; forAUX < GOfeet.Length; forAUX++) ChangeMaterial(GOfeet[forAUX], MAT, todo);
        }
        void SwitchMaterial(GameObject GO, Object[] MAT1, Object[] MAT2)
        {
            Material[] AUXmaterials;
            AUXmaterials = GO.GetComponent<Renderer>().sharedMaterials;
            int materialcount = GO.GetComponent<Renderer>().sharedMaterials.Length;
            int index = 0;
            for (int forAUX = 0; forAUX < materialcount; forAUX++)
                for (int forAUX2 = 0; forAUX2 < MAT1.Length; forAUX2++)
                {
                    if (AUXmaterials[forAUX].name == MAT1[forAUX2].name)
                    {
                        index = forAUX2;
                        if (forAUX2 > MAT2.Length - 1) index -= (int)Mathf.Floor(index / 4) * 4;
                        AUXmaterials[forAUX] = MAT2[index] as Material;
                        GO.GetComponent<Renderer>().sharedMaterials = AUXmaterials;
                    }
                }
        }
    }
