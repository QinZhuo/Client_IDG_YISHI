using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace IDG
{
	

public class DataFile:MonoBehaviour{
	private static DataFile _instance;
	public static DataFile Instance{
		get{
			if(_instance==null){
				var obj= new GameObject("DataFile");
				_instance= obj.AddComponent<DataFile>();
			}
			return _instance;
		}
	}
		IEnumerator wwwLoad(string _path, Action<byte[],ISerializable> action,ISerializable obj){
	
			#if UNITY_EDITOR || UNITY_IOS
			_path = "file://" + _path;
			#endif
			
			WWW www = new WWW (_path);
	
			yield return www;
			Debug.LogWarning("WWW 加载二进制文件["+_path+"]长度"+www.bytes.Length);
			action (www.bytes,obj);
		}
		
		public static void SerializeToFile(string filePath,ISerializable obj)
	     {
			#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh();
	     	using (System.IO.FileStream fs = System.IO.File.Create( Application.streamingAssetsPath+"/"+filePath))
			{
				var bytes=Serialization.Serialize(obj);
				fs.Write(bytes,0,bytes.Length);
				fs.Close();
	   		}
			#endif
		}
		
		public void DeserializeToData(string filePath,ISerializable obj)
		{
			StartCoroutine(wwwLoad( Application.streamingAssetsPath+"/"+filePath,Deserialize,obj));

		}
		private void Deserialize(Byte[] bytes,ISerializable obj){
			Serialization.Deserialize(obj,bytes);
		}
	}
}