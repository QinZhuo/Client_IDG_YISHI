using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
namespace IDG
{
	public class Serialization  {

		public static Byte[] Serialize(ISerializable obj){
			return Serialize(obj.Serialize);
		}
		public static void Deserialize(ISerializable obj,byte[] byteData){
			Deserialize(obj.Deserialize,byteData);
		}
		public static Byte[] Serialize(Action<ByteProtocol> serialize){
			var pro=new ByteProtocol();
			serialize(pro);
			return pro.GetByteStream();
		}

		public static void Deserialize(Action<ByteProtocol> deserialize,byte[] byteData ){
			var pro=new ByteProtocol();
			pro.InitMessage(byteData);
			deserialize(pro);
		}

	}	

	public interface ISerializable {
		void Serialize(ByteProtocol protocol);
		void Deserialize(ByteProtocol protocol);
	}

}
