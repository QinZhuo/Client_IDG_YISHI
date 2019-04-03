using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
public class IDGData{
	public static void SerialData(string filePath,object obj)
     {
     	using (System.IO.FileStream fs = System.IO.File.Create(filePath))
		{
			BinaryFormatter bf = new BinaryFormatter();
			 bf.Serialize(fs, obj);
   		}
	}
	public static T DeserialData<T>(string filePath)where T:class
	{
		using (System.IO.FileStream fs = System.IO.File.Open(filePath, System.IO.FileMode.Open))
		{
			BinaryFormatter bf = new BinaryFormatter();
			var obj = bf.Deserialize(fs);
			fs.Close();
			return obj as T;
		}
	}
}
