
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; 
using System.IO;

using UnityEditor;
namespace Puppet2D
{
	public class Puppet2D_BakeAnimation : MonoBehaviour
	{

		private AnimationClip clip;
		private AnimationClip newClip;

		private List<AnimationCurve> curvePosX = new List<AnimationCurve>();
		private List<AnimationCurve> curvePosY = new List<AnimationCurve>();
		private List<AnimationCurve> curvePosZ = new List<AnimationCurve>();
		private List<AnimationCurve> curveRotX = new List<AnimationCurve>();
		private List<AnimationCurve> curveRotY = new List<AnimationCurve>();
		private List<AnimationCurve> curveRotZ = new List<AnimationCurve>();
		private List<AnimationCurve> curveRotW = new List<AnimationCurve>();
		private List<AnimationCurve> curveScaleX = new List<AnimationCurve>();
		private List<AnimationCurve> curveScaleY = new List<AnimationCurve>();
		private List<AnimationCurve> curveScaleZ = new List<AnimationCurve>();

		private List<Transform> bones = new List<Transform>();
		private List<GameObject> childsOfGameobject = new List<GameObject>();

		private float frameTime;

		static private string _puppet2DPath;


		// Use this for initialization
		public void Run()
		{
			RecursivelyFindFolderPath("Assets");
			Debug.Log("Baking the following animations....");
			List<AnimationClip> animClips = GetAnimationLengths();
			bones = GetListBones();

			for (int j = 0; j < animClips.Count; j++)
			{
				newClip = new AnimationClip();

				//AnimatorStateInfo state =  transform.GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0);
				//float leng = state.length;

				clip = animClips[j];
				float leng = clip.length;
				//Debug.Log(leng);

				curvePosX.Clear(); curvePosY.Clear(); curvePosZ.Clear();
				curveRotX.Clear(); curveRotY.Clear(); curveRotZ.Clear(); curveRotW.Clear();
				curveScaleX.Clear(); curveScaleY.Clear(); curveScaleZ.Clear();

				for (int i = 0; i < bones.Count; i++)
				{
					curvePosX.Add(new AnimationCurve());
					curvePosY.Add(new AnimationCurve());
					curvePosZ.Add(new AnimationCurve());
					curveRotX.Add(new AnimationCurve());
					curveRotY.Add(new AnimationCurve());
					curveRotZ.Add(new AnimationCurve());
					curveRotW.Add(new AnimationCurve());
					curveScaleX.Add(new AnimationCurve());
					curveScaleY.Add(new AnimationCurve());
					curveScaleZ.Add(new AnimationCurve());

				}


				//Time.timeScale = 0.0f;
				//float _lastRealTime = 0.0f;
				frameTime = 1f / clip.frameRate;
				float numFramesInAnim = (leng * clip.frameRate);



				for (int i = 0; i < numFramesInAnim + 1; i++)
				{
					AnimationMode.StartAnimationMode();
					AnimationMode.BeginSampling();
					AnimationMode.SampleAnimationClip(transform.gameObject, clip, i * frameTime);
					transform.GetComponent<Puppet2D_GlobalControl>().Run();

					foreach (Transform bone in bones)
						bakeAnimation(bone, i * frameTime);



				}
				AnimationMode.EndSampling();
				AnimationMode.StopAnimationMode();




				newClip.name = clip.name + "_Baked";
				newClip.wrapMode = clip.wrapMode;
				AnimationClipSettings animClipSettings = new AnimationClipSettings();
				animClipSettings.stopTime = clip.length;
#if (!UNITY_4_3 && !UNITY_4_4 && !UNITY_4_5 && !UNITY_4_6)

				newClip.legacy = false;
#else
			AnimationUtility.SetAnimationType(newClip, ModelImporterAnimationType.Generic);
#endif
				SaveAnimationClip(newClip);

				AnimationEvent[] events = new AnimationEvent[1];
				events[0] = new AnimationEvent();
				events[0].time = clip.length;
				AnimationUtility.SetAnimationEvents(newClip, events);
				AnimationEvent[] events2 = new AnimationEvent[0];
				AnimationUtility.SetAnimationEvents(newClip, events2);
				/*AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves(newClip, true);
				for (int i=0; i<curveDatas.Length; i++) 
				{
					AnimationUtility.SetEditorCurve(
						newClip,
						curveDatas[i].path,
						curveDatas[i].type,
						curveDatas[i].propertyName,
						curveDatas[i].curve
						);
				}*/

				AssetDatabase.SaveAssets();

				UnityEngine.Object[] newSelection = new UnityEngine.Object[Selection.objects.Length + 1];
				for (int i = 0; i < Selection.objects.Length; i++)
				{
					newSelection[i] = Selection.objects[i];

				}
				newSelection[newSelection.Length - 1] = newClip;
				Selection.objects = newSelection;
				/*
				transform.GetComponent<Animator> ().StartRecording (0);


				Invoke ("stopRecording", leng);
				*/
			}
			Debug.Log("Finished Baking.");
		}


