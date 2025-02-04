﻿using UnityEngine;
using VBspViewer.Importing.Entities;

namespace VBspViewer.Behaviours.Entities
{
    [ClassName(HammerName = "func_brush", ClassName = "CFuncBrush")]
    public class BrushEntity : BaseEntity
    {
        public int ModelIndex = -1;

        protected override void OnKeyVal(string key, EntValue val)
        {
            switch (key)
            {
                case "model":
                    var valStr = (string) val;
                    if (valStr.StartsWith("*")) ModelIndex = int.Parse(valStr.Substring(1));
                    break;
                default:
                    base.OnKeyVal(key, val);
                    break;
            }
        }

        protected override void OnReadProperty<TVal>(string name, int index, TVal value)
        {
            switch (name)
            {
                case "m_nModelIndex":
                    ModelIndex = (int) (object) value;
                    return;
                default:
                    base.OnReadProperty(name, index, value);
                    return;
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (ModelIndex < 0) return;
            if (World == null || World.BspFile == null) return;

            var worldModelIndex = ModelIndex;

            if (ModelIndex > 0)
            {
                var modelTable = World.NetClient.GetStringTable("modelprecache");
                var modelName = modelTable[ModelIndex];

                if (!modelName.StartsWith("*")) return;

                worldModelIndex = int.Parse(modelName.Substring(1));
            }

            var meshes = World.BspFile.GenerateMeshes(worldModelIndex);
            foreach (var group in meshes)
            {
                if (group.Mesh.vertexCount == 0) continue;

                var modelChild = new GameObject("faces", typeof(MeshFilter), typeof(MeshRenderer));
                modelChild.transform.SetParent(transform, false);

                modelChild.GetComponent<MeshFilter>().sharedMesh = group.Mesh;
                modelChild.GetComponent<MeshRenderer>().sharedMaterials = group.Materials;
                modelChild.isStatic = ModelIndex == 0;
            }
        }
    }
}
