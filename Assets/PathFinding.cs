using UnityEngine;
using System.Collections.Generic;

public class PathFinding<T>  {

	public enum PFState{
		None,
		Opened,
		Closed
	}

	protected Dictionary<T,PFState> cache;
	protected Dictionary<T,T> parent;


	protected List<T> next;

	protected T Goal;
	protected T Start;

	protected float GetMoveCost(T pos){
		return 0.0f;
	}

	protected float CalcHScore(T Pos){
		return 0;
	}

	public PathFinding(){
		cache = new Dictionary<T, PFState>();
		parent = new Dictionary<T, T>();
		next = new List<T>();
	}

	protected T [] calcNexts(T orig){
		return null;
	}

	protected void openNext(T pos){
		cache[pos] = PFState.Closed;

		foreach(var nex in calcNexts(pos)){
			parent[nex] = pos;
			cache[nex] = PFState.Opened;
			next.Add(pos);
		}
	}

	public void FindPath(){
		var c = Start;

		cache[c] = PFState.Opened;
		next.Add(c);

		next.Sort(
			(t1,t2) =>{
			if(cache[t1] == PFState.Closed){ return -1;}
			if(cache[t2] == PFState.Closed){ return 1;}
			return 
				System.Math.Sign((GetMoveCost(t1)+CalcHScore(t1))-
					(GetMoveCost(t2)+CalcHScore(t2)));
		}
			);
		var n = next[0];

		if(n .Equals( Goal)){
			return;
		}

		openNext(n);
	}
}