		// Update is called once per frame
		List<Transform> GetListBones()
		{
			List<Transform> returnList = new List<Transform>();
			GetAllChilds(transform.gameObject);
			foreach (GameObject child in childsOfGameobject)
			{
				if (child.transform.GetComponent<SpriteRenderer>() && child.transform.GetComponent<SpriteRenderer>().sprite && child.transform.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone") && !child.transform.GetComponent<Puppet2D_HiddenBone>())
				{
					returnList.Add(child.transform);
					if (child.transform.GetComponent<SpriteRenderer>().sprite.name.Contains("ffd"))
					{
						returnList.Add(child.transform.parent.transform);
					}

				}
			}
			return returnList;
		}

		private List<GameObject> GetAllChilds(GameObject transformForSearch)
		{
			List<GameObject> getedChilds = new List<GameObject>();

			foreach (Transform trans in transformForSearch.transform)
			{

				GetAllChilds(trans.gameObject);
				childsOfGameobject.Add(trans.gameObject);
			}
			return getedChilds;
		}

		void bakeAnimation(Transform bone, float time)
		{


			int index = bones.LastIndexOf(bone);
			string boneNameFul = GetFullName(bone, bone.name);
			//Debug.Log ("setting curve for " + boneNameFul + " at time " + time + " index is " + index + " curvePosX is " + curvePosX[index]);
			//Debug.Log (newClip.name);

			if (time > 0)
			{
				if ((curvePosX[index].Evaluate(time - frameTime) != bone.localPosition.x) ||
					(curvePosY[index].Evaluate(time - frameTime) != bone.localPosition.y) ||
						(curvePosZ[index].Evaluate(time - frameTime) != bone.localPosition.z))
				{
					curvePosX[index].AddKey(new Keyframe(time - frameTime, curvePosX[index].Evaluate(time - frameTime)));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localPosition.x", curvePosX[index]);

					curvePosY[index].AddKey(new Keyframe(time - frameTime, curvePosY[index].Evaluate(time - frameTime)));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localPosition.y", curvePosY[index]);

					curvePosZ[index].AddKey(new Keyframe(time - frameTime, curvePosZ[index].Evaluate(time - frameTime)));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localPosition.z", curvePosZ[index]);

					curvePosX[index].AddKey(new Keyframe(time, bone.localPosition.x));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localPosition.x", curvePosX[index]);

					curvePosY[index].AddKey(new Keyframe(time, bone.localPosition.y));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localPosition.y", curvePosY[index]);

					curvePosZ[index].AddKey(new Keyframe(time, bone.localPosition.z));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localPosition.z", curvePosZ[index]);

					//if(curvePosX[index].keys.Length>2)
					//	AnimationUtility.
					//	AnimationUtility.SetEditorCurve(newClip,EditorCurveBinding.FloatCurve(boneNameFul,typeof(Transform),"localPosition.x") ,curvePosX[index]);



				}

			}
			else
			{

				curvePosX[index].AddKey(new Keyframe(time, bone.localPosition.x));
				newClip.SetCurve(boneNameFul, typeof(Transform), "localPosition.x", curvePosX[index]);
				//AnimationUtility.SetEditorCurve(newClip,EditorCurveBinding.FloatCurve(boneNameFul,typeof(Transform),"m_LocalPosition.x") ,curvePosX[index]);


				curvePosY[index].AddKey(new Keyframe(time, bone.localPosition.y));
				newClip.SetCurve(boneNameFul, typeof(Transform), "localPosition.y", curvePosY[index]);

				curvePosZ[index].AddKey(new Keyframe(time, bone.localPosition.z));
				newClip.SetCurve(boneNameFul, typeof(Transform), "localPosition.z", curvePosZ[index]);

			}

			if (time > 0)
			{
				if ((curveRotX[index].Evaluate(time - frameTime) != bone.localRotation.x) ||
				   (curveRotY[index].Evaluate(time - frameTime) != bone.localRotation.y) ||
				   (curveRotZ[index].Evaluate(time - frameTime) != bone.localRotation.z) ||
				   (curveRotW[index].Evaluate(time - frameTime) != bone.localRotation.w))
				{
					curveRotX[index].AddKey(new Keyframe(time - frameTime, curveRotX[index].Evaluate(time - frameTime)));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localRotation.x", curveRotX[index]);

					curveRotY[index].AddKey(new Keyframe(time - frameTime, curveRotY[index].Evaluate(time - frameTime)));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localRotation.y", curveRotY[index]);

					curveRotZ[index].AddKey(new Keyframe(time - frameTime, curveRotZ[index].Evaluate(time - frameTime)));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localRotation.z", curveRotZ[index]);

					curveRotW[index].AddKey(new Keyframe(time - frameTime, curveRotW[index].Evaluate(time - frameTime)));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localRotation.w", curveRotW[index]);

					curveRotX[index].AddKey(new Keyframe(time, bone.localRotation.x));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localRotation.x", curveRotX[index]);

					curveRotY[index].AddKey(new Keyframe(time, bone.localRotation.y));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localRotation.y", curveRotY[index]);

					curveRotZ[index].AddKey(new Keyframe(time, bone.localRotation.z));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localRotation.z", curveRotZ[index]);

					curveRotW[index].AddKey(new Keyframe(time, bone.localRotation.w));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localRotation.w", curveRotW[index]);
				}

			}
			else
			{

				curveRotX[index].AddKey(new Keyframe(time, bone.localRotation.x));
				newClip.SetCurve(boneNameFul, typeof(Transform), "localRotation.x", curveRotX[index]);

				curveRotY[index].AddKey(new Keyframe(time, bone.localRotation.y));
				newClip.SetCurve(boneNameFul, typeof(Transform), "localRotation.y", curveRotY[index]);

				curveRotZ[index].AddKey(new Keyframe(time, bone.localRotation.z));
				newClip.SetCurve(boneNameFul, typeof(Transform), "localRotation.z", curveRotZ[index]);

				curveRotW[index].AddKey(new Keyframe(time, bone.localRotation.w));
				newClip.SetCurve(boneNameFul, typeof(Transform), "localRotation.w", curveRotW[index]);

			}

			if (time > 0)
			{
				if ((curveScaleX[index].Evaluate(time - frameTime) != bone.localScale.x) ||
				   (curveScaleY[index].Evaluate(time - frameTime) != bone.localScale.y) ||
				   (curveScaleZ[index].Evaluate(time - frameTime) != bone.localScale.z))
				{
					curveScaleX[index].AddKey(new Keyframe(time - frameTime, curveScaleX[index].Evaluate(time - frameTime)));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localScale.x", curveScaleX[index]);

					curveScaleY[index].AddKey(new Keyframe(time - frameTime, curveScaleY[index].Evaluate(time - frameTime)));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localScale.y", curveScaleY[index]);

					curveScaleZ[index].AddKey(new Keyframe(time - frameTime, curveScaleZ[index].Evaluate(time - frameTime)));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localScale.z", curveScaleZ[index]);

					curveScaleX[index].AddKey(new Keyframe(time, bone.localScale.x));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localScale.x", curveScaleX[index]);

					curveScaleY[index].AddKey(new Keyframe(time, bone.localScale.y));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localScale.y", curveScaleY[index]);

					curveScaleZ[index].AddKey(new Keyframe(time, bone.localScale.z));
					newClip.SetCurve(boneNameFul, typeof(Transform), "localScale.z", curveScaleZ[index]);
				}

			}
			else
			{

				curveScaleX[index].AddKey(new Keyframe(time, bone.localScale.x));
				newClip.SetCurve(boneNameFul, typeof(Transform), "localScale.x", curveScaleX[index]);

				curveScaleY[index].AddKey(new Keyframe(time, bone.localScale.y));
				newClip.SetCurve(boneNameFul, typeof(Transform), "localScale.y", curveScaleY[index]);

				curveScaleZ[index].AddKey(new Keyframe(time, bone.localScale.z));
				newClip.SetCurve(boneNameFul, typeof(Transform), "localScale.z", curveScaleZ[index]);

			}





		}
		string GetFullName(Transform obj, string name)
		{

			if (obj.parent)
			{
				if (obj.parent.name == transform.name)
				{

					return name;
				}
				else
				{
					name = (obj.parent.name + "/" + name);
					name = GetFullName(obj.parent, name);
					return name;
				}
			}
			else
				return name;

		}
		void SaveAnimationClip(AnimationClip a)
		{
			if (!Directory.Exists(_puppet2DPath + "/Animation/Baked"))
			{
				AssetDatabase.CreateFolder(_puppet2DPath + "/Animation", "Baked");
				AssetDatabase.Refresh();
			}
			string path = AssetDatabase.GenerateUniqueAssetPath(_puppet2DPath + "/Animation/Baked/" + a.name + ".asset");
			AssetDatabase.CreateAsset(a, path);

			//ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;

			//transform.GetComponent<Animator>().
		}
		List<AnimationClip> GetAnimationLengths()
		{
			List<AnimationClip> animationClips = new List<AnimationClip>();
#if (!UNITY_4_3 && !UNITY_4_4 && !UNITY_4_5 && !UNITY_4_6)
			RuntimeAnimatorController controller = transform.GetComponent<Animator>().runtimeAnimatorController;

			for (int i = 0; i < controller.animationClips.Length; i++)
			{
				AnimationClip clip = new AnimationClip();
				// Obviously loading it depends on where/how clip is stored, best case its a resource, worse case you have to search asset database.

				string path = AssetDatabase.GetAssetPath(controller.animationClips[i]);
				clip = (AnimationClip)AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip));

				//clip = (AnimationClip)Resources.LoadAssetAtPath(_puppet2DPath+"/Animation/" + m.GetState(i).GetMotion().name + ".anim", typeof(AnimationClip));
				animationClips.Add(clip);


			}

