using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderTest : MonoBehaviour {
	public bool useComputShader=false;

	public ComputeShader shader;
	public int loop=10;
	int lesize=128;
	 int len;
	public int kernel;
	 Vector3[] addArray;
	 Vector3[] subArray;
	 Vector3[] resultArray;
	private ComputeBuffer inputBuffer;
	private ComputeBuffer resultBuffer;

	// Use this for initialization
	void Start () {
		len=lesize*lesize;
		addArray=new Vector3[len];
		subArray=new Vector3[len];
			resultArray=new Vector3[len];
		for (int i = 0; i < len; i++)
		{
			addArray[i]=Vector3.one* i;
			subArray[i]=Vector3.one* -i;	
		}
		if(useComputShader){
			ComputeInit();
		}
	}
	void ComputeInit(){
	
		inputBuffer=new ComputeBuffer(len,12);
		resultBuffer=new ComputeBuffer(len,12);
		kernel= shader.FindKernel("CSMain");
		shader.SetBuffer(kernel,"input",inputBuffer);
		shader.SetBuffer(kernel,"Result",resultBuffer);
		resultBuffer.SetData(resultArray);
	}
	void ComputeRun(){
		for (int i = 0; i < loop; i++)
		{
			//if(i%3==0){
				inputBuffer.SetData(addArray);
			// }else
			// {
			// 	inputBuffer.SetData(subArray);
			// }
				shader.Dispatch(kernel,lesize/2,lesize/2,1);
				resultBuffer.GetData(resultArray);
		}
		
	}
	private void OnDestroy() {
		inputBuffer.Release();
		resultBuffer.Release();
		
	}
	void DefaultRun(){
	
		for (int i = 0; i < loop; i++)
		{
			for (int j = 0; j < len; j++)
			{
				///if(i%3==0){
					resultArray[j]+=addArray[j];
				// }else
				// {
				// 	resultArray[j]-=addArray[j];
				// }
				
			}
		}

	}
	// Update is called once per frame
	void Update () {
		//if(Input.GetKeyDown(KeyCode.A))
		{
			
			//var time=System.DateTime.Now;
			if(useComputShader){
				ComputeRun();
			}else
			{
				DefaultRun();
			}
			//Debug.LogError((System.DateTime.Now-time).TotalMilliseconds);

		}
		
	}	
}
