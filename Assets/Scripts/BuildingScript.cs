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

	bool done = false;

	// Use this for initialization
	void Start () {
        present = new bool[rows];
        building = new Transform[rows,cols];
        Vector3 origin = transform.position;
        for (int j = 0; j < rows; j++){
            present[j] = true;
            for (int i = 0; i < cols; i++){
				Vector3 CurrPos = origin + new Vector3(i , -j, 0);
				building[j,i] = (Transform)Instantiate(
					BlockTransform,
					CurrPos, 
					Quaternion.identity);
                if (j == 0) {
                    ((Transform)Instantiate(
                        PlatformTransform,
                        CurrPos, 
                        Quaternion.identity)).parent = transform;
                }
                if (i == 0) {
                    ((Transform)Instantiate(
                        BlockWallTransform,
                        CurrPos, 
                        Quaternion.identity)).parent = transform;
                }
                if (i == cols - 1){
                    CurrPos += new Vector3(1, 0, 0);
                    ((Transform)Instantiate(
                        BlockWallTransform,
                        CurrPos, 
                        Quaternion.identity)).parent = transform;
                }
                building[j,i].parent = transform;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        int n_gone = 0;
        for (int j = 0; j < rows; j++){
            if (!present[j]){
                n_gone++;
                continue;
            }
            int n_damaged = 0;
            for (int i = 0; i < cols; i++){
                if (building[j,i].gameObject.GetComponent<BlockScript>().damaged)
                    n_damaged++;
            }
            if (n_damaged == cols){
                present[j] = false;
                for (int y = 0; y < j; y++){
                    for (int i = 0; i < cols; i++){
                        building[y,i].Translate(new Vector3(0, -1, 0));
                    }
                }
            }
        }
        if (n_gone >= rows){
            // Drop powerup
			if ((FindObjectsOfType<BuildingScript> ()).Length<=1) {
				//end game, monster wins
				FindObjectOfType<Hero> ().victory = true;
			}
            Destroy(gameObject);
        }
	}
}
