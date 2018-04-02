using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
namespace Puppet2D
{
	public class Puppet2D_BoneCreation : Editor
	{


		public static void BoneFinishCreation()
		{

			Puppet2D_Editor.BoneCreation = false;
			EditorPrefs.SetBool("Puppet2D_BoneCreation", false);

			Puppet2D_HiddenBone[] hiddenBones = GameObject.FindObjectsOfType<Puppet2D_HiddenBone>();


			SpriteRenderer[] sprites = GameObject.FindObjectsOfType<SpriteRenderer>();
			foreach (SpriteRenderer spr in sprites)
			{
				if (spr.sprite != null && spr.sprite.name == "BoneNoJoint" && spr.transform.parent == null)
				{
					GameObject globalCtrlNew = Puppet2D_CreateControls.CreateGlobalControl();

					spr.transform.parent = globalCtrlNew.transform;
				}

			}


			GameObject globalCtrl = Puppet2D_CreateControls.CreateGlobalControl();

			if (globalCtrl != null)
			{
				foreach (Puppet2D_HiddenBone hiddenBone in hiddenBones)
				{
					if (hiddenBone && hiddenBone.transform.parent && hiddenBone.transform.parent.parent == null)
						hiddenBone.transform.parent.parent = globalCtrl.transform;
				}
			}


		}
		[MenuItem("GameObject/Puppet2D/Skeleton/Create Bone Tool")]
		public static void CreateBoneTool()
		{
			Puppet2D_Editor.BoneCreation = true;
			EditorPrefs.SetBool("Puppet2D_BoneCreation", true);

		}


