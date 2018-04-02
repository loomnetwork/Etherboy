using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
namespace Puppet2D
{
	public class Puppet2D_Editor : EditorWindow
	{

		public static bool SkinWeightsPaint = false;
		public static Mesh currentSelectionMesh;
		public static GameObject currentSelection;
		public static Color[] previousVertColors;
		public static float EditSkinWeightRadius = 5f;
		public static GameObject paintWeightsBone;
		public static Shader previousShader;
		public static Vector3 ChangeRadiusStartPosition;
		public static float ChangeRadiusStartValue = 0f;
		public static bool ChangingRadius = false;
		public static float paintWeightsStrength = 0.25f;
		public static Color paintControlColor = new Color(.8f, 1f, .8f, .5f);
		public bool showLayerSizePanel = true;
		public string layerSizePanel = "Hide";

		public static bool BoneCreation = false;
		static bool EditSkinWeights = false;
		public static bool SplineCreation = false;
		public static bool FFDCreation = false;

		GameObject currentBone;
		GameObject previousBone;

		public bool ReverseNormals;

		public static string _boneSortingLayer, _controlSortingLayer;
		public static int _boneSortingIndex, _controlSortingIndex, _triangulationIndex, _numberBonesToSkinToIndex = 1, _skinningType;

		public static Sprite boneNoJointSprite = null;
		public static Sprite boneSprite = null;
		public static Sprite boneHiddenSprite = null;
		public static Sprite boneOriginal = null;

		public static GameObject currentActiveBone = null;

		//public static List<Transform> splineCtrls = new List<Transform>();
		public static int numberSplineJoints = 4;

		static public List<Transform> FFDCtrls = new List<Transform>();
		static public List<int> FFDPathNumber = new List<int>();
		private GameObject FFDSprite;
		private Mesh FFDMesh;
		public static GameObject FFDGameObject;

		public static bool BlackAndWhiteWeights;

		[SerializeField]
		static float BoneSize;
		static float ControlSize;
		static float VertexHandleSize;

		private string pngSequPath, checkPath;
		bool recordPngSequence = false;
		private int imageCount = 0;
		private float recordDelta = 0f;
		bool ExportPngAlpha;

		public static List<List<string>> selectedControls = new List<List<string>>();
		public static List<List<string>> selectedControlsData = new List<List<string>>();

		static public string _puppet2DPath;

		static public bool HasGuides = false;

		public enum GUIChoice
		{
			AutoRig,
			BoneCreation,
			RigginSetup,
			Skinning,
			Animation,
			About,
		}
		GUIChoice currentGUIChoice;

		[MenuItem("GameObject/Puppet2D/Window/Puppet2D")]
		[MenuItem("Window/Puppet2D")]
		static void Init()
		{
			Puppet2D_Editor window = (Puppet2D_Editor)EditorWindow.GetWindow(typeof(Puppet2D_Editor));
			window.Show();
		}
		void OnEnable()
		{

			RecursivelyFindFolderPath("Assets");

			BoneSize = EditorPrefs.GetFloat("Puppet2D_EditorBoneSize", 0.85f);
			ControlSize = EditorPrefs.GetFloat("Puppet2D_EditorControlSize", 0.85f);
			VertexHandleSize = EditorPrefs.GetFloat("Puppet2D_EditorVertexHandleSize", 0.8f);
			BoneCreation = EditorPrefs.GetBool("Puppet2D_BoneCreation", false);

			_boneSortingIndex = EditorPrefs.GetInt("Puppet2D_BoneLayer", 0);
			_controlSortingIndex = EditorPrefs.GetInt("Puppet2D_ControlLayer", 0);


			Puppet2D_Selection.GetSelectionString();

		}

