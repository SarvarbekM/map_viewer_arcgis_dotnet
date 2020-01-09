using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace MapViewer
{
    public partial class CreateFieldForm : Form
    {
        public struct FieldsStruct
        {
            public string name;
            public esriFieldType type;
        }

        string[] types;

        public CreateFieldForm()
        {
            InitializeComponent();
            AddItemsField();
        }

        public void AddItemsField()
        {
            types = new string[9];
            types[0] = "Blob";
            types[1] = "Date";
            types[2] = "Double";
            types[3] = "Geometry";
            types[4] = "Integer";
            types[5] = "Raster";
            types[6] = "Text";
            types[7] = "GUID";
            types[8] = "Small Integer";
            FieldType.Items.Clear();

            for(int i=0;i<types.Length;i++)
            {
                FieldType.Items.Add(types[i]);
            }            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {                
                MyGlobalClass.fields = new MapViewer.FieldsStruct[dataGridView1.RowCount-1];
                for (int i = 0; i < dataGridView1.RowCount-1; i++)
                {
                    MyGlobalClass.fields[i].name = dataGridView1.Rows[i].Cells[0].Value.ToString();

                    switch (dataGridView1.Rows[i].Cells[1].Value.ToString())
                    {
                        case "Blob": MyGlobalClass.fields[i].type = esriFieldType.esriFieldTypeBlob; break;
                        case "Date": MyGlobalClass.fields[i].type = esriFieldType.esriFieldTypeDate; break;
                        case "Double": MyGlobalClass.fields[i].type = esriFieldType.esriFieldTypeDouble; break;
                        case "Geometry": MyGlobalClass.fields[i].type = esriFieldType.esriFieldTypeGeometry; break;
                        case "Integer": MyGlobalClass.fields[i].type = esriFieldType.esriFieldTypeInteger; break;
                        case "Raster": MyGlobalClass.fields[i].type = esriFieldType.esriFieldTypeRaster; break;
                        case "Text": MyGlobalClass.fields[i].type = esriFieldType.esriFieldTypeString; break;
                        case "GUID": MyGlobalClass.fields[i].type = esriFieldType.esriFieldTypeGUID; break;
                        case "Small Integer": MyGlobalClass.fields[i].type = esriFieldType.esriFieldTypeSmallInteger; break;
                    }
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
