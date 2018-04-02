using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Puppet2D
{
	[ExecuteInEditMode]
	public class Puppet2D_GlobalControl : MonoBehaviour
	{

		public float startRotationY;

		public List<Puppet2D_SplineControl> _SplineControls = new List<Puppet2D_SplineControl>();
		public List<Puppet2D_IKHandle> _Ikhandles = new List<Puppet2D_IKHandle>();
		public List<Puppet2D_ParentControl> _ParentControls = new List<Puppet2D_ParentControl>();
		public List<Puppet2D_FFDLineDisplay> _ffdControls = new List<Puppet2D_FFDLineDisplay>();

		[HideInInspector]
		public List<SpriteRenderer> _Controls = new List<SpriteRenderer>();
		[HideInInspector]
		public List<SpriteRenderer> _Bones = new List<SpriteRenderer>();
		[HideInInspector]
		public List<SpriteRenderer> _FFDControls = new List<SpriteRenderer>();

		public bool ControlsVisiblity = true;
		public bool BonesVisiblity = true;
		public bool FFD_Visiblity = true;

		public bool CombineMeshes = false;

		public bool flip = false;
		private bool internalFlip = false;

		public bool AutoRefresh = true;
		public bool ControlsEnabled = true;
		public bool lateUpdate = true;

		[HideInInspector]
		public int _flipCorrection = 1;

		// CACHED VALUES

		private Transform myTransform;
		private Puppet2D_SplineControl[] _SplineControlsArray;
		private Puppet2D_IKHandle[] _IkhandlesArray;
		private Puppet2D_ParentControl[] _ParentControlsArray;
		private Puppet2D_FFDLineDisplay[] _ffdControlsArray;

		//public float boneSize;
		// Use this for initialization
		void OnEnable()
		{

			if (AutoRefresh)
			{
				_Ikhandles.Clear();
				_SplineControls.Clear();
				_ParentControls.Clear();
				_Controls.Clear();
				_Bones.Clear();
				_FFDControls.Clear();
				_ffdControls.Clear();
				TraverseHierarchy(transform);
				InitializeArrays();
			}

		}
		public void Refresh()
		{
			_Ikhandles.Clear();
			_SplineControls.Clear();
			_ParentControls.Clear();
			_Controls.Clear();
			_Bones.Clear();
			_FFDControls.Clear();
			_ffdControls.Clear();
			TraverseHierarchy(transform);
			InitializeArrays();
		}
		void Awake()
		{
			this.myTransform = this.GetComponent<Transform>();

			internalFlip = flip;
			if (Application.isPlaying)
			{
				if (CombineMeshes)
					CombineAllMeshes();

			}


		}
		void Start()
		{

		}
		public void InitializeArrays()
		{
			_SplineControlsArray = _SplineControls.ToArray();
			_IkhandlesArray = _Ikhandles.ToArray();
			_ParentControlsArray = _ParentControls.ToArray();
			_ffdControlsArray = _ffdControls.ToArray();
		}
		// Update is called once per frame
		public void Init()
		{

			_Ikhandles.Clear();
			_SplineControls.Clear();
			_ParentControls.Clear();
			_Controls.Clear();
			_Bones.Clear();
			_FFDControls.Clear();
			_ffdControls.Clear();
			TraverseHierarchy(transform);
			InitializeArrays();

		}
		void OnValidate()
		{
			UpdateVisibility();
		}

		public void UpdateVisibility()
		{
			if (AutoRefresh)
			{
				_Ikhandles.Clear();
				_SplineControls.Clear();
				_ParentControls.Clear();
				_Controls.Clear();
				_Bones.Clear();
				_FFDControls.Clear();
				_ffdControls.Clear();
				TraverseHierarchy(transform);
				InitializeArrays();
			}
			foreach (SpriteRenderer ctrl in _Controls)
			{
				if (ctrl && ctrl.enabled != ControlsVisiblity)
					ctrl.enabled = ControlsVisiblity;
			}
			foreach (SpriteRenderer bone in _Bones)
			{
				if (bone && bone.enabled != BonesVisiblity)
					bone.enabled = BonesVisiblity;
			}
			foreach (SpriteRenderer ffdCtrl in _FFDControls)
			{
				if (ffdCtrl && ffdCtrl.transform.parent && ffdCtrl.transform.parent.gameObject && ffdCtrl.transform.parent.gameObject.activeSelf != FFD_Visiblity)
					ffdCtrl.transform.parent.gameObject.SetActive(FFD_Visiblity);
			}
		}

		void Update()
		{
			if (!lateUpdate)
			{

#if UNITY_EDITOR
				if (AutoRefresh)
				{
					for (int i = _ParentControls.Count - 1; i >= 0; i--)
					{
						if (_ParentControls[i] == null)
							_ParentControls.RemoveAt(i);
					}
					for (int i = _Ikhandles.Count - 1; i >= 0; i--)
					{
						if (_Ikhandles[i] == null)
							_Ikhandles.RemoveAt(i);
					}
					for (int i = _SplineControls.Count - 1; i >= 0; i--)
					{
						if (_SplineControls[i] == null)
							_SplineControls.RemoveAt(i);
					}
					for (int i = _ffdControls.Count - 1; i >= 0; i--)
					{
						if (_ffdControls[i] == null)
							_ffdControls.RemoveAt(i);
					}
				}
#endif

				if (ControlsEnabled)
					Run();

				if (internalFlip != flip)
				{
					if (flip)
					{

						transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -transform.localScale.z);
						transform.localEulerAngles = new Vector3(transform.rotation.eulerAngles.x, startRotationY + 180, transform.rotation.eulerAngles.z);

					}
					else
					{

						transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
						transform.localEulerAngles = new Vector3(transform.rotation.eulerAngles.x, startRotationY, transform.rotation.eulerAngles.z);
					}
					internalFlip = flip;
					Run();
				}
			}





		}
		void LateUpdate()
		{
			if (lateUpdate)
			{

#if UNITY_EDITOR
				if (AutoRefresh)
				{
					for (int i = _ParentControls.Count - 1; i >= 0; i--)
					{
						if (_ParentControls[i] == null)
							_ParentControls.RemoveAt(i);
					}
					for (int i = _Ikhandles.Count - 1; i >= 0; i--)
					{
						if (_Ikhandles[i] == null)
							_Ikhandles.RemoveAt(i);
					}
					for (int i = _SplineControls.Count - 1; i >= 0; i--)
					{
						if (_SplineControls[i] == null)
							_SplineControls.RemoveAt(i);
					}
					for (int i = _ffdControls.Count - 1; i >= 0; i--)
					{
						if (_ffdControls[i] == null)
							_ffdControls.RemoveAt(i);
					}
				}
#endif

				if (ControlsEnabled)
					Run();

				if (internalFlip != flip)
				{
					if (flip)
					{

						transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -transform.localScale.z);
						transform.localEulerAngles = new Vector3(transform.rotation.eulerAngles.x, startRotationY + 180, transform.rotation.eulerAngles.z);

					}
					else
					{

						transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
						transform.localEulerAngles = new Vector3(transform.rotation.eulerAngles.x, startRotationY, transform.rotation.eulerAngles.z);
					}
					internalFlip = flip;
					Run();
				}
			}





		}
		public void Run()
		{

			for (int i = 0; i < _SplineControlsArray.Length; i++)
			{
				if (_SplineControlsArray[i])
					_SplineControlsArray[i].Run();
			}
			for (int i = 0; i < _ParentControlsArray.Length; i++)
			{
				if (_ParentControlsArray[i])
					_ParentControlsArray[i].ParentControlRun();
			}
			FaceCamera();
			for (int i = 0; i < _IkhandlesArray.Length; i++)
			{
				if (_IkhandlesArray[i])
					_IkhandlesArray[i].CalculateIK();
			}
			for (int i = 0; i < _ffdControlsArray.Length; i++)
			{
				if (_ffdControlsArray[i])
					_ffdControlsArray[i].Run();
			}



		}

		public void TraverseHierarchy(Transform root)
		{
			foreach (Transform child in root)
			{
				GameObject Go = child.gameObject;
				SpriteRenderer spriteRenderer = Go.transform.GetComponent<SpriteRenderer>();
				if (spriteRenderer && spriteRenderer.sprite)
				{
					if (spriteRenderer.sprite.name.Contains("Control"))
						_Controls.Add(spriteRenderer);
					else if (spriteRenderer.sprite.name.Contains("ffd"))
						_FFDControls.Add(spriteRenderer);
					else if (spriteRenderer.sprite.name.Contains("Bone"))
						_Bones.Add(spriteRenderer);
				}
				Puppet2D_ParentControl newParentCtrl = Go.transform.GetComponent<Puppet2D_ParentControl>();

				if (newParentCtrl)
				{
					_ParentControls.Add(newParentCtrl);

				}
				Puppet2D_IKHandle newIKCtrl = Go.transform.GetComponent<Puppet2D_IKHandle>();
				if (newIKCtrl)
					_Ikhandles.Add(newIKCtrl);

				Puppet2D_FFDLineDisplay ffdCtrl = Go.transform.GetComponent<Puppet2D_FFDLineDisplay>();
				if (ffdCtrl)
					_ffdControls.Add(ffdCtrl);

				Puppet2D_SplineControl splineCtrl = Go.transform.GetComponent<Puppet2D_SplineControl>();
				if (splineCtrl)
					_SplineControls.Add(splineCtrl);

				TraverseHierarchy(child);

			}

		}
		void CombineAllMeshes()
		{
			Vector3 originalScale = transform.localScale;
			Quaternion originalRot = transform.rotation;
			Vector3 originalPos = transform.position;
			transform.localScale = Vector3.one;
			transform.rotation = Quaternion.identity;
			transform.position = Vector3.zero;

			SkinnedMeshRenderer[] smRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
			List<Transform> bones = new List<Transform>();
			List<BoneWeight> boneWeights = new List<BoneWeight>();
			List<CombineInstance> combineInstances = new List<CombineInstance>();
			List<Texture2D> textures = new List<Texture2D>();

			Material currentMaterial = null;

			int numSubs = 0;
			var smRenderersDict = new Dictionary<SkinnedMeshRenderer, float>(smRenderers.Length);

			bool updateWhenOffscreen = false;

			foreach (SkinnedMeshRenderer smr in smRenderers)
			{
				smRenderersDict.Add(smr, smr.transform.position.z);
				updateWhenOffscreen = smr.updateWhenOffscreen;
			}


			var items = from pair in smRenderersDict
						orderby pair.Key.sortingOrder ascending
						select pair;

			items = from pair in items
					orderby pair.Value descending
					select pair;
			foreach (KeyValuePair<SkinnedMeshRenderer, float> pair in items)
			{
				//Debug.Log(pair.Key.name + " " + pair.Value);
				numSubs += pair.Key.sharedMesh.subMeshCount;
			}


			int[] meshIndex = new int[numSubs];
			int boneOffset = 0;

			int s = 0;
			foreach (KeyValuePair<SkinnedMeshRenderer, float> pair in items)
			{
				SkinnedMeshRenderer smr = pair.Key;

				if (currentMaterial == null)
					currentMaterial = smr.sharedMaterial;
				else if (currentMaterial.mainTexture && smr.sharedMaterial.mainTexture && currentMaterial.mainTexture != smr.sharedMaterial.mainTexture)
					continue;

				bool ffdMesh = false;
				foreach (Transform boneToCheck in smr.bones)
				{
					Puppet2D_FFDLineDisplay ffdLine = boneToCheck.GetComponent<Puppet2D_FFDLineDisplay>();
					if (ffdLine && ffdLine.outputSkinnedMesh != smr)
						ffdMesh = true;
				}
				if (ffdMesh)
					continue;
				BoneWeight[] meshBoneweight = smr.sharedMesh.boneWeights;

				foreach (BoneWeight bw in meshBoneweight)
				{
					BoneWeight bWeight = bw;

					bWeight.boneIndex0 += boneOffset;
					bWeight.boneIndex1 += boneOffset;
					bWeight.boneIndex2 += boneOffset;
					bWeight.boneIndex3 += boneOffset;

					boneWeights.Add(bWeight);
				}
				boneOffset += smr.bones.Length;

				Transform[] meshBones = smr.bones;
				foreach (Transform bone in meshBones)
					bones.Add(bone);

				if (smr.material.mainTexture != null)
					textures.Add(smr.GetComponent<Renderer>().material.mainTexture as Texture2D);



				CombineInstance ci = new CombineInstance();
				ci.mesh = smr.sharedMesh;
				meshIndex[s] = ci.mesh.vertexCount;
				ci.transform = smr.transform.localToWorldMatrix;
				combineInstances.Add(ci);

				Object.Destroy(smr.gameObject);
				s++;
			}


			List<Matrix4x4> bindposes = new List<Matrix4x4>();

			for (int b = 0; b < bones.Count; b++)
			{
				if (bones[b].GetComponent<Puppet2D_FFDLineDisplay>())
				{
					Vector3 boneparentPos = bones[b].transform.parent.parent.position;
					Quaternion boneparentRot = bones[b].transform.parent.parent.rotation;
					bones[b].transform.parent.parent.position = Vector3.zero;
					bones[b].transform.parent.parent.rotation = Quaternion.identity;
					bindposes.Add(bones[b].worldToLocalMatrix * transform.worldToLocalMatrix);
					bones[b].transform.parent.parent.position = boneparentPos;
					bones[b].transform.parent.parent.rotation = boneparentRot;
				}
				else
					bindposes.Add(bones[b].worldToLocalMatrix * transform.worldToLocalMatrix);
			}

			SkinnedMeshRenderer r = gameObject.AddComponent<SkinnedMeshRenderer>();

			r.updateWhenOffscreen = updateWhenOffscreen;
			r.sharedMesh = new Mesh();
			r.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

			Material combinedMat;
			if (currentMaterial != null)
				combinedMat = currentMaterial;
			else
				combinedMat = new Material(Shader.Find("Unlit/Transparent"));

			combinedMat.mainTexture = textures[0];
			r.sharedMesh.uv = r.sharedMesh.uv;
			r.sharedMaterial = combinedMat;

			r.bones = bones.ToArray();
			r.sharedMesh.boneWeights = boneWeights.ToArray();
			r.sharedMesh.bindposes = bindposes.ToArray();
			r.sharedMesh.RecalculateBounds();


			transform.localScale = originalScale;
			transform.rotation = originalRot;
			transform.position = originalPos;
		}
		private void FaceCamera()
		{
			for (int i = 0; i < this._IkhandlesArray.Length; ++i)
			{
				this._IkhandlesArray[i].AimDirection = this.myTransform.forward.normalized * _flipCorrection;
			}
		}

	}

}