		void OnGUI()
		{
			string path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/BoneNoJoint.psd");
			string path2 = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/BoneScaled.psd");
			string path3 = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/BoneJoint.psd");
			string path4 = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/Bone.psd");
			boneNoJointSprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			boneSprite = AssetDatabase.LoadAssetAtPath(path2, typeof(Sprite)) as Sprite;
			boneHiddenSprite = AssetDatabase.LoadAssetAtPath(path3, typeof(Sprite)) as Sprite;
			boneOriginal = AssetDatabase.LoadAssetAtPath(path4, typeof(Sprite)) as Sprite;
			Texture aTexture = AssetDatabase.LoadAssetAtPath(Puppet2D_Editor._puppet2DPath + "/Textures/GUI/GUI_Bones.png", typeof(Texture)) as Texture;
			Texture puppetManTexture = AssetDatabase.LoadAssetAtPath(Puppet2D_Editor._puppet2DPath + "/Textures/GUI/GUI_puppetman.png", typeof(Texture)) as Texture;
			Texture rigTexture = AssetDatabase.LoadAssetAtPath(Puppet2D_Editor._puppet2DPath + "/Textures/GUI/GUI_Rig.png", typeof(Texture)) as Texture;
			Texture ControlTexture = AssetDatabase.LoadAssetAtPath(Puppet2D_Editor._puppet2DPath + "/Textures/GUI/parentControl.psd", typeof(Texture)) as Texture;
			Texture VertexTexture = AssetDatabase.LoadAssetAtPath(Puppet2D_Editor._puppet2DPath + "/Textures/GUI/VertexHandle.psd", typeof(Texture)) as Texture;
			Texture autoRigTexture = AssetDatabase.LoadAssetAtPath(Puppet2D_Editor._puppet2DPath + "/Textures/GUI/autoRigRobot.png", typeof(Texture)) as Texture;
			Texture autoRigGuidesTexture = AssetDatabase.LoadAssetAtPath(Puppet2D_Editor._puppet2DPath + "/Textures/GUI/autoRigRobotGuides.png", typeof(Texture)) as Texture;


			string[] sortingLayers = GetSortingLayerNames();
			Color bgColor = GUI.backgroundColor;

			if (currentGUIChoice == GUIChoice.BoneCreation)
				GUI.backgroundColor = Color.green;

			if (GUI.Button(new Rect(0, 0, 100, 20), "Skeleton"))
			{
				currentGUIChoice = GUIChoice.BoneCreation;
			}

			GUI.backgroundColor = bgColor;
			if (currentGUIChoice == GUIChoice.RigginSetup)
				GUI.backgroundColor = Color.green;

			if (GUI.Button(new Rect(100, 0, 100, 20), "Rigging"))
			{
				currentGUIChoice = GUIChoice.RigginSetup;
			}
			GUI.backgroundColor = bgColor;
			if (currentGUIChoice == GUIChoice.Skinning)
				GUI.backgroundColor = Color.green;

			if (GUI.Button(new Rect(200, 0, 100, 20), "Skinning"))
			{
				currentGUIChoice = GUIChoice.Skinning;
			}
			GUI.backgroundColor = bgColor;
			if (currentGUIChoice == GUIChoice.Animation)
				GUI.backgroundColor = Color.green;

			if (GUI.Button(new Rect(0, 20, 100, 20), "Animation"))
			{
				currentGUIChoice = GUIChoice.Animation;
			}
			GUI.backgroundColor = bgColor;

			if (currentGUIChoice == GUIChoice.AutoRig)
				GUI.backgroundColor = Color.green;
			if (GUI.Button(new Rect(100, 20, 100, 20), "Auto Rig"))
			{
				currentGUIChoice = GUIChoice.AutoRig;
			}
			GUI.backgroundColor = bgColor;
			if (currentGUIChoice == GUIChoice.About)
				GUI.backgroundColor = Color.green;
			if (GUI.Button(new Rect(200, 20, 100, 20), "About"))
			{
				currentGUIChoice = GUIChoice.About;
			}
			GUI.backgroundColor = bgColor;


			if (EditSkinWeights || SplineCreation || FFDCreation)
				GUI.backgroundColor = Color.grey;

			showLayerSizePanel = EditorGUI.Foldout(new Rect(5, 40, 15, 15), showLayerSizePanel, layerSizePanel);
			int offsetControls = 130;

			if (showLayerSizePanel)
			{

				offsetControls = 130;
				layerSizePanel = "Close";
				GUI.DrawTexture(new Rect(25, 60, 32, 32), boneSprite.texture, ScaleMode.StretchToFill, true, 10.0F);

				EditorGUI.BeginChangeCheck();
				BoneSize = EditorGUI.Slider(new Rect(80, 60, 150, 20), BoneSize, 0F, 0.9999F);
				if (EditorGUI.EndChangeCheck())
				{
					ChangeBoneSize();
					EditorPrefs.SetFloat("Puppet2D_EditorBoneSize", BoneSize);
				}
				EditorGUI.BeginChangeCheck();
				_boneSortingIndex = EditorGUI.Popup(new Rect(80, 90, 150, 30), _boneSortingIndex, sortingLayers);
				if (EditorGUI.EndChangeCheck())
				{
					EditorPrefs.SetInt("Puppet2D_BoneLayer", _boneSortingIndex);
				}
				if (sortingLayers.Length <= _boneSortingIndex)
				{
					_boneSortingIndex = 0;
					EditorPrefs.SetInt("Puppet2D_BoneLayer", _boneSortingIndex);
				}
				_boneSortingLayer = sortingLayers[_boneSortingIndex];


				GUI.DrawTexture(new Rect(25, 110, 32, 32), ControlTexture, ScaleMode.StretchToFill, true, 10.0F);

				EditorGUI.BeginChangeCheck();
				ControlSize = EditorGUI.Slider(new Rect(80, 120, 150, 20), ControlSize, 0F, .9999F);
				if (EditorGUI.EndChangeCheck())
				{
					ChangeControlSize();
					EditorPrefs.SetFloat("Puppet2D_EditorControlSize", ControlSize);
				}
				EditorGUI.BeginChangeCheck();
				_controlSortingIndex = EditorGUI.Popup(new Rect(80, 150, 150, 30), _controlSortingIndex, sortingLayers);
				if (EditorGUI.EndChangeCheck())
				{
					EditorPrefs.SetInt("Puppet2D_ControlLayer", _controlSortingIndex);
				}
				if (sortingLayers.Length <= _controlSortingIndex)
				{
					_controlSortingIndex = 0;
					EditorPrefs.SetInt("Puppet2D_ControlLayer", _controlSortingIndex);
				}
				_controlSortingLayer = sortingLayers[_controlSortingIndex];


				//GUI.DrawTexture(new Rect(15, 160, 275, 5), aTexture, ScaleMode.StretchToFill, true, 10.0F);
			}
			else
			{
				layerSizePanel = "Open";
				offsetControls = 30;
			}
			GUILayout.Space(offsetControls + 50);
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });


