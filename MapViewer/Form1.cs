using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;

namespace MapViewer
{
    public partial class Form1 : Form
    {
        #region Global vars
        //The popup menu
        private IToolbarMenu m_ToolbarMenu;
        //The envelope drawn on the MapControl
        private IEnvelope m_Envelope;
        //The symbol used to draw the envelope on the MapControl
        private object m_FillSymbol;
        //The PageLayoutControl's focus map events 
        private ITransformEvents_Event m_transformEvents;
        private ITransformEvents_VisibleBoundsUpdatedEventHandler visBoundsUpdatedE;
        //The CustomizeDialog used by the ToolbarControl
        private ICustomizeDialog m_CustomizeDialog;
        //The CustomizeDialog start event
        private ICustomizeDialogEvents_OnStartDialogEventHandler startDialogE;
        //The CustomizeDialog close event 
        private ICustomizeDialogEvents_OnCloseDialogEventHandler closeDialogE;

        IMapDocument mapdocument;
        //IMxDocument mxdocument;
        //   string mxfilename;
        IMap map;
        IPageLayoutControl2 m_pageLayoutControl;
        //IApplication m_application;
        string filenameforToolbar;
        #endregion
        public Form1()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Engine);
            ESRI.ArcGIS.RuntimeManager.BindLicense(ESRI.ArcGIS.ProductCode.EngineOrDesktop, ESRI.ArcGIS.LicenseLevel.Standard);
            LicenseInitializer l = new LicenseInitializer();
            l.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB }, new esriLicenseExtensionCode[] { esriLicenseExtensionCode.esriLicenseExtensionCodeGeoStats });
            string[] addres= System.Reflection.Assembly.GetExecutingAssembly().Location.Split(new char[] { '\\' });
            string add = null;
            for(int i=0;i<addres.Length-1;i++)
            {
                add=add+addres[i]+@"\";
            }
            add = add + @"PersistedItems.txt";
            filenameforToolbar = add;
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Create the customize dialog for the ToolbarControl
            CreateCustomizeDialog();

            //Create symbol used on the MapControl
            CreateOverviewSymbol();

            //Set label editing to manual
            axTOCControl1.LabelEdit = esriTOCControlEdit.esriTOCControlManual;

            //Get file name used to persist the ToolbarControl  
            if (System.IO.File.Exists(filenameforToolbar))
                LoadToolbarControlItems(filenameforToolbar);
            else
            {
                //Add generic commands
                //axToolbarControl1.AddItem("esriControls.ControlsOpenDocCommand", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
                axToolbarControl1.AddItem("esriControls.ControlsAddDataCommand", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
                //Add page layout navigation commands
                axToolbarControl1.AddItem("esriControls.ControlsPageZoomInTool", -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
                axToolbarControl1.AddItem("esriControls.ControlsPageZoomOutTool", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
                axToolbarControl1.AddItem("esriControls.ControlsPagePanTool", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
                axToolbarControl1.AddItem("esriControls.ControlsPageZoomWholePageCommand", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
                //Add map navigation commands
                axToolbarControl1.AddItem("esriControls.ControlsMapZoomInTool", -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
                axToolbarControl1.AddItem("esriControls.ControlsMapZoomOutTool", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
                axToolbarControl1.AddItem("esriControls.ControlsMapPanTool", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
                axToolbarControl1.AddItem("esriControls.ControlsMapFullExtentCommand", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
                axToolbarControl1.AddItem("esriControls.ControlsMapZoomToLastExtentBackCommand", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
                axToolbarControl1.AddItem("esriControls.ControlsMapZoomToLastExtentForwardCommand", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
                //Add map inquiry commands
                axToolbarControl1.AddItem("esriControls.ControlsMapIdentifyTool", -1, -1, true, 0, esriCommandStyles.esriCommandStyleIconOnly);
                axToolbarControl1.AddItem("esriControls.ControlsMapFindCommand", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
                axToolbarControl1.AddItem("esriControls.ControlsMapMeasureTool", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
                //Add custom AddDateTool
                //axToolbarControl1.AddItem("Commands.AddDateTool", -1, -1, false, 0, esriCommandStyles.esriCommandStyleIconAndText);

                //Create a new ToolbarPalette
                IToolbarPalette toolbarPalette = new ToolbarPalette();
                //Add commands and tools to the ToolbarPalette
                toolbarPalette.AddItem("esriControls.ControlsNewMarkerTool", -1, -1);
                toolbarPalette.AddItem("esriControls.ControlsNewLineTool", -1, -1);
                toolbarPalette.AddItem("esriControls.ControlsNewCircleTool", -1, -1);
                toolbarPalette.AddItem("esriControls.ControlsNewEllipseTool", -1, -1);
                toolbarPalette.AddItem("esriControls.ControlsNewRectangleTool", -1, -1);
                toolbarPalette.AddItem("esriControls.ControlsNewPolygonTool", -1, -1);
                //Add the ToolbarPalette to the ToolbarControl
                axToolbarControl1.AddItem(toolbarPalette, 0, -1, false, 0, esriCommandStyles.esriCommandStyleIconOnly);
            }

            //Create a new ToolbarMenu
            m_ToolbarMenu = new ToolbarMenu();
            //Share the ToolbarControl's command pool
            m_ToolbarMenu.CommandPool = axToolbarControl1.CommandPool;
            //Set the hook to the PageLayoutControl
            m_ToolbarMenu.SetHook(axPageLayoutControl1);
            //Add commands to the ToolbarMenu
            m_ToolbarMenu.AddItem("esriControls.ControlsPageZoomInFixedCommand", -1, -1, false, esriCommandStyles.esriCommandStyleIconAndText);
            m_ToolbarMenu.AddItem("esriControls.ControlsPageZoomOutFixedCommand", -1, -1, false, esriCommandStyles.esriCommandStyleIconAndText);
            m_ToolbarMenu.AddItem("esriControls.ControlsPageZoomWholePageCommand", -1, -1, false, esriCommandStyles.esriCommandStyleIconAndText);
            m_ToolbarMenu.AddItem("esriControls.ControlsPageZoomPageToLastExtentBackCommand", -1, -1, true, esriCommandStyles.esriCommandStyleIconAndText);
            m_ToolbarMenu.AddItem("esriControls.ControlsPageZoomPageToLastExtentForwardCommand", -1, -1, false, esriCommandStyles.esriCommandStyleIconAndText);

            //Set buddy controls
            axTOCControl1.SetBuddyControl(axPageLayoutControl1);
            axToolbarControl1.SetBuddyControl(axPageLayoutControl1);
        }

        private void axPageLayoutControl1_OnPageLayoutReplaced(object sender, IPageLayoutControlEvents_OnPageLayoutReplacedEvent e)
        {
            //Get the IActiveView of the focus map in the PageLayoutControl
            IActiveView activeView = axPageLayoutControl1.ActiveView.FocusMap as IActiveView;
            //Trap the ITranformEvents of the PageLayoutCntrol's focus map 
            visBoundsUpdatedE = new ITransformEvents_VisibleBoundsUpdatedEventHandler(OnVisibleBoundsUpdated);
            IDisplayTransformation displayTransformation = activeView.ScreenDisplay.DisplayTransformation;
            //Start listening to the transform events interface
            m_transformEvents = (ITransformEvents_Event)displayTransformation;
            //Start listening to the VisibleBoundsUpdated method on ITransformEvents interface
            m_transformEvents.VisibleBoundsUpdated += visBoundsUpdatedE;
            //Get the extent of the focus map
            m_Envelope = activeView.Extent;

            //Load the same pre-authored map document into the MapControl
            axMapControl1.LoadMxFile(mapdocument.DocumentFilename, null, null);
            //Set the extent of the MapControl to the full extent of the data
            axMapControl1.Extent = axMapControl1.FullExtent;

        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            //Suppress data redraw and draw bitmap instead
            axMapControl1.SuppressResizeDrawing(true, 0);
            axPageLayoutControl1.SuppressResizeDrawing(true, 0);
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            //Stop bitmap draw and draw data
            axMapControl1.SuppressResizeDrawing(false, 0);
            axPageLayoutControl1.SuppressResizeDrawing(false, 0);
        }

        private void axPageLayoutControl1_OnMouseDown(object sender, IPageLayoutControlEvents_OnMouseDownEvent e)
        {
            //Popup the ToolbarMenu
            if (e.button == 2)
                m_ToolbarMenu.PopupMenu(e.x, e.y, axPageLayoutControl1.hWnd);
        }

        private void axTOCControl1_OnEndLabelEdit(object sender, ITOCControlEvents_OnEndLabelEditEvent e)
        {
            //If the new label is an empty string then prevent the edit
            if (e.newLabel.Trim() == "") e.canEdit = false;
        }

        private void CreateOverviewSymbol()
        {
            //Get the IRGBColor interface
            IRgbColor color = new RgbColor();
            //Set the color properties
            color.RGB = 255;
            //Get the ILine symbol interface
            ILineSymbol outline = new SimpleLineSymbol();
            //Set the line symbol properties
            outline.Width = 1.5;
            outline.Color = color;
            //Get the IFillSymbol interface
            ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbol();
            //Set the fill symbol properties
            simpleFillSymbol.Outline = outline;
            simpleFillSymbol.Style = esriSimpleFillStyle.esriSFSHollow;
            m_FillSymbol = simpleFillSymbol;
        }

        private void OnVisibleBoundsUpdated(IDisplayTransformation sender, bool sizeChanged)
        {
            //Set the extent to the new visible extent
            m_Envelope = sender.VisibleBounds;
            //Refresh the MapControl's foreground phase
            axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
        }

        private void axMapControl1_OnAfterDraw(object sender, IMapControlEvents2_OnAfterDrawEvent e)
        {
            if (m_Envelope == null) return;

            //If the foreground phase has drawn      
            esriViewDrawPhase viewDrawPhase = (esriViewDrawPhase)e.viewDrawPhase;
            if (viewDrawPhase == esriViewDrawPhase.esriViewForeground)
            {
                IGeometry geometry = m_Envelope;
                axMapControl1.DrawShape(geometry, ref m_FillSymbol);
            }
        }

        private void CreateCustomizeDialog()
        {
            //Create new customize dialog 
            m_CustomizeDialog = new CustomizeDialog();
            //Set the title
            m_CustomizeDialog.DialogTitle = "Customize ToolbarControl Items";
            //Show the 'Add from File' button
            m_CustomizeDialog.ShowAddFromFile = true;
            //Set the ToolbarControl that new items will be added to
            m_CustomizeDialog.SetDoubleClickDestination(axToolbarControl1);

            //Set the customize dialog events 
            startDialogE = new ICustomizeDialogEvents_OnStartDialogEventHandler(OnStartDialog);
            ((ICustomizeDialogEvents_Event)m_CustomizeDialog).OnStartDialog += startDialogE;
            closeDialogE = new ICustomizeDialogEvents_OnCloseDialogEventHandler(OnCloseDialog);
            ((ICustomizeDialogEvents_Event)m_CustomizeDialog).OnCloseDialog += closeDialogE;
        }

        private void OnStartDialog()
        {
            axToolbarControl1.Customize = true;
        }

        private void OnCloseDialog()
        {
            axToolbarControl1.Customize = false;            
            customizeToolStripMenuItem.Checked = false;
        }

        private void SaveToolbarControlItems(string filePath)
        {
            //Create a MemoryBlobStream
            IBlobStream blobStream = new MemoryBlobStream();
            //Get the IStream interface
            IStream stream = blobStream;

            //Save the ToolbarControl into the stream
            axToolbarControl1.SaveItems(stream);
            //Save the stream to a file
            blobStream.SaveToFile(filePath);
        }

        private void LoadToolbarControlItems(string filePath)
        {
            //Create a MemoryBlobStream
            IBlobStream blobStream = new MemoryBlobStream();
            //Get the IStream interface
            IStream stream = blobStream;

            //Load the stream from the file
            blobStream.LoadFromFile(filePath);
            //Load the stream into the ToolbarControl
            axToolbarControl1.LoadItems(stream);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure the exit system?", "Exit system", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SaveToolbarControlItems(filenameforToolbar);
                try
                {
                    mapdocument.Close();
                }
                catch (Exception)
                {

                }
                Application.ExitThread();
                Application.Exit();
            }


        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Exit if there is no system default printer
            if (axPageLayoutControl1.Printer == null)
            {
                MessageBox.Show("Unable to print!", "No default printer");
                return;
            }

            //Set printer papers orientation to that of the Page
            axPageLayoutControl1.Printer.Paper.Orientation = axPageLayoutControl1.Page.Orientation;
            //Scale to the page
            axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingScale;
            //Send the pagelayout to the printer
            axPageLayoutControl1.PrintPageLayout();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filter = "Map document (*.mxd)|*.mxd";
            openFileDialog1.Filter = filter;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;
                LoadMxdFile(filename);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filter = "Map document (*.mxd)|*.mxd";
            saveFileDialog1.Filter = filter;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;
                //mxdocument = new MxDocumentClass();
                mapdocument = new MapDocument();
                mapdocument.New(filename);
                mapdocument.SaveAs(filename);
                mapdocument.Close();
                LoadMxdFile(filename);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = mapdocument.DocumentFilename;
            mapdocument.Close();
            IMapDocument newmapdocument = new MapDocument();
            m_pageLayoutControl = (IPageLayoutControl2)axPageLayoutControl1.Object;
            //m_pageLayoutControl.PageLayout = mapdocument.PageLayout;
            newmapdocument.Open(filename);
            newmapdocument.ReplaceContents((IMxdContents)m_pageLayoutControl.PageLayout);
            newmapdocument.Save(newmapdocument.UsesRelativePaths, false);
            newmapdocument.Close();
            LoadMxdFile(filename);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filter = "Map document (*.mxd)|*.mxd";
            saveFileDialog1.Filter = filter;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;
                IMapDocument newmapdocument = new MapDocument();
                newmapdocument.New(filename);
                m_pageLayoutControl = (IPageLayoutControl2)axPageLayoutControl1.Object;
                newmapdocument.ReplaceContents((IMxdContents)m_pageLayoutControl.PageLayout);
                newmapdocument.Save(newmapdocument.UsesRelativePaths, false);
                newmapdocument.Close();
                LoadMxdFile(filename);
            }
        }

        private void LoadMxdFile(string filename)
        {
            if (axPageLayoutControl1.CheckMxFile(filename))
            {
                //ESRI.ArcGIS.Framework.IDocument doc = new ESRI.ArcGIS.ArcMapUI.MxDocumentClass();
                //m_application = doc.Parent;
                //m_application.Visible = false;
                //mxdocument = m_application.Document as IMxDocument;
                mapdocument = new MapDocument();
                mapdocument.Open(filename);
                map = mapdocument.Map[0];
                mapdocument.SetActiveView((IActiveView)map);
                m_pageLayoutControl = (IPageLayoutControl2)axPageLayoutControl1.Object;
                m_pageLayoutControl.PageLayout = mapdocument.PageLayout;
                //axPageLayoutControl1.LoadMxFile(filename);                    
                this.Text = mapdocument.DocumentFilename;

                saveAsToolStripMenuItem.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
                printToolStripMenuItem.Enabled = true;
            }
            else
            {
                MessageBox.Show("The Selected file is not suppoted !!!");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            //Exit if not right mouse button
            if (e.button != 2) return;

            IBasicMap map = new MapClass();
            ILayer layer = new FeatureLayerClass();
            object other = new object();
            object index = new object();
            esriTOCControlItem item = new esriTOCControlItem();

            //Determine what kind of item has been clicked on
            axTOCControl1.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);

            //QI to IFeatureLayer and IGeoFeatuerLayer interface
            if (layer == null) return;
            IFeatureLayer featureLayer = layer as IFeatureLayer;
            if (featureLayer == null) return;
            IGeoFeatureLayer geoFeatureLayer = (IGeoFeatureLayer)featureLayer;
            ISimpleRenderer simpleRenderer = (ISimpleRenderer)geoFeatureLayer.Renderer;

            //Create the form with the SymbologyControl
            frmSymbol symbolForm = new frmSymbol();

            //Get the IStyleGalleryItem
            IStyleGalleryItem styleGalleryItem = null;
            //Select SymbologyStyleClass based upon feature type
            switch (featureLayer.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    styleGalleryItem = symbolForm.GetItem(esriSymbologyStyleClass.esriStyleClassMarkerSymbols, simpleRenderer.Symbol);
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    styleGalleryItem = symbolForm.GetItem(esriSymbologyStyleClass.esriStyleClassLineSymbols, simpleRenderer.Symbol);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    styleGalleryItem = symbolForm.GetItem(esriSymbologyStyleClass.esriStyleClassFillSymbols, simpleRenderer.Symbol);
                    break;
                case esriGeometryType.esriGeometryMultipoint:
                    styleGalleryItem = symbolForm.GetItem(esriSymbologyStyleClass.esriStyleClassMarkerSymbols, simpleRenderer.Symbol);
                    break;
            }

            //Release the form
            symbolForm.Dispose();
            this.Activate();

            if (styleGalleryItem == null) return;

            //Create a new renderer
            simpleRenderer = new SimpleRendererClass();
            //Set its symbol from the styleGalleryItem
            simpleRenderer.Symbol = (ISymbol)styleGalleryItem.Item;
            //Set the renderer into the geoFeatureLayer
            geoFeatureLayer.Renderer = (IFeatureRenderer)simpleRenderer;

            //Fire contents changed event that the TOCControl listens to
            axPageLayoutControl1.ActiveView.ContentsChanged();
            //Refresh the display
            axPageLayoutControl1.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(textBox1.Text) && !String.IsNullOrEmpty(textBox2.Text))
                {
                    string databasefilename = openFileDialog1.FileName;
                    IWorkspace workspace = AccessWorkspaceFromPropertySet(databasefilename);

                    ISpatialReference spatialreference = map.SpatialReference;
                    IFeatureClassDescription fcDesc = new FeatureClassDescriptionClass();
                    IObjectClassDescription ocDesc = (IObjectClassDescription)fcDesc;                    
                    IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
                    IFeatureWorkspace featureworkspace = (IFeatureWorkspace)workspace;
                    IFeatureClass featureclass = CreateFeatureClass(textBox1.Text, featureworkspace);
                    if (featureclass == null)
                    {
                        MessageBox.Show("Feature Class not created");
                    }
                    else
                    {
                        MessageBox.Show("Feature Class successfull created");
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            textBox1.Clear();
            textBox2.Clear();
            panel3.Visible = false;
        }

        public IFeatureClass CreateFeatureClass(String featureClassName, IFeatureWorkspace featureWorkspace)
        {
            CreateFieldForm f = new CreateFieldForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
                    IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;
                    IFields fields = ocDescription.RequiredFields;
                    IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
                    for (int i = 0; i < MyGlobalClass.fields.Length; i++)
                    {
                        IField field = new FieldClass();
                        IFieldEdit fieldEdit = (IFieldEdit)field;
                        fieldEdit.Name_2 = MyGlobalClass.fields[i].name;
                        fieldEdit.Type_2 = MyGlobalClass.fields[i].type;                        
                        fieldsEdit.AddField(field);
                    }
                    IFieldChecker fieldChecker = new FieldCheckerClass();
                    IEnumFieldError enumFieldError = null;
                    IFields validatedFields = null;
                    fieldChecker.ValidateWorkspace = (IWorkspace)featureWorkspace;
                    fieldChecker.Validate(fields, out enumFieldError, out validatedFields);
                    IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(featureClassName, validatedFields,ocDescription.InstanceCLSID, ocDescription.ClassExtensionCLSID, esriFeatureType.esriFTSimple,fcDescription.ShapeFieldName, "");
                    return featureClass;
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Error is function CreateFeatureClass: " + ex.Message);
                }
            }

            return null;
        }

        public IWorkspace AccessWorkspaceFromPropertySet(String database)
        {
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("DATABASE", database);
            Type factoryType = Type.GetTypeFromProgID(
              "esriDataSourcesGDB.AccessWorkspaceFactory");
            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)
              Activator.CreateInstance(factoryType);
            return workspaceFactory.Open(propertySet, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
            textBox1.Clear();
            textBox2.Clear();
        }

        private void createTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel3.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string filter = "Ms Access database(*.mdb)|*.mdb";
            openFileDialog1.Filter = filter;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = openFileDialog1.FileName;
            }
        }

        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customizeToolStripMenuItem.Checked = !customizeToolStripMenuItem.Checked;            
            //Show or hide the customize dialog
            if (customizeToolStripMenuItem.Checked == false)
                m_CustomizeDialog.CloseDialog();
            else
                m_CustomizeDialog.StartDialog(axToolbarControl1.hWnd);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
