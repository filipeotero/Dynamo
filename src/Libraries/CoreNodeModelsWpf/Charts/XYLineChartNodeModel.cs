using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Autodesk.DesignScript.Runtime;
using CoreNodes.ChartHelpers;
using CoreNodeModelsWpf.Charts.Controls;
using CoreNodeModelsWpf.Charts.Utilities;
using Dynamo.Controls;
using Dynamo.Graph.Nodes;
using Dynamo.Wpf;
using Newtonsoft.Json;
using ProtoCore.AST.AssociativeAST;
using DynamoServices;
using Dynamo.Wpf.Properties;

namespace CoreNodeModelsWpf.Charts
{
    [IsDesignScriptCompatible]
    [NodeName("XY Line Plot")]
    [NodeCategory("Display.Charts.Create")]        
    [NodeDescription("ChartsXYLineChartDescription", typeof(CoreNodeModelWpfResources))]
    [NodeSearchTags("ChartsXYLineChartSearchTags", typeof(CoreNodeModelWpfResources))]

    [InPortTypes("List<string>", "List<List<double>>", "List<List<double>>", "List<color>")]
    [OutPortTypes("Dictionary<string, double>")]
    [AlsoKnownAs("CoreNodeModelsWpf.Charts.XYLinePlot")]
    public class XYLineChartNodeModel : NodeModel
    {
        #region Properties
        private Random rnd = new Random();

        /// <summary>
        /// A list of Labels for each line to be plotted.
        /// </summary>
        public List<string> Labels { get; set; }

        /// <summary>
        /// List of lists each containing double values representing x-coordinates.
        /// </summary>
        public List<List<double>> XValues { get; set; }

        /// <summary>
        /// List of lists each containing double values representing y-coordinates.
        /// </summary>
        public List<List<double>> YValues { get; set; }