			if (currentGUIChoice == GUIChoice.BoneCreation)
			{
				//GUILayout.Label("Bone Creation", EditorStyles.boldLabel);

				GUILayout.Space(15);
				GUI.DrawTexture(new Rect(0, 60 + offsetControls, 64, 128), aTexture, ScaleMode.StretchToFill, true, 10.0F);
				GUILayout.Space(15);


				if (BoneCreation)
					GUI.backgroundColor = Color.green;


				if (GUI.Button(new Rect(80, 60 + offsetControls, 150, 30), "Create Bone Tool"))
				{
					BoneCreation = true;
					currentActiveBone = null;
					EditorPrefs.SetBool("Puppet2D_BoneCreation", BoneCreation);

				}
				if (BoneCreation)
					GUI.backgroundColor = bgColor;


				if (GUI.Button(new Rect(80, 90 + offsetControls, 150, 30), "Finish Bone"))
				{
					Puppet2D_BoneCreation.BoneFinishCreation();
				}

				if (BoneCreation)
					GUI.backgroundColor = Color.grey;



				if (SplineCreation)
				{
					GUI.backgroundColor = Color.green;
				}
				if (GUI.Button(new Rect(80, 150 + offsetControls, 150, 30), "Create Spline Tool"))
				{
					//Puppet2D_Spline.splineStoreData.FFDCtrls.Clear();
					//SplineCreation = true; 
					Puppet2D_Spline.CreateSplineTool();
				}
				if (SplineCreation)
				{
					GUI.backgroundColor = bgColor;
				}
				numberSplineJoints = EditorGUI.IntSlider(new Rect(80, 190 + offsetControls, 150, 20), numberSplineJoints, 1, 10);

				if (GUI.Button(new Rect(80, 220 + offsetControls, 150, 30), "Finish Spline"))
				{
					Puppet2D_Spline.SplineFinishCreation();

				}
			}
			if (currentGUIChoice == GUIChoice.RigginSetup)
			{
				// GUILayout.Label("Rigging Setup", EditorStyles.boldLabel);

				GUI.DrawTexture(new Rect(0, 60 + offsetControls, 64, 128), rigTexture, ScaleMode.StretchToFill, true, 10.0F);
				if (GUI.Button(new Rect(80, 60 + offsetControls, 150, 30), "Create IK Control"))
				{
					Puppet2D_CreateControls.IKCreateTool();

				}
				if (GUI.Button(new Rect(80, 90 + offsetControls, 150, 30), "Create Parent Control"))
				{
					Puppet2D_CreateControls.CreateParentControl();

				}
				if (GUI.Button(new Rect(80, 120 + offsetControls, 150, 30), "Create Orient Control"))
				{
					Puppet2D_CreateControls.CreateOrientControl();

				}

				GUI.DrawTexture(new Rect(15, 200 + offsetControls, 275, 5), aTexture, ScaleMode.StretchToFill, true, 10.0F);



			}
			if (currentGUIChoice == GUIChoice.Skinning)
			{
				//GUILayout.Label("Skinning", EditorStyles.boldLabel);

				GUI.DrawTexture(new Rect(0, 50 + offsetControls, 64, 128), puppetManTexture, ScaleMode.StretchToFill, true, 10.0F);

				GUILayout.Space(5);
				GUIStyle labelNew = EditorStyles.label;
				labelNew.alignment = TextAnchor.LowerLeft;
				labelNew.contentOffset = new Vector2(80, 0);
				GUILayout.Label("Type of Mesh: ", labelNew);
				labelNew.contentOffset = new Vector2(0, 0);
				string[] TriangulationTypes = { "0", "1", "2", "3", "4", "5" };

				_triangulationIndex = EditorGUI.Popup(new Rect(180, 60 + offsetControls, 50, 30), _triangulationIndex, TriangulationTypes);


				if (GUI.Button(new Rect(80, 80 + offsetControls, 150, 30), "Convert Sprite To Mesh"))
				{
					Puppet2D_Skinning.ConvertSpriteToMesh(_triangulationIndex);
				}
				if (GUI.Button(new Rect(80, 110 + offsetControls, 150, 30), "Parent Object To Bones"))
				{
					Puppet2D_Skinning.BindRigidSkin();

				}
				GUILayout.Space(70);
				labelNew.alignment = TextAnchor.LowerLeft;
				labelNew.contentOffset = new Vector2(80, 0);
				GUILayout.Label("Num Skin Bones: ", labelNew);
				labelNew.contentOffset = new Vector2(0, 0);
				string[] NumberBonesToSkinTo = { "1", "2", "4 (FFD)" };

				_numberBonesToSkinToIndex = EditorGUI.Popup(new Rect(180, 150 + offsetControls, 50, 30), _numberBonesToSkinToIndex, NumberBonesToSkinTo);

				if (GUI.Button(new Rect(80, 200 + offsetControls, 150, 30), "Bind Smooth Skin"))
				{
					if (_numberBonesToSkinToIndex == 1)
						Puppet2D_Skinning.BindSmoothSkin(_skinningType);
					else
						Puppet2D_Skinning.BindSmoothSkin(1);
				}
				if (EditSkinWeights || SkinWeightsPaint)
				{
					GUI.backgroundColor = Color.green;
				}
				string[] SkinningTypeChoice = { "Heat Map", "Closest Point" };
				GUILayout.Space(10);
				labelNew.alignment = TextAnchor.LowerLeft;
				labelNew.contentOffset = new Vector2(80, 0);
				GUILayout.Label("Skinning: ", labelNew);
				labelNew.contentOffset = new Vector2(0, 0);
				_skinningType = EditorGUI.Popup(new Rect(140, 175 + offsetControls, 90, 30), _skinningType, SkinningTypeChoice);


				if (SkinWeightsPaint)
				{
					if (GUI.Button(new Rect(80, 230 + offsetControls, 150, 30), "Manually Edit Weights"))
					{
						// finish paint weights
						Selection.activeGameObject = currentSelection;
						if (currentSelection)
						{
							if (previousShader)
								currentSelection.GetComponent<Renderer>().sharedMaterial.shader = previousShader;
							SkinWeightsPaint = false;
							if (previousVertColors != null && previousVertColors.Length > 0)
								currentSelectionMesh.colors = previousVertColors;
							currentSelectionMesh = null;
							currentSelection = null;
							previousVertColors = null;
						}

						EditSkinWeights = Puppet2D_Skinning.EditWeights();

					}
				}
				if (!SkinWeightsPaint)
				{
					if (GUI.Button(new Rect(80, 230 + offsetControls, 150, 30), "Paint Weights"))
					{
						if (EditSkinWeights)
						{
							EditSkinWeights = false;
							Object[] bakedMeshes = Puppet2D_Skinning.FinishEditingWeights();

							Selection.objects = bakedMeshes;
						}

						if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>() && Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh)
						{
							SkinWeightsPaint = true;
							SkinnedMeshRenderer smr = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
							currentSelectionMesh = smr.sharedMesh;
							currentSelection = Selection.activeGameObject;
							previousShader = currentSelection.GetComponent<Renderer>().sharedMaterial.shader;
							currentSelection.GetComponent<Renderer>().sharedMaterial.shader = Shader.Find("Puppet2D/vertColor");



							if (currentSelectionMesh.colors.Length != currentSelectionMesh.vertices.Length)
							{
								currentSelectionMesh.colors = new Color[currentSelectionMesh.vertices.Length];
								EditorUtility.SetDirty(currentSelection);
								EditorUtility.SetDirty(currentSelectionMesh);
								AssetDatabase.SaveAssets();
								AssetDatabase.SaveAssets();
							}
							else
								previousVertColors = currentSelectionMesh.colors;
							Selection.activeGameObject = smr.bones[0].gameObject;
						}
					}
				}



				if (EditSkinWeights || SkinWeightsPaint)
					GUI.backgroundColor = bgColor;

				if (GUI.Button(new Rect(80, 260 + offsetControls, 150, 30), "Finish Edit Skin Weights"))
				{
					if (SkinWeightsPaint)
					{
						if (currentSelection)
						{
							Selection.activeGameObject = currentSelection;

							if (previousShader)
								currentSelection.GetComponent<Renderer>().sharedMaterial.shader = previousShader;
							SkinWeightsPaint = false;
							if (previousVertColors != null && previousVertColors.Length > 0)
								currentSelectionMesh.colors = previousVertColors;
							currentSelectionMesh = null;
							currentSelection = null;
							previousVertColors = null;

							Puppet2D_HiddenBone[] hiddenBones = Transform.FindObjectsOfType<Puppet2D_HiddenBone>();
							foreach (Puppet2D_HiddenBone hiddenBone in hiddenBones)
							{
								hiddenBone.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
								if (hiddenBone.transform.parent != null)
									hiddenBone.transform.parent.GetComponent<SpriteRenderer>().color = Color.white;

							}

						}
						else
							SkinWeightsPaint = false;
					}
					else
					{
						EditSkinWeights = false;
						Puppet2D_Skinning.FinishEditingWeights();
					}

				}
				float SkinWeightsPaintOffset = -80;