			return animationClips;
#else
		RuntimeAnimatorController controller = transform.GetComponent<Animator>().runtimeAnimatorController;
		if (controller is UnityEditorInternal.AnimatorController)
		{
			UnityEditorInternal.StateMachine m = ((UnityEditorInternal.AnimatorController)controller).GetLayer(0).stateMachine;

			for (int i = 0; i < m.stateCount; i++)

			{
				AnimationClip clip = new AnimationClip();
				// Obviously loading it depends on where/how clip is stored, best case its a resource, worse case you have to search asset database.
				if (m.GetState(i).GetMotion())
				{
					string path = AssetDatabase.GetAssetPath(m.GetState(i).GetMotion());
					clip = (AnimationClip)Resources.LoadAssetAtPath(path, typeof(AnimationClip));

					//clip = (AnimationClip)Resources.LoadAssetAtPath(_puppet2DPath+"/Animation/" + m.GetState(i).GetMotion().name + ".anim", typeof(AnimationClip));
					animationClips.Add(clip);
				}
				if (clip)

				{

					Debug.Log(clip.name + ", length is " + clip.length);

				}

			}

		}
		return animationClips;
#endif
		}
		private static void RecursivelyFindFolderPath(string dir)
		{
			string[] paths = Directory.GetDirectories(dir);
			foreach (string s in paths)
			{
				if (s.Contains("Puppet2D"))
				{
					_puppet2DPath = s;
					break;
				}
				else
				{
					RecursivelyFindFolderPath(s);
				}
			}
		}

	}

}
#endif