using UnityEngine;
using System.Collections.Generic;

public class FieldRenderer : MonoBehaviour {

	public GameObject Blank;

	public GameObject S;

	public GameObject G;

	public GameObject Kabe;

	public GameObject Path;


	public List<List<int>> All;

	private Test.MyPathFind pf;

	void Awake(){
		All = new List<List<int>>();
		for(var y = 0;y<10;++y){
			All.Add(new List<int>());
			for(var x = 0;x<10;++x){
				All[y].Add(0);
			}
		}
		//All[0][0] = 1;
		//All[9][9] = 2;

		pf = new  Test.MyPathFind();
		pf.SetField(All);
		pf.SetStartAndGoal(Test.PlainField.Create(0,0),Test.PlainField.Create(9,9));
	}

	// Use this for initialization
	void Start () {
	
		for(var y = 0;y<10;++y){
			for(var x = 0;x<10;++x){
				GameObject o = null;
				if(All[x][y]==0){
					o = Instantiate(Blank);
				}else if(All[x][y]==1){
					o = Instantiate(S);
				}else if(All[x][y]==2){
					o = Instantiate(G);
				}else if(All[x][y]==3){
					o = Instantiate(Kabe);
				}

				if(o==null){continue;}
				o.transform.position = new Vector3(x*32,y*32,0);
			}
		}

		pf.FindPath ();
		var p = pf.GetPath ();
		foreach(var v in p){
			var route = Instantiate(Path);
			route.transform.position = new Vector3(v.X*32,v.Y*32,-1);
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