				if (EditSkinWeights)
				{
					SkinWeightsPaintOffset = -40;
					GUI.DrawTexture(new Rect(25, 290 + offsetControls, 32, 32), VertexTexture, ScaleMode.StretchToFill, true, 10.0F);
					EditorGUI.BeginChangeCheck();
					VertexHandleSize = EditorGUI.Slider(new Rect(80, 310 + offsetControls, 150, 20), VertexHandleSize, 0F, .9999F);
					if (EditorGUI.EndChangeCheck())
					{
						ChangeVertexHandleSize();
						EditorPrefs.SetFloat("Puppet2D_EditorVertexHandleSize", VertexHandleSize);
					}
				}
				if (SkinWeightsPaint)
				{
					SkinWeightsPaintOffset = 0;

					GUILayout.Space(offsetControls - 20);
					GUILayout.Label(" Brush Size", EditorStyles.boldLabel);
					EditSkinWeightRadius = EditorGUI.Slider(new Rect(80, 305 + offsetControls, 150, 20), EditSkinWeightRadius, 0F, 100F);
					GUILayout.Label(" Strength", EditorStyles.boldLabel);
					paintWeightsStrength = EditorGUI.Slider(new Rect(80, 335 + offsetControls, 150, 20), paintWeightsStrength, 0F, 1F);



					EditorGUI.BeginChangeCheck();
					BlackAndWhiteWeights = EditorGUI.Toggle(new Rect(5, 365 + offsetControls, 200, 20), "BlackAndWhite", BlackAndWhiteWeights);
					if (EditorGUI.EndChangeCheck())
					{
						RefreshMeshColors();

					}
				}

				if (EditSkinWeights || SkinWeightsPaint)
					GUI.backgroundColor = Color.grey;

				if (FFDCreation)
					GUI.backgroundColor = Color.green;