        /// <summary>
        /// A list of color values, one for each plotted line.
        /// </summary>
        public List<SolidColorBrush> Colors { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Instantiate a new NodeModel instance.
        /// </summary>
        public XYLineChartNodeModel()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("labels", "A list of string labels for each line to be plotted")));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("x-values", "A list of lists each containing double values representing x-coordinates for each point in a line.")));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("y-values", "A list of lists each containing double values representing y-coordinates for each point in a line.")));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("colors", "A list of colors for each line in the line plot.")));

            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("labels:values", "Dictionary containing label:value key-pairs")));

            RegisterAllPorts();

            PortDisconnected += XYLineChartNodeModel_PortDisconnected;

            ArgumentLacing = LacingStrategy.Disabled;
        }

        [JsonConstructor]
        /// <summary>
        /// Instantiate a new NodeModel instance.
        /// </summary>
        public XYLineChartNodeModel(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
            PortDisconnected += XYLineChartNodeModel_PortDisconnected;
        }
        #endregion

        #region Events
        private void XYLineChartNodeModel_PortDisconnected(PortModel port)
        {
            // Clear UI when a input port is disconnected
            if (port.PortType == PortType.Input && this.State == ElementState.Active)
            {
                Labels.Clear();
                XValues.Clear();
                YValues.Clear();
                Colors.Clear();

                RaisePropertyChanged("DataUpdated");
            }
        }
        #endregion

        #region Databridge
        // Use the VMDataBridge to safely retrieve our input values

        /// <summary>
        /// Register the data bridge callback.
        /// </summary>
        protected override void OnBuilt()
        {
            base.OnBuilt();
            VMDataBridge.DataBridge.Instance.RegisterCallback(GUID.ToString(), DataBridgeCallback);
        }

        /// <summary>
        /// Callback method for DataBridge mechanism.
        /// This callback only gets called when 
        ///     - The AST is executed
        ///     - After the BuildOutputAST function is executed 
        ///     - The AST is fully built
        /// </summary>
        /// <param name="data">The data passed through the data bridge.</param>
        private void DataBridgeCallback(object data)
        {
            // Grab input data which always returned as an ArrayList
            var inputs = data as ArrayList;

            // Each of the list inputs are also returned as ArrayLists
            var labels = inputs[0] as ArrayList;
            var xValues = inputs[1] as ArrayList;
            var yValues = inputs[2] as ArrayList;
            var colors = inputs[3] as ArrayList;

            // Only continue if key/values match in length
            if (labels.Count != xValues.Count || xValues.Count != yValues.Count || labels.Count < 1)
            {
                throw new Exception("Label and Values do not properly align in length.");
            }

            // Clear current chart values
            Labels = new List<string>();
            XValues = new List<List<double>>();
            YValues = new List<List<double>>();
            Colors = new List<SolidColorBrush>();

            // If color count doesn't match title count use random colors
            if (colors == null || colors.Count == 0 || colors.Count != labels.Count)
            {
                // In case colors are not provided, we supply some from the default library of colors
                Info(Dynamo.Wpf.Properties.CoreNodeModelWpfResources.ProvideDefaultColorsWarningMessage);

                for (var i = 0; i < labels.Count; i++)
                {
                    var outputXValues = new List<double>();
                    var outputYValues = new List<double>();

                    var unpackedXValues = xValues[i] as ArrayList;
                    var unpackedYValues = yValues[i] as ArrayList;

                    for (var j = 0; j < unpackedXValues.Count; j++)
                    {
                        outputXValues.Add(Convert.ToDouble(unpackedXValues[j]));
                        outputYValues.Add(Convert.ToDouble(unpackedYValues[j]));
                    }

                    Labels.Add((string)labels[i]);
                    XValues.Add(outputXValues);
                    YValues.Add(outputYValues);

                    Color color = Utilities.Colors.GetColor();
                    SolidColorBrush brush = new SolidColorBrush(color);
                    brush.Freeze();
                    Colors.Add(brush);
                }

                Utilities.Colors.ResetColors();
            }
            // Else all inputs should be consistent in length
            else
            {
                for (var i = 0; i < labels.Count; i++)
                {
                    var outputXValues = new List<double>();
                    var outputYValues = new List<double>();

                    var unpackedXValues = xValues[i] as ArrayList;
                    var unpackedYValues = yValues[i] as ArrayList;

                    for (var j = 0; j < unpackedXValues.Count; j++)
                    {
                        outputXValues.Add(Convert.ToDouble(unpackedXValues[j]));
                        outputYValues.Add(Convert.ToDouble(unpackedYValues[j]));
                    }

                    Labels.Add((string)labels[i]);
                    XValues.Add(outputXValues);
                    YValues.Add(outputYValues);

                    var dynColor = (DSCore.Color)colors[i];
                    var convertedColor = Color.FromArgb(dynColor.Alpha, dynColor.Red, dynColor.Green, dynColor.Blue);
                    SolidColorBrush brush = new SolidColorBrush(convertedColor);
                    brush.Freeze();
                    Colors.Add(brush);
                }
            }

            // Notify UI the data has been modified
            RaisePropertyChanged("DataUpdated");
        }
        #endregion

        #region Methods
        /// <summary>
        /// BuildOutputAst is where the outputs of this node are calculated.
        /// This method is used to do the work that a compiler usually does 
        /// by parsing the inputs List inputAstNodes into an abstract syntax tree.
        /// </summary>
        /// <param name="inputAstNodes"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // WARNING!!!
            // Do not throw an exception during AST creation.

            // If inputs are not connected return null
            if (!InPorts[0].IsConnected ||
                !InPorts[1].IsConnected ||
                !InPorts[2].IsConnected)
            {
                return new[]
                {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()),
                };
            }

            AssociativeNode inputNode = AstFactory.BuildFunctionCall(
                new Func<List<string>, List<List<double>>, List<List<double>>, List<DSCore.Color>, Dictionary<string, Dictionary<string, List<double>>>>(XYLineChartFunctions.GetNodeInput),
                new List<AssociativeNode> { inputAstNodes[0], inputAstNodes[1], inputAstNodes[2], inputAstNodes[3] }
            );

            return new[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), inputNode),
                    AstFactory.BuildAssignment(
                        AstFactory.BuildIdentifier(AstIdentifierBase + "_dummy"),
                        VMDataBridge.DataBridge.GenerateBridgeDataAst(GUID.ToString(), AstFactory.BuildExprList(inputAstNodes)
                    )
                ),
            };
        }
        #endregion

        #region Dispose

        /// <summary>
        /// Finalize the usage of this Node
        /// </summary>
        public override void Dispose()
        {
            PortDisconnected -= XYLineChartNodeModel_PortDisconnected;
            VMDataBridge.DataBridge.Instance.UnregisterCallback(GUID.ToString());
        }

        #endregion
    }

    /// <summary>
    ///     View customizer for CustomNodeModel Node Model.
    /// </summary>
    public class XYLineChartNodeView : INodeViewCustomization<XYLineChartNodeModel>
    {
        private XYLineChartControl xyLineChartControl;

        /// <summary>
        /// At run-time, this method is called during the node 
        /// creation. Add custom UI element to the node view.
        /// </summary>
        /// <param name="model">The NodeModel representing the node's core logic.</param>
        /// <param name="nodeView">The NodeView representing the node in the graph.</param>
        public void CustomizeView(XYLineChartNodeModel model, NodeView nodeView)
        {
            xyLineChartControl = new XYLineChartControl(model);
            nodeView.inputGrid.Children.Add(xyLineChartControl);

            MenuItem exportImage = new MenuItem();
            exportImage.Header = "Export Chart as Image";
            exportImage.Click += ExportImage_Click;

            var contextMenu = (nodeView.Content as Grid).ContextMenu;
            contextMenu.Items.Add(exportImage);
        }

        private void ExportImage_Click(object sender, RoutedEventArgs e)
        {
            Export.ToPng(xyLineChartControl.XYLineChart);
        }

        /// <summary>
        /// Here you can do any cleanup you require if you've assigned callbacks for particular 
        /// UI events on your node.
        /// </summary>
        public void Dispose() { }
    }
}