		public static void sortOutBoneHierachy(GameObject changedBone, bool move = false)
		{


			SpriteRenderer spriteRenderer = changedBone.GetComponent<SpriteRenderer>();
			if (spriteRenderer)
				if (spriteRenderer.sprite)
					if (!spriteRenderer.sprite.name.Contains("Bone"))
						return;

			// UNPARENT CHILDREN
			List<Transform> children = new List<Transform>();

			foreach (Transform child in changedBone.transform)
			{
				if (child.GetComponent<Puppet2D_HiddenBone>() == null)
					children.Add(child);
			}
			foreach (Transform child in children)
			{
				if (!move)
					child.transform.parent = null;
			}
			Transform changedBonesParent = null;
			Transform changedBonesParentsParent = null;
			if (changedBone.transform.parent)
			{
				changedBonesParent = changedBone.transform.parent.transform;
				Undo.RecordObject(changedBonesParent, "bone parent");

				if (changedBone.transform.parent.transform.parent)
				{
					changedBonesParentsParent = changedBone.transform.parent.transform.parent.transform;

					changedBone.transform.parent.transform.parent = null;
				}

			}
			if (!move)
				changedBone.transform.parent = null;

			List<Transform> parentsChildren = new List<Transform>();

			// ORIENT & SCALE PARENT

			if (changedBonesParent)
			{
				foreach (Transform child in changedBonesParent.transform)
				{
					if (child.GetComponent<Puppet2D_HiddenBone>() == null)
					{
						parentsChildren.Add(child);
					}

				}
				foreach (Transform child in parentsChildren)
				{
					Undo.RecordObject(child, "parents child");
					child.transform.parent = null;
				}
				SpriteRenderer sprParent = changedBonesParent.GetComponent<SpriteRenderer>();
				if (sprParent)
					if (sprParent.sprite)
						if (sprParent.sprite.name.Contains("Bone"))
						{
							float dist = Vector3.Distance(changedBonesParent.position, changedBone.transform.position);
							if (dist > 0)
								changedBonesParent.rotation = Quaternion.LookRotation(changedBone.transform.position - changedBonesParent.position, Vector3.back) * Quaternion.AngleAxis(90, Vector3.right);
							float length = (changedBonesParent.position - changedBone.transform.position).magnitude;

							changedBonesParent.localScale = new Vector3(length, length, length);
						}


			}
			if (!move)
				changedBone.transform.localScale = Vector3.one;

			// REPARENT CHILDREN

			if (children.Count > 0)
			{
				foreach (Transform child in children)
				{
					SpriteRenderer spr = child.GetComponent<SpriteRenderer>();
					if (spr)
						if (spr.sprite)
							if (spr.sprite.name.Contains("Bone"))
							{
								Undo.RecordObject(child, "parents child");
								child.transform.parent = changedBone.transform;
							}
				}
			}
			else
			{
				Undo.RecordObject(spriteRenderer, "sprite change");
				spriteRenderer.sprite = Puppet2D_Editor.boneNoJointSprite;
			}

			if (changedBonesParent)
			{
				changedBone.transform.parent = changedBonesParent;
				if (changedBonesParentsParent)
					changedBone.transform.parent.transform.parent = changedBonesParentsParent;

				foreach (Transform child in parentsChildren)
				{
					Undo.RecordObject(child, "parents child");
					child.transform.parent = changedBonesParent;
				}
				SpriteRenderer spr = changedBonesParent.GetComponent<SpriteRenderer>();
				if (spr)
				{
					if (spr.sprite)
					{
						if (spr.sprite.name.Contains("Bone"))
						{
							Undo.RecordObject(spr, "sprite change");
							spr.sprite = Puppet2D_Editor.boneSprite;
						}
					}
				}



			}

			// SET CORRECT SPRITE
			if (!move)
			{
				if (children.Count > 0)
					changedBone.GetComponent<SpriteRenderer>().sprite = Puppet2D_Editor.boneSprite;
				else
					changedBone.GetComponent<SpriteRenderer>().sprite = Puppet2D_Editor.boneNoJointSprite;

			}

			children.Clear();
			parentsChildren.Clear();

		}
		public static GameObject BoneCreationMode(Vector3 mousePos)
		{

			bool isSplineBone = false;
			if (Selection.activeGameObject == null)
				if (Puppet2D_Editor.currentActiveBone)
					Selection.activeGameObject = Puppet2D_Editor.currentActiveBone;
			if (Selection.activeGameObject)
			{
				if (Selection.activeGameObject.GetComponent<Puppet2D_HiddenBone>())
					Selection.activeGameObject = Selection.activeGameObject.transform.parent.gameObject;

				if (Selection.activeGameObject.GetComponent<SpriteRenderer>())
				{
					if (Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite)
					{
						if (Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
						{
							// MAKE SURE SELECTION IS NOT AN IK OR PARENT

							Puppet2D_GlobalControl[] globalCtrlScripts = Transform.FindObjectsOfType<Puppet2D_GlobalControl>();
							for (int i = 0; i < globalCtrlScripts.Length; i++)
							{
								foreach (Puppet2D_IKHandle Ik in globalCtrlScripts[i]._Ikhandles)
								{
									if ((Ik.topJointTransform == Selection.activeGameObject.transform) || (Ik.bottomJointTransform == Selection.activeGameObject.transform) || (Ik.middleJointTransform == Selection.activeGameObject.transform))
									{
										Debug.LogWarning("Cannot parent bone, as this one has an IK handle");
										Selection.activeGameObject = null;
										return null;
									}
								}
								foreach (Puppet2D_SplineControl splineCtrl in globalCtrlScripts[i]._SplineControls)
								{
									foreach (GameObject bone in splineCtrl.bones)
									{
										if (bone == Selection.activeGameObject)
										{
											isSplineBone = true;

										}
									}
								}
							}
						}
						else
							return null;
					}
					else
						return null;
				}
				else
					return null;
			}




			GameObject newBone = new GameObject(GetUniqueBoneName("bone"));
			Undo.RegisterCreatedObjectUndo(newBone, "Created newBone");
			newBone.transform.position = mousePos;
			newBone.transform.position = new Vector3(newBone.transform.position.x, newBone.transform.position.y, 0);

			if (Selection.activeGameObject)
			{
				newBone.transform.parent = Selection.activeGameObject.transform;

				GameObject newInvisibleBone = new GameObject(GetUniqueBoneName("hiddenBone"));
				Undo.RegisterCreatedObjectUndo(newInvisibleBone, "Created new invisible Bone");

				SpriteRenderer spriteRendererInvisbile = newInvisibleBone.AddComponent<SpriteRenderer>();
				newInvisibleBone.transform.position = new Vector3(10000, 10000, 10000);
				spriteRendererInvisbile.sortingLayerName = Puppet2D_Editor._boneSortingLayer;
				spriteRendererInvisbile.sprite = Puppet2D_Editor.boneHiddenSprite;
				newInvisibleBone.transform.parent = Selection.activeGameObject.transform;
				Undo.AddComponent(newInvisibleBone, typeof(Puppet2D_HiddenBone));
				Puppet2D_HiddenBone hiddenBoneComp = newInvisibleBone.GetComponent<Puppet2D_HiddenBone>();
				hiddenBoneComp.boneToAimAt = newBone.transform;
				hiddenBoneComp.Refresh();

			}

			SpriteRenderer spriteRenderer = newBone.AddComponent<SpriteRenderer>();
			spriteRenderer.sortingLayerName = Puppet2D_Editor._boneSortingLayer;

			if (isSplineBone)
				newBone.transform.parent = null;

			sortOutBoneHierachy(newBone);

			if (isSplineBone)
				newBone.transform.parent = Selection.activeGameObject.transform;

			Selection.activeGameObject = newBone;

			Puppet2D_Editor.currentActiveBone = newBone;

			return newBone;

		}
		public static void BoneMoveMode(Vector3 mousePos)
		{
			GameObject selectedGO = Selection.activeGameObject;

			bool isParentSplineBone = false;

			if (selectedGO)
			{
				if (selectedGO.GetComponent<Puppet2D_HiddenBone>())
				{
					selectedGO = Selection.activeGameObject.transform.parent.gameObject;
					Selection.activeGameObject = selectedGO;
				}

				if (selectedGO.GetComponent<SpriteRenderer>())
				{
					if (selectedGO.GetComponent<SpriteRenderer>().sprite)
					{
						if (selectedGO.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
						{
							// MAKE SURE SELECTION IS NOT AN IK OR PARENT

							Puppet2D_GlobalControl[] globalCtrlScripts = Transform.FindObjectsOfType<Puppet2D_GlobalControl>();
							for (int i = 0; i < globalCtrlScripts.Length; i++)
							{
								foreach (Puppet2D_IKHandle Ik in globalCtrlScripts[i]._Ikhandles)
								{
									if ((Ik.topJointTransform == selectedGO.transform) || (Ik.bottomJointTransform == selectedGO.transform) || (Ik.middleJointTransform == selectedGO.transform))
									{
										Debug.LogWarning("Cannot move bone, as this one has an IK handle");
										return;
									}
								}
								foreach (Puppet2D_SplineControl splineCtrl in globalCtrlScripts[i]._SplineControls)
								{

									foreach (GameObject bone in splineCtrl.bones)
									{
										if (bone.transform == selectedGO.transform)
										{
											Debug.LogWarning("Cannot move Spline Bones");
											return;
										}
										if (selectedGO.transform.parent)
										{
											if (bone.transform == selectedGO.transform.parent)
											{
												isParentSplineBone = true;
											}
										}

									}
								}
							}
						}
						else
							return;
					}
					else
						return;
				}
				else
					return;


				selectedGO.transform.position = mousePos;
				selectedGO.transform.position = new Vector3(Selection.activeGameObject.transform.position.x, Selection.activeGameObject.transform.position.y, 0);

				if (!isParentSplineBone)
					sortOutBoneHierachy(selectedGO, true);

			}

		}
		public static void BoneMoveIndividualMode(Vector3 mousePos)
		{
			GameObject selectedGO = Selection.activeGameObject;

			bool isParentSplineBone = false;

			if (selectedGO)
			{
				if (selectedGO.GetComponent<Puppet2D_HiddenBone>())
				{
					selectedGO = Selection.activeGameObject.transform.parent.gameObject;
					Selection.activeGameObject = selectedGO;
				}
				if (selectedGO.GetComponent<SpriteRenderer>())
				{
					if (selectedGO.GetComponent<SpriteRenderer>().sprite)
					{
						if (selectedGO.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
						{
							// MAKE SURE SELECTION IS NOT AN IK OR PARENT

							Puppet2D_GlobalControl[] globalCtrlScripts = Transform.FindObjectsOfType<Puppet2D_GlobalControl>();
							for (int i = 0; i < globalCtrlScripts.Length; i++)
							{
								foreach (Puppet2D_IKHandle Ik in globalCtrlScripts[i]._Ikhandles)
								{
									if ((Ik.topJointTransform == selectedGO.transform) || (Ik.bottomJointTransform == selectedGO.transform) || (Ik.middleJointTransform == selectedGO.transform))
									{
										Debug.LogWarning("Cannot move bone, as this one has an IK handle");
										return;
									}

								}

								foreach (Puppet2D_SplineControl splineCtrl in globalCtrlScripts[i]._SplineControls)
								{

									foreach (GameObject bone in splineCtrl.bones)
									{
										if (bone.transform == selectedGO.transform)
										{
											Debug.LogWarning("Cannot move Spline Bones");
											return;
										}
										if (selectedGO.transform.parent)
										{
											if (bone.transform == selectedGO.transform.parent)
											{
												isParentSplineBone = true;
											}
										}

									}
								}

							}
						}
						else
							return;
					}
					else
						return;
				}
				else
					return;

				List<Transform> children = new List<Transform>();
				foreach (Transform child in selectedGO.transform)
				{
					if (child.GetComponent<Puppet2D_HiddenBone>() == null)
						children.Add(child);
				}
				foreach (Transform child in children)
					child.parent = null;

				selectedGO.transform.position = mousePos;
				selectedGO.transform.position = new Vector3(Selection.activeGameObject.transform.position.x, Selection.activeGameObject.transform.position.y, 0);

				if (!isParentSplineBone)
					sortOutBoneHierachy(selectedGO, true);

				foreach (Transform child in children)
				{
					child.parent = selectedGO.transform;
					sortOutBoneHierachy(child.gameObject, true);
				}

				children.Clear();

			}

		}
		public static void BoneDeleteMode()
		{
			GameObject selectedGO = Selection.activeGameObject;
			if (selectedGO)
			{
				if (selectedGO.GetComponent<Puppet2D_HiddenBone>())
				{
					GameObject hiddenBone = selectedGO;
					selectedGO = selectedGO.transform.parent.gameObject;
					DestroyImmediate(hiddenBone);

					Selection.activeGameObject = selectedGO;

				}
				if (selectedGO.GetComponent<SpriteRenderer>())
				{
					if (selectedGO.GetComponent<SpriteRenderer>().sprite)
					{
						if (selectedGO.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
						{
							// MAKE SURE SELECTION IS NOT AN IK OR PARENT

							Puppet2D_GlobalControl[] globalCtrlScripts = Transform.FindObjectsOfType<Puppet2D_GlobalControl>();
							for (int i = 0; i < globalCtrlScripts.Length; i++)
							{
								foreach (Puppet2D_IKHandle Ik in globalCtrlScripts[i]._Ikhandles)
								{
									if ((Ik.topJointTransform == selectedGO.transform) || (Ik.bottomJointTransform == selectedGO.transform) || (Ik.middleJointTransform == selectedGO.transform))
									{
										Debug.LogWarning("Cannot move bone, as this one has an IK handle");
										return;
									}
								}
								foreach (Puppet2D_SplineControl splineCtrl in globalCtrlScripts[i]._SplineControls)
								{

									foreach (GameObject bone in splineCtrl.bones)
									{
										if (bone.transform == selectedGO.transform)
										{
											Debug.LogWarning("Cannot delete Spline Bones Individually");
											return;
										}

									}
								}
							}
						}
						else
							return;
					}
					else
						return;
				}
				else
					return;

				if (selectedGO.transform.parent)
				{
					GameObject parentGO = selectedGO.transform.parent.gameObject;
					DestroyImmediate(selectedGO);
					sortOutBoneHierachy(parentGO);
					Selection.activeGameObject = parentGO;
					foreach (Transform child in parentGO.transform)
					{
						if (child.GetComponent<Puppet2D_HiddenBone>() == null)
							sortOutBoneHierachy(child.gameObject, true);
					}
				}
				else
				{
					DestroyImmediate(selectedGO);
				}

			}

		}
		public static void BoneAddMode(Vector3 mousePos)
		{
			GameObject selectedGO = Selection.activeGameObject;

			if (selectedGO)
			{
				if (selectedGO.GetComponent<Puppet2D_HiddenBone>())
				{
					selectedGO = Selection.activeGameObject.transform.parent.gameObject;
					Selection.activeGameObject = selectedGO;
				}
				if (selectedGO.GetComponent<SpriteRenderer>())
				{
					if (selectedGO.GetComponent<SpriteRenderer>().sprite)
					{
						if (selectedGO.GetComponent<SpriteRenderer>().sprite.name.Contains("Bone"))
						{
							// MAKE SURE SELECTION IS NOT AN IK OR PARENT

							Puppet2D_GlobalControl[] globalCtrlScripts = Transform.FindObjectsOfType<Puppet2D_GlobalControl>();
							for (int i = 0; i < globalCtrlScripts.Length; i++)
							{
								foreach (Puppet2D_IKHandle Ik in globalCtrlScripts[i]._Ikhandles)
								{
									if ((Ik.topJointTransform == selectedGO.transform) || (Ik.bottomJointTransform == selectedGO.transform) || (Ik.middleJointTransform == selectedGO.transform))
									{
										Debug.LogWarning("Cannot add bone, as this one has an IK handle");
										return;
									}
								}
								foreach (Puppet2D_SplineControl splineCtrl in globalCtrlScripts[i]._SplineControls)
								{

									foreach (GameObject bone in splineCtrl.bones)
									{
										if (bone.transform == selectedGO.transform)
										{
											Debug.LogWarning("Cannot add to Spline Bones");
											return;
										}

									}
								}
							}
						}
						else
							return;
					}
					else
						return;
				}
				else
					return;


				List<Transform> children = new List<Transform>();
				foreach (Transform child in selectedGO.transform)
					children.Add(child);
				foreach (Transform child in children)
					child.parent = null;

				GameObject newBone = BoneCreationMode(mousePos);

				foreach (Transform child in children)
				{

					child.parent = newBone.transform;
					if (child.GetComponent<Puppet2D_HiddenBone>() == null)
					{
						sortOutBoneHierachy(child.gameObject, true);
					}
					else
						child.GetComponent<Puppet2D_HiddenBone>().Refresh();

				}
				Selection.activeGameObject = newBone;
				children.Clear();

			}

		}

		public static string GetUniqueBoneName(string name)
		{
			string nameToAdd = name;
			int nameToAddLength = nameToAdd.Length + 1;
			int index = 0;
			foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
			{
				if (go.name.StartsWith(nameToAdd))
				{
					string endOfName = go.name.Substring(nameToAddLength, go.name.Length - nameToAddLength);

					int indexTest = 0;
					if (int.TryParse(endOfName, out indexTest))
					{
						if (int.Parse(endOfName) > index)
						{
							index = int.Parse(endOfName);
						}
					}


				}
			}
			index++;
			return (name + "_" + index);

		}
	}

}