				if (GUI.Button(new Rect(80, 390 + offsetControls + SkinWeightsPaintOffset, 150, 30), "Create FFD Tool"))
				{
					//				if (FindObjectOfType<Puppet2D_FFDStoreData> () == null)
					//					FFDCreation = false;

					// RECREATE FFD CHECK
					if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>())
					{
						SkinnedMeshRenderer smr = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();


						Puppet2D_FFDStoreData[] storeDatas = FindObjectsOfType<Puppet2D_FFDStoreData>();
						foreach (Puppet2D_FFDStoreData storeData in storeDatas)
						{
							foreach (Transform ffdctrl in storeData.FFDCtrls)
							{
								if (ffdctrl.GetComponentInChildren<Puppet2D_FFDLineDisplay>().outputSkinnedMesh == smr)
								{
									storeData.Editable = true;

									GameObject tempSprite = new GameObject();
									Undo.RegisterCreatedObjectUndo(tempSprite, "tempSprite");
									SpriteRenderer spriteRender = tempSprite.AddComponent<SpriteRenderer>();

									Sprite thisSprite = null;// = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(smr.sharedMaterial.mainTexture) ,typeof(Sprite)) as Sprite;
									Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(smr.sharedMaterial.mainTexture)).OfType<Sprite>().ToArray();
									foreach (Sprite s in sprites)
									{
										if (s.name == ffdctrl.GetComponentInChildren<Puppet2D_FFDLineDisplay>().outputSkinnedMesh.name)
										{
											thisSprite = s;
											break;
										}
									}
									if (thisSprite == null)
									{
										DestroyImmediate(tempSprite);
										return;
									}
									//Debug.Log (AssetDatabase.GetAssetPath (smr.sharedMaterial.mainTexture));
									spriteRender.sprite = thisSprite;
									tempSprite.transform.position = storeData.transform.position;
									tempSprite.name = smr.gameObject.name;
									Undo.DestroyObjectImmediate(smr.gameObject);

									Selection.activeGameObject = storeData.FFDCtrls[storeData.FFDCtrls.Count - 1].gameObject;

									FFDGameObject = tempSprite;
									break;
								}
							}
						}

					}



					if (FindObjectOfType<Puppet2D_FFDStoreData>() == null)
					{

						StartFFDCreation();
					}
					else
					{
						bool FFDExistsInScene = false;
						foreach (Puppet2D_FFDStoreData data in FindObjectsOfType<Puppet2D_FFDStoreData>())
						{
							if (data.Editable == true)
							{
								Puppet2D_FFD.FFDControlsGrp = data.gameObject;

								FFDExistsInScene = true;
								if (data.FFDPathNumber.Count > 0)
									data.FFDPathNumber.RemoveAt(data.FFDPathNumber.Count - 1);
								if (data.FFDCtrls.Count > 0 && data.FFDCtrls[data.FFDCtrls.Count - 1])
									data.FFDCtrls[data.FFDCtrls.Count - 1].GetComponent<Puppet2D_FFDLineDisplay>().target2 = null;
								Puppet2D_FFD.ffdStoreData = data;
								FFDCreation = true;
							}
						}
						if (!FFDExistsInScene)
						{
							StartFFDCreation();

						}

					}

				}
				if (GUI.Button(new Rect(80, 470 + offsetControls + SkinWeightsPaintOffset, 150, 30), "Sort Bones"))
				{
					Puppet2D_Skinning.SortVertices();
				}
				if (FFDCreation)
					GUI.backgroundColor = bgColor;
				if (GUI.Button(new Rect(80, 420 + offsetControls + SkinWeightsPaintOffset, 150, 30), "Finish FFD"))
				{
					FFDCreation = false;
					Puppet2D_FFD.FFDFinishCreation();
				}

			}
			if (currentGUIChoice == GUIChoice.Animation)
			{
				//GUILayout.Label("Animation", EditorStyles.boldLabel);

				if (GUI.Button(new Rect(80, 60 + offsetControls, 150, 30), "Bake Animation"))
				{
					Puppet2D_GlobalControl[] globalCtrlScripts = Transform.FindObjectsOfType<Puppet2D_GlobalControl>();
					for (int i = 0; i < globalCtrlScripts.Length; i++)
					{
						Puppet2D_BakeAnimation BakeAnim = globalCtrlScripts[i].gameObject.AddComponent<Puppet2D_BakeAnimation>();
						BakeAnim.Run();
						DestroyImmediate(BakeAnim);
						globalCtrlScripts[i].enabled = false;
					}
				}
				if (recordPngSequence && !ExportPngAlpha)
					GUI.backgroundColor = Color.green;
				if (GUI.Button(new Rect(80, 130 + offsetControls, 150, 30), "Render Animation"))
				{
					checkPath = EditorUtility.SaveFilePanel("Choose Directory", pngSequPath, "exportedAnim", "");
					if (checkPath != "")
					{
						pngSequPath = checkPath;
						recordPngSequence = true;
						EditorApplication.ExecuteMenuItem("Edit/Play");
					}
				}
				GUI.backgroundColor = bgColor;

				if (ExportPngAlpha || recordPngSequence)
					GUI.backgroundColor = bgColor;
				if (GUI.Button(new Rect(80, 200 + offsetControls, 150, 30), "Save Selection"))
				{
					selectedControls.Add(new List<string>());
					selectedControlsData.Add(new List<string>());

					foreach (GameObject go in Selection.gameObjects)
					{
						selectedControls[selectedControls.Count - 1].Add(Puppet2D_Selection.GetGameObjectPath(go));
						selectedControlsData[selectedControlsData.Count - 1].Add(go.transform.localPosition.x + " " + go.transform.localPosition.y + " " + go.transform.localPosition.z + " " + go.transform.localRotation.x + " " + go.transform.localRotation.y + " " + go.transform.localRotation.z + " " + go.transform.localRotation.w + " " + go.transform.localScale.x + " " + go.transform.localScale.y + " " + go.transform.localScale.z + " ");

					}
					Puppet2D_Selection.SetSelectionString();
				}
				if (GUI.Button(new Rect(80, 230 + offsetControls, 150, 30), "Clear Selections"))
				{
					selectedControls.Clear();
					selectedControlsData.Clear();
					Puppet2D_Selection.SetSelectionString();
				}


				for (int i = 0; i < selectedControls.Count; i++)
				{
					int column = i % 3;
					int row = 0;

					row = i / 3;
					Rect newLoadButtonPosition = new Rect(80 + (50 * column), 265 + offsetControls + row * 30, 50, 30);

					if (Event.current.type == EventType.ContextClick)
					{
						Vector2 mousePos = Event.current.mousePosition;
						if ((Event.current.button == 1) && newLoadButtonPosition.Contains(mousePos))
						{
							GenericMenu menu = new GenericMenu();

							menu.AddItem(new GUIContent("Select Objects"), false, Puppet2D_Selection.SaveSelectionLoad, i);
							menu.AddItem(new GUIContent("Remove Selection"), false, Puppet2D_Selection.SaveSelectionRemove, i);
							menu.AddItem(new GUIContent("Append Selection"), false, Puppet2D_Selection.SaveSelectionAppend, i);
							menu.AddItem(new GUIContent("Store Pose"), false, Puppet2D_Selection.StorePose, i);
							menu.AddItem(new GUIContent("Load Pose"), false, Puppet2D_Selection.LoadPose, i);



							menu.ShowAsContext();
							Event.current.Use();

						}

					}
					GUI.Box(newLoadButtonPosition, "Load");
					/*if (GUI.Button(newLoadButtonPosition, "Load"))
					{
						Selection.objects = selectedControls[i].ToArray();
					}*/
				}


			}
			if (currentGUIChoice == GUIChoice.AutoRig)
			{



				if (!HasGuides)
					GUI.DrawTexture(new Rect(20, 90 + offsetControls, 256, 256), autoRigTexture, ScaleMode.StretchToFill, true, 10.0F);
				else
					GUI.DrawTexture(new Rect(20, 90 + offsetControls, 256, 256), autoRigGuidesTexture, ScaleMode.StretchToFill, true, 10.0F);



				if (GUI.Button(new Rect(80, 70 + offsetControls, 150, 30), "Make Guides"))
				{
					Puppet2D_AutoRig.MakeGuides();
					HasGuides = true;
				}
				if (GUI.Button(new Rect(80, 350 + offsetControls, 150, 30), "Auto Rig"))
				{
					Puppet2D_AutoRig.AutoRig();
					ControlSize = 0.96f;
					ChangeControlSize();
					HasGuides = false;


				}
				if (GUI.Button(new Rect(80, 400 + offsetControls, 150, 30), "Get More Animations..."))
				{
					Application.OpenURL("http://www.puppet2d.com/animationpacks/");

				}
			}
			if (currentGUIChoice == GUIChoice.About)
			{
				GUILayout.Space(30);
				GUILayout.Label("    Puppet2D by Puppetman.\n\n    Version 3.4", EditorStyles.boldLabel);
			}
		}
		void OnFocus()
		{

			SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

			SceneView.onSceneGUIDelegate += this.OnSceneGUI;
		}

		void OnDestroy()
		{

			SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

			EditorPrefs.SetFloat("Puppet2D_EditorBoneSize", BoneSize);
			EditorPrefs.SetFloat("Puppet2D_EditorControlSize", ControlSize);
			EditorPrefs.SetFloat("Puppet2D_EditorVertexHandleSize", VertexHandleSize);

			Puppet2D_Selection.SetSelectionString();
		}

		void OnSceneGUI(SceneView sceneView)
		{
			Event e = Event.current;

			switch (e.type)
			{
				case EventType.keyDown:
					{
						if (Event.current.keyCode == (KeyCode.Return))
						{
							if (BoneCreation)
								Puppet2D_BoneCreation.BoneFinishCreation();
							if (SplineCreation)
								Puppet2D_Spline.SplineFinishCreation();
							if (FFDCreation)
							{
								FFDCreation = false;
								Puppet2D_FFD.FFDFinishCreation();
							}
							Repaint();

						}
						if (Event.current.keyCode == (KeyCode.KeypadPlus) && SkinWeightsPaint)
						{
							EditSkinWeightRadius += 0.2f;

						}
						if (Event.current.keyCode == (KeyCode.KeypadMinus) && SkinWeightsPaint)
						{
							EditSkinWeightRadius -= 0.2f;
						}
						if (BoneCreation)
						{
							if (Event.current.keyCode == (KeyCode.Backspace))
							{
								Puppet2D_BoneCreation.BoneDeleteMode();
							}
						}
						if (SkinWeightsPaint)
						{

							if (Event.current.keyCode == (KeyCode.N))
							{

								Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

								if (!ChangingRadius)
								{
									ChangeRadiusStartPosition = worldRay.GetPoint(0);
									ChangeRadiusStartValue = paintWeightsStrength;
								}

								Puppet2D_Skinning.ChangePaintStrength(worldRay.GetPoint(0));
								ChangingRadius = true;

							}
							if (Event.current.keyCode == (KeyCode.B))
							{

								Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

								if (!ChangingRadius)
								{
									ChangeRadiusStartPosition = worldRay.GetPoint(0);
									ChangeRadiusStartValue = EditSkinWeightRadius;
								}

								Puppet2D_Skinning.ChangePaintRadius(worldRay.GetPoint(0));
								ChangingRadius = true;


							}
						}
						break;
					}
				case EventType.mouseMove:
					{
						if (Event.current.button == 0)
						{

							if (BoneCreation)
							{
								Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

								if (Event.current.control == true)
								{
									Puppet2D_BoneCreation.BoneMoveMode(worldRay.GetPoint(0));
								}
								if (Event.current.shift == true)
								{
									Puppet2D_BoneCreation.BoneMoveIndividualMode(worldRay.GetPoint(0));
								}

							}
							if (FFDCreation || SplineCreation)
							{
								Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

								if ((Event.current.control == true) || (Event.current.shift == true))
								{
									MoveControl(worldRay.GetPoint(0));
								}
							}

						}
						break;
					}
				case EventType.MouseDown:
					{

						if (Event.current.button == 0)
						{

							if (BoneCreation)
							{
								Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
								int controlID = GUIUtility.GetControlID(FocusType.Passive);
								HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
								GameObject c = HandleUtility.PickGameObject(Event.current.mousePosition, true);
								if (c)
								{
									Selection.activeGameObject = c;
								}
								else
								{
									if (Event.current.alt)
										Puppet2D_BoneCreation.BoneAddMode(worldRay.GetPoint(0));
									else
										Puppet2D_BoneCreation.BoneCreationMode(worldRay.GetPoint(0));
								}
								HandleUtility.AddDefaultControl(controlID);



							}
							else if (SplineCreation)
							{
								Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

								Puppet2D_Spline.SplineCreationMode(worldRay.GetPoint(0));
							}
							else if (FFDCreation)
							{
								if (FindObjectOfType<Puppet2D_FFDStoreData>() == null)
								{
									FFDCreation = false;
								}
								else
								{
									Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

									Puppet2D_FFD.FFDCreationMode(worldRay.GetPoint(0));
								}
							}
							else if (SkinWeightsPaint)
							{


								GameObject c = HandleUtility.PickGameObject(Event.current.mousePosition, true);
								if (c && c.GetComponent<SpriteRenderer>() && c.GetComponent<SpriteRenderer>().sprite && c.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
								{
									Selection.activeGameObject = c;
								}
							}


						}

						else if (Event.current.button == 1)
						{
							if (BoneCreation)
							{
								Puppet2D_BoneCreation.BoneFinishCreation();
								Selection.activeObject = null;
								currentActiveBone = null;
								BoneCreation = true;

							}
							else if (FFDCreation)
							{
								Puppet2D_FFD.CloseFFDPath();
							}
						}
						break;

					}
				case EventType.keyUp:
					{
						if (Event.current.keyCode == (KeyCode.B) || Event.current.keyCode == (KeyCode.N))
						{
							if (SkinWeightsPaint)
							{
								ChangingRadius = false;

							}
						}
						break;
					}
				case EventType.mouseDrag:
					{
						paintControlColor = new Color(.8f, 1f, .8f, .5f);



						if (SkinWeightsPaint)
						{

							Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
							if (Event.current.control == true && Event.current.shift == false)
							{
								paintControlColor = new Color(1f, .8f, .8f, .5f);
								if (Event.current.button == 0)
									Puppet2D_Skinning.PaintWeights(worldRay.GetPoint(0), -1);
							}
							else if (Event.current.shift == true && Event.current.control == true)
							{
								paintControlColor = new Color(.8f, .8f, 1f, .5f);

								if (Event.current.button == 0)
									Puppet2D_Skinning.PaintSmoothWeightsOld(worldRay.GetPoint(0));

							}
							else if (Event.current.shift == true)
							{
								paintControlColor = new Color(.8f, .5f, 1f, .5f);

								if (Event.current.button == 0)
									Puppet2D_Skinning.PaintSmoothWeights(worldRay.GetPoint(0));

							}
							else
							{
								paintControlColor = new Color(.8f, 1f, .8f, .5f);
								if (Event.current.button == 0)
									Puppet2D_Skinning.PaintWeights(worldRay.GetPoint(0), 1);
							}

						}


						break;
					}
			}
			if (SkinWeightsPaint)
			{
				if ((Event.current.type == EventType.ValidateCommand &&
					Event.current.commandName == "UndoRedoPerformed"))
				{

					RefreshMeshColors();

				}


				Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				if (ChangingRadius)
					Puppet2D_Skinning.DrawHandle(ChangeRadiusStartPosition);
				else
					Puppet2D_Skinning.DrawHandle(worldRay.GetPoint(0));
				Repaint();
				SceneView.RepaintAll();

				int controlID = GUIUtility.GetControlID(FocusType.Passive);
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));



				HandleUtility.AddDefaultControl(controlID);


			}
			// Do your drawing here using Handles.

			GameObject[] selection = Selection.gameObjects;

			Handles.BeginGUI();
			if (BoneCreation)
			{
				if (selection.Length > 0)
				{
					Handles.color = Color.blue;
					Handles.Label(selection[0].transform.position + new Vector3(2, 2, 0),
								  "Left Click To Draw Bones\nPress Enter To Finish.\nBackspace To Delete A Bone\nHold Shift To Move Individual Bone\nHold Ctrl To Move Bone & Hierachy\nAlt Left Click To Add A Bone In Chain\nRight Click To Deselect");
				}
				else
				{
					Handles.color = Color.blue;
					Handles.Label(SceneView.lastActiveSceneView.camera.transform.position + Vector3.forward * 2,
								  "Bone Create Mode.\nLeft Click to Draw Bones.\nOr click on a bone to be a parent");
				}

			}
			if (SkinWeightsPaint)
			{
				Handles.color = Color.blue;
				Handles.Label(new Vector3(20, -40, 0),
							  "Select Bones to paint their Weights\n" +
							  "Left Click Adds Weights\n" +
							  "Left Click & Ctrl Removes Weights\n" +
							  "Left Click & Shift Smooths Weights\n" +
							  "Hold B to Change Brush Size\n" +
							  "Hold N to Change Strength");

			}
			// Do your drawing here using GUI.
			Handles.EndGUI();

		}








		void ChangeBoneSize()
		{
			string path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/BoneNoJoint.psd");
			Sprite sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite)) as TextureImporter;
