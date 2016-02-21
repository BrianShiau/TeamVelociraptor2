using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BuildingScript : MonoBehaviour {

    [SerializeField]
    int rows, cols;
    [SerializeField]
    Transform BlockTransform, BlockWallTransform, PlatformTransform;

    bool[] present;
    Transform[,] building;
    Transform[,] walltriggers;
    int[,] wallposes;
    Transform[] platforms;
    int[] platposes;

	bool done = false;

	// Use this for initialization
	void Start () {
        present = new bool[rows];
        building = new Transform[rows,cols];
        walltriggers = new Transform[rows, 2];
        wallposes = new int[rows, 2];
        platforms = new Transform[cols];
        platposes = new int[cols];
        Vector3 origin = transform.position;
        for (int j = 0; j < rows; j++){
            present[j] = true;
            walltriggers[j, 0] = ((Transform)Instantiate(
                BlockWallTransform,
                origin + new Vector3(0, -j, 0), 
                Quaternion.identity));
            walltriggers[j, 0].parent = transform;
            wallposes[j,0] = 0;
            walltriggers[j, 1] = ((Transform)Instantiate(
                BlockWallTransform,
                origin + new Vector3(cols, -j, 0),
                Quaternion.identity));
            walltriggers[j, 1].parent = transform;
            wallposes[j,1] = cols;
            for (int i = 0; i < cols; i++){
				Vector3 CurrPos = origin + new Vector3(i , -j, 0);
				building[j,i] = (Transform)Instantiate(
					BlockTransform,
					CurrPos, 
					Quaternion.identity);
                building[j,i].parent = transform;
                if (j == 0) {
                    platforms[i] = ((Transform)Instantiate(
                        PlatformTransform,
                        CurrPos, 
                        Quaternion.identity));
                    platforms[i].parent = transform;
                    platposes[i] = 0;
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        int n_gone = 0;
        int[] currplatpos = new int[cols];
        for (int j = 0; j < rows; j++){
            if (!present[j]){
                n_gone++;
                continue;
            }
            int n_damaged = 0;
            int minwall = -1, maxwall = cols;
            for (int i = 0; i < cols; i++){
                if (j == 0){
                    currplatpos[i] = -1;
                }
                if (building[j,i].gameObject.GetComponent<BlockScript>().damaged){
                    n_damaged++;
                } else {
                    if (minwall == -1){
                        minwall = i;
                    } else {
                        maxwall = i;
                    }
                    if (currplatpos[i] == -1)
                        currplatpos[i] = j;
                }
            }
            if (n_damaged == cols){
                present[j] = false;
                for (int y = 0; y < j; y++){
                    for (int i = 0; i < cols; i++){
                        if (y == 0){
                            if (currplatpos[i] < j){
                                currplatpos[i]++;
                            }
                        }
                        building[y,i].Translate(new Vector3(0, -1, 0));
                        if (i == 0){
                            walltriggers[y, 0].Translate(new Vector3(0, -1, 0));
                            walltriggers[y, 1].Translate(new Vector3(0, -1, 0));
                        }
                    }
                }
            }
            if (minwall > wallposes[j,0]){
                walltriggers[j,0].Translate(
                    new Vector3(minwall - wallposes[j,0], 0, 0));
                wallposes[j,0] = minwall;
            }
            if (maxwall < wallposes[j,1]){
                walltriggers[j,1].Translate(
                    new Vector3(maxwall - wallposes[j,1] + 1, 0, 0));
                wallposes[j,1] = maxwall;
            }
        }
        for (int i = 0; i < cols; i++){
            if (currplatpos[i] > platposes[i]){
                platforms[i].Translate(
                    new Vector3(0, platposes[i] - currplatpos[i], 0));
                platposes[i] = currplatpos[i];
            }
        }
        if (n_gone >= rows){
			if ((FindObjectsOfType<BuildingScript> ()).Length<=1) {
				//end game, monster wins
				FindObjectOfType<Hero> ().victory = true;
			}
            Destroy(gameObject);
        }
	}
}
