using UnityEngine;
using System.Collections.Generic;

// http://qiita.com/2dgames_jp/items/f29e915357c1decbc4b7

/// <summary>
/// A-Starアルゴリズムのそれなりに柔軟な実装
/// 位置をTで表す前提で、ふたつのT間の仮説距離が計算可能である必要がある
/// Path finding.
/// </summary>
public class PathFinding<T> where T : IHeuristicDistance<T>  {

	public enum PFState{
		None,
		Opened,
		Closed
	}

	/// <summary>
	/// 探索状態の保存域
	/// The cache.
	/// </summary>
	protected Dictionary<T,PFState> cache;

	/// <summary>
	/// その場所がどこから繋がって探索されたものなのかの保存域。これをたどることによってゴールからスタートに向かって経路が一意に決まる
	/// The parent.
	/// </summary>
	protected Dictionary<T,T> parent;

	/// <summary>
	/// スタート地点を0として、そこからここまでの総距離の保存域。
	/// The moved cost.
	/// </summary>
	protected Dictionary<T,float> MovedCost;

	/// <summary>
	/// 次の検索対象となるべき位置の保存域。こいつが常にソートされて先頭から次の探索候補となる
	/// The next.
	/// </summary>
	protected List<T> next;

	protected T Goal;
	protected T Start;


	private float CalcHScore(T Pos){
		return Goal.CalcHeuristicDistance(Pos);
	}

	public PathFinding(){
		cache = new Dictionary<T, PFState>();
		parent = new Dictionary<T, T>();
		next = new List<T>();
		MovedCost = new Dictionary<T, float> ();
	}


	public void SetStartAndGoal(T s,T g){
		Start = s;
		Goal = g;
		OnPostSetup ();
	}

	protected virtual void OnPostSetup(){
	}

	/// <summary>
	/// 引数の地点から、次の探索候補として追加されるべき地点の一覧を返却する。こいつを良い感じにoverrideすることで、地点同士の繋がりを実現する
	/// Calculates the nexts.
	/// </summary>
	/// <returns>The nexts.</returns>
	/// <param name="orig">Original.</param>
	protected virtual List<T> calcNexts(T orig){
		return null;
	}

	/// <summary>
	/// 引数の地点から次の探索候補を得てそれを追加する。ついでにそこへのスタート地点からの総距離や、繋がりをセットする。
	/// 地点へのリンク数とは別に距離の重さを設定したい場合はここら辺をoverrideしたりする想定
	/// Opens the next.
	/// </summary>
	/// <param name="pos">Position.</param>
	protected virtual void openNext(T pos){
		cache[pos] = PFState.Closed;
		var bas = MovedCost [pos];

		foreach(var nex in calcNexts(pos)){
			MovedCost[nex] = bas + 1;
			parent[nex] = pos;
			cache[nex] = PFState.Opened;
			next.Add(nex);
		}
	}

	/// <summary>
	/// 探索を行った結果を、GoalからStartへの地点のListとして返却する
	/// Gets the path.
	/// </summary>
	/// <returns>The path.</returns>
	public List<T> GetPath(){
		var safe = 100;
		var ret = new List<T>();

		var p = Goal;

	loop:
			--safe;

		if (safe < 0) {
			return ret;
		}
		ret.Add (p);
		if (p.Equals (Start)) {
			return ret;
		}
		p = parent [p];
		goto loop;

	}

	/// <summary>
	/// 探索を行う。
	/// Finds the path.
	/// </summary>
	public void FindPath(){

		var safe = 100;

		// 初期地点の初期設定。総移動量は0で自分を親としOpened状態にして探索候補一覧にぶっ込んでおく
		var c = Start;
		MovedCost [c] = 0;
		parent [c] = c;
		cache[c] = PFState.Opened;
		next.Add(c);

	loop:
			--safe;
		if (safe < 0) {
			return;
		}

		// 現在の候補一覧の中で、総移動量＋仮定距離の順にソートする。ただしすでに移動したところは無限に遠方とする。
		next.Sort(
			(t1,t2) =>{
			if(cache[t1] == PFState.Closed){ return 1;}
			if(cache[t2] == PFState.Closed){ return -1;}

			var val1 = (MovedCost[t1]+CalcHScore(t1));
			var val2 = (MovedCost[t2]+CalcHScore(t2));
			if(val1==val2){
				return System.Math.Sign(MovedCost[t1]-MovedCost[t2]);
			}

			return System.Math.Sign(val1-val2);
		}
			);

		// もし候補が存在しなければ打ち切る
		if (next.Count == 0) {
			return;
		}

		// ソートした結果の先頭を候補とする
		var n = next[0];

		// もし探索済みであれば候補がすべて探索完了したということなので打ち切る
		if (cache [n] == PFState.Closed) {
			return;
		}

		// ゴールと同位置にたどり着いたなら終了
		if(n.Equals(Goal)){
			return;
		}

		// 次の候補はゴールではなかったので、さらにそこから次の候補を一覧へと追加する
		openNext(n);
		goto loop;
	}
}

public interface IHeuristicDistance<T> {
	float CalcHeuristicDistance (T dst);
}

public class Tuple<XType,YType> 
	where XType : struct 
	where YType : struct {

	public XType X;
	public YType Y;
	
		public static Tuple<X,Y> Create<X,Y>(X x,Y y) where X : struct where Y : struct {
			var ret = new Tuple<X, Y> ();
			ret.X = x;
			ret.Y = y;
			return ret;
		}

	public override bool Equals (object obj)
	{
		if (!(obj is Tuple<XType,YType>)) {
			return false;
		}
		var o = (Tuple<XType,YType>)obj;
		return X.Equals (o.X) && Y.Equals (o.Y);
	}

	public override int GetHashCode ()
	{
		return X.GetHashCode() ^ (Y.GetHashCode() << 1);
	}
}