#if (!UNITY_4_3 && !UNITY_4_4 && !UNITY_4_5 && !UNITY_4_6)

			textureImporter.spritePixelsPerUnit = (1 - BoneSize) * (1 - BoneSize) * 1000f;
#else
			textureImporter.spritePixelsToUnits = (1-BoneSize)*(1-BoneSize)*1000f;
#endif
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

		}

		static void StartFFDCreation()
		{
			if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<SpriteRenderer>() && Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite && !Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite.name.Contains("bone") && !Selection.activeGameObject.GetComponent<Puppet2D_FFDLineDisplay>())
				FFDGameObject = Selection.activeGameObject;
			else
				if (!EditorUtility.DisplayDialog("Are you sure?", "You haven't selected a sprite. Do you want to create a new FFD mesh without a sprite?", "Yes", "No"))
			{
				return;
			}
			FFDCreation = true;
			Puppet2D_FFD.FFDSetFirstPath();
		}

		void ChangeControlSize()
		{
			string path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/IKControl.psd");
			Sprite sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite)) as TextureImporter;
#if (!UNITY_4_3 && !UNITY_4_4 && !UNITY_4_5 && !UNITY_4_6)

			textureImporter.spritePixelsPerUnit = (1 - ControlSize) * (1 - ControlSize) * 1000f;
