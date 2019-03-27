using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
//using UnityEditor;
namespace VoxelRender
{
	public class VoxImporter{

	
		public static VoxelData LoadVoxFile(string path){
			if(path==null|| path=="")return null;
			byte[] bytes=File.ReadAllBytes(path);
			using(MemoryStream memoryStream=new MemoryStream(bytes)){
				using(BinaryReader binaryReader=new BinaryReader(memoryStream)){
					return ReadData(binaryReader);
				}
			}
		}
		public static VoxelData ReadData(BinaryReader reader){
			var vox= reader.ReadBytes(4);
			if(!Compare("VOX ",vox)){
				ErrorCantFind("VOX");
				return null;
			}
			var version=reader.ReadBytes(4);
			var main=reader.ReadBytes(4);
			if(!Compare("MAIN",main)){
				ErrorCantFind("MAIN");
				return null;
			}
			var voxelData=VoxelRenderManager.GetData();
			int mainSize=reader.ReadInt32();
			int childSize=reader.ReadInt32();
			reader.ReadBytes(mainSize);
			int readSize=0;
			string unKnowNode="";
			while(readSize<childSize){
				var nodeType=reader.ReadBytes(4);
				if(Compare("PACK",nodeType)){
					int packDataSize=reader.ReadInt32();
					int packChildSize=reader.ReadInt32();
					reader.ReadInt32();
					readSize+=packDataSize+packChildSize+4*3;
				}else if(Compare("SIZE",nodeType)){
					readSize+= ReadSize(reader,voxelData);
				}else if(Compare("XYZI",nodeType)){
					readSize+=ReadVoxelIndex(reader,voxelData);
				}else if(Compare("RGBA",nodeType)){
					readSize+=ReadColor(reader,voxelData);
				}else{
					var typeStr=System.Text.Encoding.ASCII.GetString(nodeType);
					if(!unKnowNode.Contains(typeStr)){
						unKnowNode+="["+typeStr+"]";
					}
					
					int chunkContentBytes=reader.ReadInt32();
					int childrenBytes=reader.ReadInt32();
					reader.ReadBytes(chunkContentBytes+childrenBytes);
					readSize+=chunkContentBytes+childrenBytes+12;
				}
			}
			Warning(" 不支持解析节点类型 "+unKnowNode); 
			Log(" 导入体素模型完成 体素数目"+voxelData.voxels.Count); 
			return voxelData;
		}
		static int ReadSize(BinaryReader reader,VoxelData voxelData){
			int dataSize=reader.ReadInt32();
			int childSize=reader.ReadInt32();
			voxelData.SetSceneScale(reader.ReadInt32(),reader.ReadInt32(),reader.ReadInt32());
			if(childSize>0){
				reader.ReadBytes(childSize);
				Warning("Size节点拥有多余未知数据");
			}
			return dataSize+childSize+4*3;
		}
		static int ReadVoxelIndex(BinaryReader reader,VoxelData voxelData){
			int dataSize=reader.ReadInt32();
			int childSize=reader.ReadInt32();
			int voxelsCount=reader.ReadInt32();
			for (int i = 0; i < voxelsCount; i++)
			{
				var x=(int)reader.ReadByte();
				var z=(int)reader.ReadByte();
				var y=(int)reader.ReadByte();
				voxelData.SetVoxel(x,y,z,(int)reader.ReadByte());
			}
			if(childSize>0){
				reader.ReadBytes(childSize);
				Warning("XYZI节点拥有多余未知数据");
			}
			return dataSize+childSize+4*3;
		}
		static int ReadColor(BinaryReader reader,VoxelData voxelData){
			int dataSize=reader.ReadInt32();
			int childSize=reader.ReadInt32();
		
			for (int i = 0; i < 256; i++)
			{
				var colorInfo=new ColorInfo();
				colorInfo.r=(float)reader.ReadByte()/256;
				colorInfo.g=(float)reader.ReadByte()/256;
				colorInfo.b=(float)reader.ReadByte()/256;
				colorInfo.a=(float)reader.ReadByte()/256;
				voxelData.colors.Add(colorInfo);
			
			}
			if(childSize>0){
				reader.ReadBytes(childSize);
				Warning("RGBA节点拥有多余未知数据");
			}
			return dataSize+childSize+4*3;
		}
		static void ErrorCantFind(string cantFindStr){
			Error("格式出错 找不到"+cantFindStr+"标志");
		}
		static void Error(string error){
			UnityEngine.Debug.LogError("【VoxImporter】解析Vox文件失败 "+error);
		}
		static void Warning(string info){
			UnityEngine.Debug.LogWarning("【VoxImporter】"+info);
		}
		static void Log(string info){
			UnityEngine.Debug.Log("【VoxImporter】"+info);
		}
		public static bool Compare(string str,byte[] bytes,int offset=0){
			for (int i = 0; i < str.Length; i++)
			{
				if(bytes[offset+i]!=str[i]||(offset+i)>=bytes.Length){
					return false;
				}
			}
			return true;
		}
	}

}