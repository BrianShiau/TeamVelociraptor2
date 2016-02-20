using UnityEngine;
using System.Collections;

public class BuildingScript : MonoBehaviour {

    [SerializeField]
    int rows, cols;
    [SerializeField]
    Transform BlockTransform;

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
                building[j,i].parent = transform;
            }
        }
        done = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (done){
            for (int i = 0; i < cols - 1; i++){
                building[1,i].gameObject.GetComponent<BlockScript>().DamageBuilding();
            }
            done = false;
        }
	}

    public void CheckBuilding(){
        for (int j = 0; j < rows; j++){
            if (!present[j]){
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
    }
}