#else
			textureImporter.spritePixelsToUnits = (1-ControlSize)*(1-ControlSize)*1000f;
#endif

			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

			path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/orientControl.psd");
			sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite)) as TextureImporter;
#if (!UNITY_4_3 && !UNITY_4_4 && !UNITY_4_5 && !UNITY_4_6)

			textureImporter.spritePixelsPerUnit = (1 - ControlSize) * (1 - ControlSize) * 1000f;
#else
			textureImporter.spritePixelsToUnits = (1-ControlSize)*(1-ControlSize)*1000f;
#endif
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

			path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/parentControl.psd");
			sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite)) as TextureImporter;
#if (!UNITY_4_3 && !UNITY_4_4 && !UNITY_4_5 && !UNITY_4_6)
			textureImporter.spritePixelsPerUnit = (1 - ControlSize) * (1 - ControlSize) * 1000f;
#else
			textureImporter.spritePixelsToUnits = (1-ControlSize)*(1-ControlSize)*1000f;
#endif
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

			path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/splineControl.psd");
			sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite)) as TextureImporter;
#if (!UNITY_4_3 && !UNITY_4_4 && !UNITY_4_5 && !UNITY_4_6)
			textureImporter.spritePixelsPerUnit = (1 - ControlSize) * (1 - ControlSize) * 1000f;
#else
			textureImporter.spritePixelsToUnits = (1-ControlSize)*(1-ControlSize)*1000f;
#endif
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

			path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/splineMiddleControl.psd");
			sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite)) as TextureImporter;
#if (!UNITY_4_3 && !UNITY_4_4 && !UNITY_4_5 && !UNITY_4_6)
			textureImporter.spritePixelsPerUnit = (1 - ControlSize) * (1 - ControlSize) * 1000f;
#else
			textureImporter.spritePixelsToUnits = (1-ControlSize)*(1-ControlSize)*1000f;
#endif
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

			path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/ffdBone.psd");
			sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite)) as TextureImporter;
#if (!UNITY_4_3 && !UNITY_4_4 && !UNITY_4_5 && !UNITY_4_6)
			textureImporter.spritePixelsPerUnit = (1 - ControlSize) * (1 - ControlSize) * 1000f;
#else
			textureImporter.spritePixelsToUnits = (1-ControlSize)*(1-ControlSize)*1000f;
#endif
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

		}

		void ChangeVertexHandleSize()
		{
			string path = (Puppet2D_Editor._puppet2DPath + "/Textures/GUI/VertexHandle.psd");
			Sprite sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite)) as TextureImporter;
#if (!UNITY_4_3 && !UNITY_4_4 && !UNITY_4_5 && !UNITY_4_6)
			textureImporter.spritePixelsPerUnit = (1 - VertexHandleSize) * (1 - VertexHandleSize) * 1000f;
#else
		textureImporter.spritePixelsToUnits = (1-VertexHandleSize)*(1-VertexHandleSize)*1000f;
