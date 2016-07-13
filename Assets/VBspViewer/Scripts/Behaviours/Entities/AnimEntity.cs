﻿using System;
using JetBrains.Annotations;
using UnityEngine;
using VBspViewer.Importing.Entities;
using VBspViewer.Importing.Mdl;
using VBspViewer.Importing.Vmt;

namespace VBspViewer.Behaviours.Entities
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class AnimEntity : BaseEntity
    {
        private string _curModel;

        private MeshRenderer _renderer;
        private MeshFilter _meshFilter;

        public MeshRenderer Renderer { get { return _renderer; } }

        public string Model;

        protected virtual Mesh OnGetMesh(MdlFile mdl, int lod)
        {
            return mdl.GetMesh(lod, null);
        }

        public void SetModel(string mdlPath)
        {
            if (StringComparer.InvariantCultureIgnoreCase.Equals(mdlPath, _curModel)) return;
            _curModel = Model = mdlPath;

            if (string.IsNullOrEmpty(mdlPath))
            {
                _meshFilter.sharedMesh = null;
                return;
            }

            try
            {
                var mdl = World.Resources.LoadMdl(mdlPath);

                _meshFilter.sharedMesh = OnGetMesh(mdl, 0);

                var mats = new Material[_meshFilter.sharedMesh.subMeshCount];

                for (var i = 0; i < mats.Length; ++i)
                {
                    try
                    {
                        var matName = mdl.GetMaterialName(0, i);
                        var mat = World.Resources.LoadVmt(matName);
                        mats[i] = mat.GetMaterial(World.Resources);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        mats[i] = VmtFile.GetDefaultMaterial();
                    }
                }

                _renderer.sharedMaterials = mats;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _meshFilter.sharedMesh = null;
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            _renderer = GetComponent<MeshRenderer>();
            _meshFilter = GetComponent<MeshFilter>();
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (string.IsNullOrEmpty(_curModel)) SetModel(Model);
        }

        protected override void OnKeyVal(string key, EntValue val)
        {
            switch (key)
            {
                case "model":
                    Model = (string) val;
                    break;
                default:
                    base.OnKeyVal(key, val);
                    break;
            }
        }

        [UsedImplicitly]
        private void Update()
        {
            if (!StringComparer.InvariantCultureIgnoreCase.Equals(Model, _curModel)) SetModel(Model);
        }
    }
}