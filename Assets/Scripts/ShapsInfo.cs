using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	public class ShapsInfo : MonoBehaviour {
		public ShapDataInfo[] shaps;

		private void OnDrawGizmosSelected() {
			if(shaps==null)return;
			int i=0;
			foreach (var shap in shaps)
			{
				Gizmos.color=Color.HSVToRGB(((i+0.0f)/shaps.Length),0.6f,1);
				foreach (var p in shap.points)
				{
					Gizmos.DrawSphere(transform.position+ new Vector3(p.x,0.5f,p.y),0.05f);
				}
				i++;
			}
		}
		// Use this for initialization
		
	}
	[System.Serializable]
	public struct ShapDataInfo{
		public Vector2[] points;
	}