#endif
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		}


		static public void AddNewSortingName()
		{
			object newName = new object();

			var internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			string[] stuff = (string[])sortingLayersProperty.GetValue(null, new object[0]);

			sortingLayersProperty.SetValue(null, newName, new object[stuff.Length]);
		}

		public string[] GetSortingLayerNames()
		{
			var internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);

			return (string[])sortingLayersProperty.GetValue(null, new object[0]);
		}



		void MoveControl(Vector3 mousePos)
		{
			GameObject selectedGO = Selection.activeGameObject;
			if (selectedGO && selectedGO.transform && selectedGO.transform.parent)
				selectedGO.transform.parent.position = new Vector3(mousePos.x, mousePos.y, 0);


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

		void OnSelectionChange()
		{
			if (SkinWeightsPaint)
			{

				RefreshMeshColors();
			}
		}

		static void RefreshMeshColors()
		{
			SkinnedMeshRenderer smr = currentSelection.GetComponent<SkinnedMeshRenderer>();

			if (!BlackAndWhiteWeights)
			{

				Puppet2D_HiddenBone[] hiddenBones = FindObjectsOfType<Puppet2D_HiddenBone>();
				for (int j = 0; j < hiddenBones.Length; j++)
				{

					int boneIndex = smr.bones.ToList().IndexOf(hiddenBones[j].transform.parent);
					float hue = ((float)boneIndex / smr.bones.Length);
					Color col = Color.HSVToRGB(hue * 1f, 1f, 1f);
					if (hue >= 0)
					{
						hiddenBones[j].GetComponent<SpriteRenderer>().color = col;
						hiddenBones[j].transform.parent.GetComponent<SpriteRenderer>().color = col;
					}

				}
			}

			if (currentSelection == null)
				return;

			GameObject c = Selection.activeGameObject;
			if (c && c.GetComponent<SpriteRenderer>() && c.GetComponent<SpriteRenderer>().sprite && c.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
			{
				if (BlackAndWhiteWeights)
				{
					Puppet2D_HiddenBone[] hiddenBones = Transform.FindObjectsOfType<Puppet2D_HiddenBone>();
					foreach (Puppet2D_HiddenBone hiddenBone in hiddenBones)
					{
						if (hiddenBone.transform.parent != null && hiddenBone.transform.parent && hiddenBone.transform.parent == c.transform)
						{
							hiddenBone.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, .5f, 0);
							hiddenBone.transform.parent.GetComponent<SpriteRenderer>().color = new Color(1, .5f, 0);
						}
						else if (hiddenBone.transform.parent)
						{
							hiddenBone.transform.parent.GetComponent<SpriteRenderer>().color = Color.white;
							hiddenBone.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
						}
						else
						{
							hiddenBone.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
						}
					}
				}

				paintWeightsBone = c;

			}

			Vector3[] vertices = currentSelectionMesh.vertices;
			Color[] colrs = currentSelectionMesh.colors;
			BoneWeight[] boneWeights = Puppet2D_Editor.currentSelectionMesh.boneWeights;

			//Debug.Log("pos is " +pos);
			for (int i = 0; i < vertices.Length; i++)
			{

				colrs[i] = Color.black;

				//If both bones are this bone copy weights to first bone
				if (currentSelectionMesh.boneWeights[i].boneIndex0 == smr.bones.ToList().IndexOf(paintWeightsBone.transform) && currentSelectionMesh.boneWeights[i].boneIndex1 == smr.bones.ToList().IndexOf(paintWeightsBone.transform))
				{
					boneWeights[i].weight0 = currentSelectionMesh.boneWeights[i].weight0 + currentSelectionMesh.boneWeights[i].weight1;
					boneWeights[i].weight1 = 0;

					currentSelectionMesh.boneWeights = boneWeights;
				}
				//normalize weights
				if (boneWeights[i].weight0 + boneWeights[i].weight1 != 1)
				{
					if (boneWeights[i].weight0 + boneWeights[i].weight1 <= 0)
					{
						boneWeights[i].weight0 = 1;
						boneWeights[i].boneIndex0 = 1;

					}
					boneWeights[i].weight0 = boneWeights[i].weight0 / (boneWeights[i].weight0 + boneWeights[i].weight1);
					boneWeights[i].weight1 = 1 - boneWeights[i].weight0;

					currentSelectionMesh.boneWeights = boneWeights;
				}

				if (smr.bones.ToList().IndexOf(paintWeightsBone.transform) > -1 && currentSelectionMesh.boneWeights[i].boneIndex0 == smr.bones.ToList().IndexOf(paintWeightsBone.transform))
					colrs[i] = new Color(currentSelectionMesh.boneWeights[i].weight0, currentSelectionMesh.boneWeights[i].weight0, currentSelectionMesh.boneWeights[i].weight0);
				else if (smr.bones.ToList().IndexOf(paintWeightsBone.transform) > -1 && currentSelectionMesh.boneWeights[i].boneIndex1 == smr.bones.ToList().IndexOf(paintWeightsBone.transform))
					colrs[i] = new Color(currentSelectionMesh.boneWeights[i].weight1, currentSelectionMesh.boneWeights[i].weight1, currentSelectionMesh.boneWeights[i].weight1);
				//              else if(smr.bones[ Puppet2D_Editor.currentSelectionMesh.boneWeights[i].boneIndex2]== Puppet2D_Editor.paintWeightsBone.transform)
				//                  colrs[i] =new Color( Puppet2D_Editor.currentSelectionMesh.boneWeights[i].weight2, Puppet2D_Editor.currentSelectionMesh.boneWeights[i].weight2, Puppet2D_Editor.currentSelectionMesh.boneWeights[i].weight2);
				//              else if(smr.bones[ Puppet2D_Editor.currentSelectionMesh.boneWeights[i].boneIndex3]== Puppet2D_Editor.paintWeightsBone.transform)
				//                  colrs[i] =new Color( Puppet2D_Editor.currentSelectionMesh.boneWeights[i].weight3, Puppet2D_Editor.currentSelectionMesh.boneWeights[i].weight3, Puppet2D_Editor.currentSelectionMesh.boneWeights[i].weight3);



			}
			if (Puppet2D_Editor.BlackAndWhiteWeights)
				Puppet2D_Editor.currentSelectionMesh.colors = colrs;
			else
				Puppet2D_Editor.currentSelectionMesh.colors = Puppet2D_Skinning.SetColors(currentSelectionMesh.boneWeights);
		}

		void Start()
		{
			imageCount = 0;
		}
		void Update()
		{
			if (recordPngSequence && Application.isPlaying)
			{
				Time.captureFramerate = 30;

				recordDelta += Time.deltaTime;

				if (recordDelta >= 1 / 30)
				{
					imageCount++;

					SaveScreenshotToFile(pngSequPath + "." + imageCount.ToString("D4") + ".png");

					recordDelta = 0f;
				}
				Repaint();

			}
			if (!Application.isPlaying && imageCount > 0)
			{
				recordPngSequence = false;
				imageCount = 0;
				ExportPngAlpha = false;
			}


		}
		public static Texture2D TakeScreenShot()
		{
			return Screenshot();
		}

		static Texture2D Screenshot()
		{
			if (Camera.main == null)
			{
				Debug.LogWarning("Need a main camera in the scene");
				return null;
			}
			int resWidth = Camera.main.pixelWidth;
			int resHeight = Camera.main.pixelHeight;
			Camera camera = Camera.main;
			RenderTexture rt = new RenderTexture(resWidth, resHeight, 32);
			camera.targetTexture = rt;
			Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
			camera.Render();
			RenderTexture.active = rt;
			screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
			screenShot.Apply();
			camera.targetTexture = null;
			RenderTexture.active = null; // JC: added to avoid errors
			Destroy(rt);
			return screenShot;
		}

		public static Texture2D SaveScreenshotToFile(string fileName)
		{
			Texture2D screenShot = Screenshot();
			byte[] bytes = screenShot.EncodeToPNG();
			Debug.Log("Saving " + fileName);

			System.IO.File.WriteAllBytes(fileName, bytes);
			return screenShot;
		}

	}
}