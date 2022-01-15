using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using netDxf;

namespace ETLOK.Core.Config
{
    public class Bootstrap : UnitySingleton<Bootstrap>
    {
        [SerializeReference] private DxfDocument dxfDoc = null;

        public override void Awake()
        {
            base.Awake();
            ReadDXF();
        }

        public void ReadDXF()
        {
            dxfDoc = DxfDocument.Load(Application.dataPath + "/../" + "sample.dxf");
        }

        public void CloseDXF()
        {
            dxfDoc = null;
        }
    }
}