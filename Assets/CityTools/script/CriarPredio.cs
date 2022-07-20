using UnityEngine;
using System.Collections;

public class CriarPredio : MonoBehaviour {

	public GameObject terrio;
	public GameObject andar;
	public GameObject cobertura;

	public Transform centro;

	public float alturaAndar = 4f;

	public int andarQuantMax = 10;
	public int andarQuantMin = 3;
	int andarQuant = 4;

    public GameObject building;

    // Use this for initialization
    void Start () {
		//criarPredio ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void criarPredio(){
        building = Instantiate(gameObject,this.transform.position, Quaternion.identity);
        building.name = "building_"+Time.time;
        andarQuant = Random.Range (andarQuantMin,andarQuantMax);
	
		criarAndar(terrio, this.transform.position);
		Vector3 pos = this.transform.position;
		//criarAndar(andar,this.transform.position);
        for (int i = 0; i < andarQuant - 1; i++ ){
			pos.y += alturaAndar;
			criarAndar(andar,pos);
		}

		pos.y += alturaAndar;
		criarAndar(cobertura,pos);

	}

	public void criarAndar(GameObject obj, Vector3 pos){

		GameObject a = Instantiate (obj, pos, Quaternion.identity) as GameObject;
		a.transform.parent = building.transform;

	}
}
