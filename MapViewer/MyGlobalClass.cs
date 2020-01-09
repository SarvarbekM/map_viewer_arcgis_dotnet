using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Geodatabase;

namespace MapViewer
{
    public struct FieldsStruct
    {
        public string name;
        public esriFieldType type;
    }
    public static class MyGlobalClass
    {
        public static FieldsStruct[] fields;
    }
}
