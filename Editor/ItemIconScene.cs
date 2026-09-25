using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Landfall.TABS;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace UFoxLib.Editor
{
    public class ItemIconScene : MonoBehaviour
    {
        void Awake()
        {
            if (ItemIconScene.instance != null) return;
            ItemIconScene.instance = this;
            m_maxUnitBases = m_allUnitBases.Count;
            UpdateUnitBase(0);
        }

        public void Button_TakeSingleIcon()
        {
            string _iconName = m_iconModPrefix + m_iconItemName;
            if (_iconName == null)
            {
                _iconName = m_iconModPrefix + UnityEngine.Random.Range(100000000, 999999999).ToString();
            }
            SaveRenderTexture(_iconName, m_currentSaveDirectory);
        }

        public void Button_SelectUnitBase(int _ID) => UpdateUnitBase(_ID);

        public void Button_SelectGooglyEyes(int _ID) => UpdateGoogleEyes(_ID);
        public void Button_StartAutoGen_Props() => StartCoroutine(AutoGeneratePropIcons());

        public void Button_StartAutoGen_Weapons() => StartCoroutine(AutoGenerateWeaponIcons());

        public void Button_StartAutoGen_Projectiles() => StartCoroutine(AutoGenerateProjectileIcons());

        void SaveRenderTexture(string _name, SaveDirectory _directoryType)
        {
            RenderTexture renderTexture = new RenderTexture(m_currentIconSize, m_currentIconSize, 24);
            renderTexture.format = RenderTextureFormat.ARGB32;
            renderTexture.Create();

            m_cam.targetTexture = renderTexture;
            m_cam.Render();
            SaveIcon(renderTexture, _name + ".png", _directoryType);
            m_cam.targetTexture = null;
            renderTexture.Release();
            if (renderTexture != null) global::UnityEngine.Object.DestroyImmediate(renderTexture);
        }

        void SaveIcon(RenderTexture rt, string name, SaveDirectory _directoryType)
        {
            EncodeRT(rt, name, _directoryType);
        }

        void EncodeRT(RenderTexture rt, string _fileName, SaveDirectory _directoryType)
        {
            Texture2D texture2D = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, true);
            RenderTexture.active = rt;
            texture2D.ReadPixels(new Rect(0f, 0f, (float)rt.width, (float)rt.height), 0, 0);
            texture2D.Apply();

            var text = "";
            switch (_directoryType)
            {
                case SaveDirectory.PropItems:
                    text = m_iconSaveData.m_directoryProps;
                    break;
                case SaveDirectory.Weapons:
                    text = m_iconSaveData.m_directoryWeapons;
                    break;
                case SaveDirectory.Projectiles:
                    text = m_iconSaveData.m_directoryProjectiles;
                    break;
                default:    
                    break;
            }

            byte[] array = texture2D.EncodeToPNG();
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }

            File.WriteAllBytes(text + "/" + _fileName, array);
            if (texture2D != null) global::UnityEngine.Object.DestroyImmediate(texture2D);
        }

        void UpdateUnitBase(int _baseID)
        {
            if (_baseID > m_maxUnitBases)
            {
                Debug.LogError("ICON EDITOR: CAN'T FIND UNIT BASE WITH ID " + _baseID.ToString());
                return;
            }

            //Step 1: hide previous base if it was active
            if (m_currentUnitBaseTransform != null && m_currentUnitBaseTransform.gameObject.activeInHierarchy) 
                m_currentUnitBaseTransform.gameObject.SetActive(false);

            m_currentUnitBase = m_allUnitBases[_baseID];

            m_currentUnitBaseTransform = m_currentUnitBase.m_unitBaseRoot;
            if (!m_currentUnitBaseTransform.gameObject.activeInHierarchy) m_currentUnitBaseTransform.gameObject.SetActive(true);
            m_currentRigTransforms = m_currentUnitBase.m_baseRigs;

            foreach (Head _eyeTransform in m_currentGooglyEyesParent)
            {
                if (_eyeTransform.gameObject.activeInHierarchy) _eyeTransform.gameObject.SetActive(false);
            }

            m_currentGooglyEyesParent = m_currentUnitBase.m_googlyEyesParent;
            UpdateGoogleEyes(0);
        }

        void UpdateGoogleEyes(int _eyeType)
        {
            foreach (Head _eyeTransform in m_currentGooglyEyesParent)
            {
                if (_eyeTransform.gameObject.activeInHierarchy) _eyeTransform.gameObject.SetActive(false);
            }

            var _desiredEyes = m_currentGooglyEyesParent.GetChild(_eyeType);
            if (_desiredEyes != null && _desiredEyes.GetComponent<Head>())
                _desiredEyes.gameObject.SetActive(true);
        }

        public void SwitchCameraAngle(ItemIconScene.PropAngle _newAngle)
        {
            switch (_newAngle)
            {
                case PropAngle.Head:
                    m_currentIconSize = m_iconInfo_props.x;
                    m_cam.fieldOfView = m_iconInfo_props.y;
                    m_cam.transform.position = m_currentUnitBase.m_angles.m_angle_Head.position;
                    m_cam.transform.rotation = m_currentUnitBase.m_angles.m_angle_Head.rotation;
                    break;
                case PropAngle.Torso:
                    m_currentIconSize = m_iconInfo_props.x;
                    m_cam.fieldOfView = m_iconInfo_props.y;
                    m_cam.transform.position = m_currentUnitBase.m_angles.m_angle_Torso.position;
                    m_cam.transform.rotation = m_currentUnitBase.m_angles.m_angle_Torso.rotation;
                    break;
                case PropAngle.Shoulders:
                    m_currentIconSize = m_iconInfo_props.x;
                    m_cam.fieldOfView = m_iconInfo_props.y;
                    m_cam.transform.position = m_currentUnitBase.m_angles.m_angle_Shoulders.position;
                    m_cam.transform.rotation = m_currentUnitBase.m_angles.m_angle_Shoulders.rotation;
                    break;
                case PropAngle.Wrists:
                    m_currentIconSize = m_iconInfo_props.x;
                    m_cam.fieldOfView = m_iconInfo_props.y;
                    m_cam.transform.position = m_currentUnitBase.m_angles.m_angle_Wrists.position;
                    m_cam.transform.rotation = m_currentUnitBase.m_angles.m_angle_Wrists.rotation;
                    break;
                case PropAngle.Hip:
                    m_currentIconSize = m_iconInfo_props.x;
                    m_cam.fieldOfView = m_iconInfo_props.y;
                    m_cam.transform.position = m_currentUnitBase.m_angles.m_angle_Hip.position;
                    m_cam.transform.rotation = m_currentUnitBase.m_angles.m_angle_Hip.rotation;
                    break;
                case PropAngle.Legs:
                    m_currentIconSize = m_iconInfo_props.x;
                    m_cam.fieldOfView = m_iconInfo_props.y;
                    m_cam.transform.position = m_currentUnitBase.m_angles.m_angle_Legs.position;
                    m_cam.transform.rotation = m_currentUnitBase.m_angles.m_angle_Legs.rotation;
                    break;
                case PropAngle.Feet:
                    m_currentIconSize = m_iconInfo_props.x;
                    m_cam.fieldOfView = m_iconInfo_props.y;
                    m_cam.transform.position = m_currentUnitBase.m_angles.m_angle_Feet.position;
                    m_cam.transform.rotation = m_currentUnitBase.m_angles.m_angle_Feet.rotation;
                    break;
                case PropAngle.Weapon:
                    m_currentIconSize = m_iconInfo_weapons.x;
                    m_cam.fieldOfView = m_iconInfo_weapons.y;
                    m_cam.transform.position = m_angle_Weapon.position;
                    m_cam.transform.rotation = m_angle_Weapon.rotation;
                    break;
                case PropAngle.Projectile:
                    m_currentIconSize = m_iconInfo_weapons.x;
                    m_cam.fieldOfView = m_iconInfo_weapons.y;
                    m_cam.transform.position = m_angle_Projectile.position;
                    m_cam.transform.rotation = m_angle_Projectile.rotation;
                    break;
                case PropAngle.MapProp:
                    m_currentIconSize = m_iconInfo_weapons.x;
                    m_cam.fieldOfView = m_iconInfo_weapons.y;
                    m_cam.transform.position = m_angle_MapProp.position;
                    m_cam.transform.rotation = m_angle_MapProp.rotation;
                    break;
                default:
                    m_currentIconSize = m_iconInfo_props.x;
                    m_cam.fieldOfView = m_iconInfo_props.y;
                    m_cam.transform.position = Vector3.zero;
                    m_cam.transform.rotation = Quaternion.identity;
                    break;
            }
        }

        public void FlushLogEditor()
        {
            if (!Application.isEditor)
            {
                return;
            }
            Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.LogEntries").GetMethod("Clear")
                .Invoke(new object(), null);
        }

        public void StitchArmature(GameObject mesh)
        {
            SkinnedMeshRenderer _skinnedMesh = mesh.GetComponentInChildren<SkinnedMeshRenderer>();
            _skinnedMesh.rootBone = m_currentRigTransforms.b_armature;

            Transform[] array = new Transform[_skinnedMesh.bones.Length];

            for (int i = 0; i < _skinnedMesh.bones.Length; i++)
            {
                foreach (Transform transform in m_currentRigTransforms.b_other.GetComponentsInChildren<Transform>())
                {
                    if (transform.name == _skinnedMesh.bones[i].name)
                    {
                        array[i] = transform;
                    }
                }
            }
            _skinnedMesh.bones = array;
        }

        void InstantiateAtBodyPart(GameObject _prop, UnitRig.GearType gearT, bool _skinnedMesh)
        {
            GameObject _spawnedObject;

            switch (gearT)
            {
                case UnitRig.GearType.HEAD:
                    _spawnedObject = UnityEngine.Object.Instantiate<GameObject>
                        (_prop, _skinnedMesh ? m_spawnedPropParent : m_currentRigTransforms.b_head);
                    _spawnedObject.transform.localPosition = Vector3.zero;
                    this.SwitchCameraAngle(ItemIconScene.PropAngle.Head);
                    break;

                case UnitRig.GearType.NECK:
                    _spawnedObject = UnityEngine.Object.Instantiate<GameObject>
                        (_prop, _skinnedMesh ? m_spawnedPropParent : m_currentRigTransforms.b_head);
                    _spawnedObject.transform.localPosition = Vector3.zero;
                    this.SwitchCameraAngle(ItemIconScene.PropAngle.Shoulders);
                    break;

                case UnitRig.GearType.SHOULDER:
                    _spawnedObject = UnityEngine.Object.Instantiate<GameObject>
                        (_prop, _skinnedMesh ? m_spawnedPropParent : m_currentRigTransforms.b_armL);
                    if (!_skinnedMesh) SpawnMirroredObject(_spawnedObject, m_currentRigTransforms.b_armR);
                    _spawnedObject.transform.localPosition = Vector3.zero;
                    this.SwitchCameraAngle(ItemIconScene.PropAngle.Shoulders);
                    break;

                case UnitRig.GearType.TORSO:
                    _spawnedObject = UnityEngine.Object.Instantiate<GameObject>
                        (_prop, _skinnedMesh ? m_spawnedPropParent : m_currentRigTransforms.b_torso);
                    _spawnedObject.transform.localPosition = Vector3.zero;
                    this.SwitchCameraAngle(ItemIconScene.PropAngle.Torso);
                    break;

                case UnitRig.GearType.ARMS:
                    _spawnedObject = UnityEngine.Object.Instantiate<GameObject>
                        (_prop, _skinnedMesh ? m_spawnedPropParent : m_currentRigTransforms.b_armL);
                    if (!_skinnedMesh) SpawnMirroredObject(_spawnedObject, m_currentRigTransforms.b_armR);
                    _spawnedObject.transform.localPosition = Vector3.zero;
                    this.SwitchCameraAngle(ItemIconScene.PropAngle.Torso);
                    break;

                case UnitRig.GearType.WRISTS:
                    _spawnedObject = UnityEngine.Object.Instantiate<GameObject>
                        (_prop, _skinnedMesh ? m_spawnedPropParent : m_currentRigTransforms.b_elbowL);
                    if (!_skinnedMesh) SpawnMirroredObject(_spawnedObject, m_currentRigTransforms.b_elbowR);
                    _spawnedObject.transform.localPosition = Vector3.zero;
                    this.SwitchCameraAngle(ItemIconScene.PropAngle.Wrists);
                    break;

                case UnitRig.GearType.WAIST:
                    _spawnedObject = UnityEngine.Object.Instantiate<GameObject>
                        (_prop, _skinnedMesh ? m_spawnedPropParent : m_currentRigTransforms.b_hip);
                    _spawnedObject.transform.localPosition = Vector3.zero;
                    this.SwitchCameraAngle(ItemIconScene.PropAngle.Hip);
                    break;

                case UnitRig.GearType.LEGS:
                    _spawnedObject = UnityEngine.Object.Instantiate<GameObject>
                        (_prop, _skinnedMesh ? m_spawnedPropParent : m_currentRigTransforms.b_legL);
                    if (!_skinnedMesh) SpawnMirroredObject(_spawnedObject, m_currentRigTransforms.b_legR);
                    _spawnedObject.transform.localPosition = Vector3.zero;
                    this.SwitchCameraAngle(ItemIconScene.PropAngle.Legs);
                    break;

                case UnitRig.GearType.FEET:
                    _spawnedObject = UnityEngine.Object.Instantiate<GameObject>
                        (_prop, _skinnedMesh ? m_spawnedPropParent : m_currentRigTransforms.b_kneeL);
                    if (!_skinnedMesh) SpawnMirroredObject(_spawnedObject, m_currentRigTransforms.b_kneeR);
                    _spawnedObject.transform.localPosition = Vector3.zero;
                    this.SwitchCameraAngle(ItemIconScene.PropAngle.Feet);
                    break;
                default:
                    _spawnedObject = UnityEngine.Object.Instantiate<GameObject>
                        (_prop, m_spawnedPropParent);
                    _spawnedObject.transform.localPosition = Vector3.zero;
                    this.SwitchCameraAngle(ItemIconScene.PropAngle.Default);
                    break;
            }

            if (_skinnedMesh) StitchArmature(_spawnedObject);

            if (_spawnedObject != null) m_toFlush.Add(_spawnedObject);
        }   

        void SpawnMirroredObject(GameObject _prop, Transform _bone)
        {
            GameObject _spawnedObject = UnityEngine.Object.Instantiate<GameObject>(_prop, _bone);
            _spawnedObject.transform.localScale = new Vector3(_spawnedObject.transform.localScale.x * -1f, _spawnedObject.transform.localScale.y, _spawnedObject.transform.localScale.z);
            m_toFlush.Add(_spawnedObject);
            _spawnedObject.transform.localPosition = Vector3.zero;
        }

        private IEnumerator AutoGeneratePropIcons()
        {
            foreach (var _prop in Directory.GetFiles(m_iconSaveData.m_directoryProps, "*.prefab", SearchOption.AllDirectories))
            {
                var _truePropPath = "Assets\\" + _prop.Split(new[] { "Assets\\" }, StringSplitOptions.None)[1];
                var _trueProp = AssetDatabase.LoadAssetAtPath<GameObject>(_truePropPath);

                var _propItemScript = _trueProp.GetComponent<PropItem>();
                var _propSkinnedMesh = _trueProp.GetComponentInChildren<SkinnedMeshRenderer>();

                if (_propItemScript.disableGooglyEyesOfParent && m_currentGooglyEyesParent.gameObject.activeInHierarchy) 
                    m_currentGooglyEyesParent.gameObject.SetActive(false);

                InstantiateAtBodyPart(_trueProp, _propItemScript.GearT, _propSkinnedMesh != null);

                yield return new WaitForSeconds(0.1f);
                AS_TakePhoto.Play();

                SaveRenderTexture(m_iconModPrefix + _propItemScript.name.ToString(), SaveDirectory.PropItems);

                for (int k = 0; k < this.m_toFlush.Count; k++) UnityEngine.Object.Destroy(this.m_toFlush[k]);
                this.m_toFlush.Clear();

                if (!m_currentGooglyEyesParent.gameObject.activeInHierarchy) m_currentGooglyEyesParent.gameObject.SetActive(true);
                this.FlushLogEditor();
            }
            yield break;
        }

        private IEnumerator AutoGenerateWeaponIcons()
        {
            for (int i = 0; i < m_weaponObjects.Count; i++)
            {
                if (!m_weaponObjects[i].activeInHierarchy) m_weaponObjects[i].SetActive(true);

                yield return new WaitForSeconds(0.1f);
                AS_TakePhoto.Play();

                SaveRenderTexture(m_iconModPrefix + m_weaponObjects[i].name.ToString(), SaveDirectory.Weapons);
                if (m_weaponObjects[i].activeInHierarchy) m_weaponObjects[i].SetActive(false);
                yield return null;
            }
            this.FlushLogEditor();
            yield break;
        }

        private IEnumerator AutoGenerateProjectileIcons()
        {
            for (int i = 0; i < m_projectileObjects.Count; i++)
            {
                if (!m_projectileObjects[i].activeInHierarchy) m_projectileObjects[i].SetActive(true);

                yield return new WaitForSeconds(0.1f);
                AS_TakePhoto.Play();

                SaveRenderTexture(m_iconModPrefix + m_projectileObjects[i].name.ToString(), SaveDirectory.Projectiles);
                if (m_projectileObjects[i].activeInHierarchy) m_projectileObjects[i].SetActive(false);
                yield return null;
            }
            this.FlushLogEditor();
            yield break;
        }

        //===================================================================================================================

        private static ItemIconScene instance;

        [Header("-REFERENCES-")]
        public Camera m_cam;
        public AudioSource AS_TakePhoto;

        [Header("-MANUAL PARENTS-")]
        public Transform m_spawnedPropParent;
        public List<GameObject> m_weaponObjects = new List<GameObject>();
        public List<GameObject> m_projectileObjects = new List<GameObject>();

        [Header("-MANUAL ANGLES-")]
        public Transform m_angle_Weapon;
        public Transform m_angle_Projectile;
        public Transform m_angle_MapProp;

        public List<UnitBaseData> m_allUnitBases = new List<UnitBaseData>();

        [Header("-PLAYMODE-")]
        public SaveDirectory m_currentSaveDirectory;

        List<GameObject> m_toFlush = new List<GameObject>();

        UnitBaseData m_currentUnitBase;
        Transform m_currentUnitBaseTransform;
        RigTransforms m_currentRigTransforms;
        Transform m_currentGooglyEyesParent;
        ItemIconScene.PropAngle m_currentCameraAngle;

        int m_currentIconSize;
        int m_currentFOV;
        int m_maxUnitBases;
        int m_maxGooglyEyes;

        //===================================================================================================================

        [FoldoutGroup("-ICON SAVE DATA-")] public string m_iconModPrefix = "UFoxMod_";
        [FoldoutGroup("-ICON SAVE DATA-")] public string m_iconItemName = "moddedItem";
        [FoldoutGroup("-ICON SAVE DATA-")] public IconSaveData m_iconSaveData;
        [FoldoutGroup("-ICON SAVE DATA-")] public Vector2Int m_iconInfo_props = new Vector2Int(256, 34);
        [FoldoutGroup("-ICON SAVE DATA-")] public Vector2Int m_iconInfo_weapons = new Vector2Int(512, 24);
        [FoldoutGroup("-ICON SAVE DATA-")][ButtonAttribute] public void UpdatePropDirectory() =>
            m_iconSaveData.m_directoryProps = UFoxEditorUtilities.GetCurrentFolderData();
        [FoldoutGroup("-ICON SAVE DATA-")][ButtonAttribute] public void UpdateWeaponDirectory() =>
            m_iconSaveData.m_directoryWeapons = UFoxEditorUtilities.GetCurrentFolderData();
        [FoldoutGroup("-ICON SAVE DATA-")][ButtonAttribute] public void UpdateProjectileDirectory() =>
            m_iconSaveData.m_directoryProjectiles = UFoxEditorUtilities.GetCurrentFolderData();

        //===================================================================================================================

        public class IconGenerationData
        {
            public string m_currentPropFolder;
            [ButtonAttribute] public void UpdatePropFolder() =>
            m_currentPropFolder = UFoxEditorUtilities.GetCurrentFolderData();
        }

        public class UnitBaseData
        {
            public Transform m_unitBaseRoot;
            public Transform m_googlyEyesParent; //select children as toggleable eyes
            public RigTransforms m_baseRigs;
            public AngleTransforms m_angles;

        }
        public class RigTransforms
        {
            //[FoldoutGroup("-BODY RIG TRANSFORM-")] public Transform b_googlyEyes;
            [FoldoutGroup("-BODY RIG TRANSFORM-")] public Transform b_armature;
            [FoldoutGroup("-BODY RIG TRANSFORM-")] public Transform b_head;
            [FoldoutGroup("-BODY RIG TRANSFORM-")] public Transform b_torso;
            [FoldoutGroup("-BODY RIG TRANSFORM-")] public Transform b_hip;
            [FoldoutGroup("-BODY RIG TRANSFORM-")] public Transform b_armL;
            [FoldoutGroup("-BODY RIG TRANSFORM-")] public Transform b_elbowL;
            [FoldoutGroup("-BODY RIG TRANSFORM-")] public Transform b_armR;
            [FoldoutGroup("-BODY RIG TRANSFORM-")] public Transform b_elbowR;
            [FoldoutGroup("-BODY RIG TRANSFORM-")] public Transform b_legL;
            [FoldoutGroup("-BODY RIG TRANSFORM-")] public Transform b_legR;
            [FoldoutGroup("-BODY RIG TRANSFORM-")] public Transform b_kneeL;
            [FoldoutGroup("-BODY RIG TRANSFORM-")] public Transform b_kneeR;
            [FoldoutGroup("-BODY RIG TRANSFORM-")] public Transform b_other;
        }

        public class AngleTransforms
        {
            [FoldoutGroup("-CAMERA ANGLE POSITION-")] public Transform m_angle_Head;
            [FoldoutGroup("-CAMERA ANGLE POSITION-")] public Transform m_angle_Torso;
            [FoldoutGroup("-CAMERA ANGLE POSITION-")] public Transform m_angle_Shoulders;
            [FoldoutGroup("-CAMERA ANGLE POSITION-")] public Transform m_angle_Wrists;
            [FoldoutGroup("-CAMERA ANGLE POSITION-")] public Transform m_angle_Hip;
            [FoldoutGroup("-CAMERA ANGLE POSITION-")] public Transform m_angle_Legs;
            [FoldoutGroup("-CAMERA ANGLE POSITION-")] public Transform m_angle_Feet;
        }

        public class IconSaveData
        {
            public string m_directoryProps;
            public string m_directoryWeapons;
            public string m_directoryProjectiles;
        }

        public enum PropAngle
        {
            Default,
            Head,
            Torso,
            Shoulders,
            Wrists,
            Hip,
            Legs,
            Feet,
            Weapon,
            Projectile,
            MapProp
        }

        public enum SaveDirectory
        {
            PropItems,
            Weapons,
            Projectiles
        }
    }
}
