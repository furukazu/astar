using UnityEngine;
using System.Collections.Generic;

public class Test : MonoBehaviour {

	// テスト用のフィールドの位置情報を表す
	private class PlainField : Tuple<int,int>
		, IHeuristicDistance<PlainField>
	{

		#region IHeuristicDistance implementation
		/// <summary>
		/// 仮定距離は単なるX距離＋Y距離
		/// Calculates the heuristic distance.
		/// </summary>
		/// <returns>The heuristic distance.</returns>
		/// <param name="dst">Dst.</param>
		public float CalcHeuristicDistance (PlainField dst)
		{
			return System.Math.Abs (X - dst.X) + System.Math.Abs (Y - dst.Y);
		}
		#endregion

		public static PlainField Create(int x,int y){
			var ret = new PlainField ();
			ret.X = x;
			ret.Y = y;
			return ret;
		}

		public PlainField CreateNext(int x,int y){
			var ret = new PlainField ();
			ret.X = X + x;
			ret.Y = Y + y;
			return ret;
		}

		/// <summary>
		/// さくっと表示させたいので
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Test+PlainField"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Test+PlainField"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("({0},{1})",X,Y);
		}
	}

	// テスト用の経路探索
	private class MyPathFind : PathFinding<PlainField>{

		/// <summary>
		/// 隣り合うフィールドは上下左右とします
		/// 引数の地点から、次の探索候補として追加されるべき地点の一覧を返却する。こいつを良い感じにoverrideすることで、地点同士の繋がりを実現する
		/// Calculates the nexts.
		/// </summary>
		/// <returns>The nexts.</returns>
		/// <param name="orig">Original.</param>
		protected override List<PlainField> calcNexts (PlainField orig)
		{
			var ret = new List<PlainField> ();

			var xs = new int[]{1,0,-1,0};
			var ys = new int[]{0,1,0,-1};

			for (var i =0; i<xs.Length; ++i) {
				var next = orig.CreateNext (xs [i], ys [i]);

				// フィールドのX,Yは0以上9以下とします
				if(next.X<0 || next.Y<0 || next.X>9 || next.Y>9){ continue;}
				if (!cache.ContainsKey (next)) {
					//cache [next] = PFState.Opened;
					ret.Add (next);
				}
			}

			return ret;
		}

		protected override void OnPostSetup ()
		{
			base.OnPostSetup ();

			// 障害物があるとする
			cache [PlainField.Create (0,3)] = PFState.Closed;
			cache [PlainField.Create (1,3)] = PFState.Closed;
			cache [PlainField.Create (2,3)] = PFState.Closed;
			cache [PlainField.Create (3,3)] = PFState.Closed;
			cache [PlainField.Create (4,3)] = PFState.Closed;
			cache [PlainField.Create (5,3)] = PFState.Closed;
			cache [PlainField.Create (6,3)] = PFState.Closed;
			cache [PlainField.Create (7,3)] = PFState.Closed;
			cache [PlainField.Create (8,3)] = PFState.Closed;

			cache [PlainField.Create (9,6)] = PFState.Closed;
			cache [PlainField.Create (8,6)] = PFState.Closed;
			cache [PlainField.Create (7,6)] = PFState.Closed;
			cache [PlainField.Create (6,6)] = PFState.Closed;
			cache [PlainField.Create (5,6)] = PFState.Closed;
			cache [PlainField.Create (4,6)] = PFState.Closed;
			cache [PlainField.Create (3,6)] = PFState.Closed;
			cache [PlainField.Create (2,6)] = PFState.Closed;
			cache [PlainField.Create (1,6)] = PFState.Closed;

		}
	}

	private MyPathFind myp;

	// Use this for initialization
	void Start () {
		myp = new MyPathFind ();
		// 0,0から9,9までの経路探索
		myp.SetStartAndGoal (PlainField.Create (0, 0), PlainField.Create (9, 9));
		myp.FindPath ();
		var p = myp.GetPath ();
		print (p);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
