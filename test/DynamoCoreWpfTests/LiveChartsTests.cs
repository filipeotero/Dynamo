using Dynamo.Graph.Workspaces;
using DynamoCoreWpfTests.Utility;
using NUnit.Framework;
using System;
using System.Linq;
using SystemTestServices;
using CoreNodeModelsWpf.Charts;
using Dynamo.Graph.Nodes;
using DynCmd = Dynamo.Models.DynamoModel;

namespace DynamoCoreWpfTests
{
    [TestFixture]
    class LiveChartsTests : SystemTestBase
    {
        private CodeBlockNodeModel CreateCodeBlockNode()
        {
            var cbn = new CodeBlockNodeModel(this.Model.LibraryServices);
            var command = new DynCmd.CreateNodeCommand(cbn, 0, 0, true, false);

            this.Model.ExecuteCommand(command);

            Assert.IsNotNull(cbn);
            return cbn;
        }

        private void UpdateCodeBlockNodeContent(CodeBlockNodeModel cbn, string value)
        {
            var command = new DynCmd.UpdateModelValueCommand(Guid.Empty, cbn.GUID, "Code", value);
            this.Model.ExecuteCommand(command);
        }

        [Test]
        public void LiveChartsBarChartCreationTest()
        {
            var homespace = Model.CurrentWorkspace as HomeWorkspaceModel;
            Assert.NotNull(homespace, "The current workspace is not a HomeWorkspaceModel");

            var chart = new BarChartNodeModel();
            Model.AddNodeToCurrentWorkspace(chart, true);
            homespace.Run();
            Assert.DoesNotThrow(DispatcherUtil.DoEvents);
            Assert.AreEqual(1, homespace.Nodes.Count());

            string codeA = "[\"January\", \"February\", \"March\"];";
            string codeB = "[[4, 12, 34],[14, 22, 14],[15, 3, 6]];";

            //This create a new Code Block node and update the content
            var codeBlockNodeA = CreateCodeBlockNode();
            var codeBlockNodeB = CreateCodeBlockNode();
            UpdateCodeBlockNodeContent(codeBlockNodeA, @codeA);
            UpdateCodeBlockNodeContent(codeBlockNodeB, @codeB);

            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeA.OutPorts.FirstOrDefault(), chart.InPorts[0], Guid.NewGuid());
            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeB.OutPorts.FirstOrDefault(), chart.InPorts[1], Guid.NewGuid());

            homespace.Run();
            Assert.DoesNotThrow(DispatcherUtil.DoEvents);
            Assert.AreEqual(2, homespace.Connectors.Count());
        }

        [Test]
        public void LiveChartsBasicLineChartCreationTest()
        {
            var homespace = Model.CurrentWorkspace as HomeWorkspaceModel;
            Assert.NotNull(homespace, "The current workspace is not a HomeWorkspaceModel");

            var chart = new BasicLineChartNodeModel();
            Model.AddNodeToCurrentWorkspace(chart, true);
            homespace.Run();
            Assert.DoesNotThrow(DispatcherUtil.DoEvents);
            Assert.AreEqual(1, homespace.Nodes.Count());

            string codeA = "[\"January\", \"February\", \"March\"];";
            string codeB = "[[2,53,14,45,6],[22,41,45,61,21],[34,51,34,65,2]];";

            //This create a new Code Block node and update the content
            var codeBlockNodeA = CreateCodeBlockNode();
            var codeBlockNodeB = CreateCodeBlockNode();
            UpdateCodeBlockNodeContent(codeBlockNodeA, @codeA);
            UpdateCodeBlockNodeContent(codeBlockNodeB, @codeB);

            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeA.OutPorts.FirstOrDefault(), chart.InPorts[0], Guid.NewGuid());
            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeB.OutPorts.FirstOrDefault(), chart.InPorts[1], Guid.NewGuid());

            homespace.Run();
            Assert.DoesNotThrow(DispatcherUtil.DoEvents);
            Assert.AreEqual(2, homespace.Connectors.Count());
        }

        [Test]
        public void LiveChartsHeatSeriesCreationTest()
        {
            var homespace = Model.CurrentWorkspace as HomeWorkspaceModel;
            Assert.NotNull(homespace, "The current workspace is not a HomeWorkspaceModel");

            var chart = new HeatSeriesNodeModel();
            Model.AddNodeToCurrentWorkspace(chart, true);
            homespace.Run();
            Assert.DoesNotThrow(DispatcherUtil.DoEvents);
            Assert.AreEqual(1, homespace.Nodes.Count());

            string codeA = "[\"Item 1\",\"Item 2\",\"Item 3\",\"Item 4\",\"Item 5\",\"Item 6\"];";
            string codeB = "[\"January\",\"February\",\"March\",\"April\",\"May\",\"June\"];";
            string codeC = "[[1,2,3,4,2,1],[-12,12,4,-61,45,88],[4,4,5,5,16,-6],[-74,37,83,-262,54,44],[232,133,444,323,414,231],[332,122,98,89,89,78]];";

            //This create a new Code Block node and update the content
            var codeBlockNodeA = CreateCodeBlockNode();
            var codeBlockNodeB = CreateCodeBlockNode();
            var codeBlockNodeC = CreateCodeBlockNode();
            UpdateCodeBlockNodeContent(codeBlockNodeA, @codeA);
            UpdateCodeBlockNodeContent(codeBlockNodeB, @codeB);
            UpdateCodeBlockNodeContent(codeBlockNodeC, @codeC);

            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeA.OutPorts.FirstOrDefault(), chart.InPorts[0], Guid.NewGuid());
            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeB.OutPorts.FirstOrDefault(), chart.InPorts[1], Guid.NewGuid());
            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeC.OutPorts.FirstOrDefault(), chart.InPorts[2], Guid.NewGuid());

            homespace.Run();
            Assert.DoesNotThrow(DispatcherUtil.DoEvents);
            Assert.AreEqual(3, homespace.Connectors.Count());
        }

        [Test]
        public void LiveChartsPieChartCreationTest()
        {
            var homespace = Model.CurrentWorkspace as HomeWorkspaceModel;
            Assert.NotNull(homespace, "The current workspace is not a HomeWorkspaceModel");

            var chart = new PieChartNodeModel();
            Model.AddNodeToCurrentWorkspace(chart, true);
            homespace.Run();
            Assert.DoesNotThrow(DispatcherUtil.DoEvents);
            Assert.AreEqual(1, homespace.Nodes.Count());

            string codeA = "[\"January\", \"February\", \"March\"];";
            string codeB = "[4, 12, 34];";

            //This create a new Code Block node and update the content
            var codeBlockNodeA = CreateCodeBlockNode();
            var codeBlockNodeB = CreateCodeBlockNode();
            UpdateCodeBlockNodeContent(codeBlockNodeA, @codeA);
            UpdateCodeBlockNodeContent(codeBlockNodeB, @codeB);

            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeA.OutPorts.FirstOrDefault(), chart.InPorts[0], Guid.NewGuid());
            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeB.OutPorts.FirstOrDefault(), chart.InPorts[1], Guid.NewGuid());

            homespace.Run();
            Assert.DoesNotThrow(DispatcherUtil.DoEvents);
            Assert.AreEqual(2, homespace.Connectors.Count());
        }

        [Test]
        public void LiveChartsScatterPlotCreationTest()
        {
            var homespace = Model.CurrentWorkspace as HomeWorkspaceModel;
            Assert.NotNull(homespace, "The current workspace is not a HomeWorkspaceModel");

            var chart = new ScatterPlotNodeModel();
            Model.AddNodeToCurrentWorkspace(chart, true);
            homespace.Run();
            Assert.DoesNotThrow(DispatcherUtil.DoEvents);
            Assert.AreEqual(1, homespace.Nodes.Count());

            string codeA = "[\"January\", \"February\", \"March\"];";
            string codeB = "[[70,20,10],[12,24,44],[35,5,16]];";
            string codeC = "[[30,50,40],[60,40,-13],[62,-3,28]];";

            //This create a new Code Block node and update the content
            var codeBlockNodeA = CreateCodeBlockNode();
            var codeBlockNodeB = CreateCodeBlockNode();
            var codeBlockNodeC = CreateCodeBlockNode();
            UpdateCodeBlockNodeContent(codeBlockNodeA, @codeA);
            UpdateCodeBlockNodeContent(codeBlockNodeB, @codeB);
            UpdateCodeBlockNodeContent(codeBlockNodeC, @codeC);

            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeA.OutPorts.FirstOrDefault(), chart.InPorts[0], Guid.NewGuid());
            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeB.OutPorts.FirstOrDefault(), chart.InPorts[1], Guid.NewGuid());
            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeC.OutPorts.FirstOrDefault(), chart.InPorts[2], Guid.NewGuid());

            homespace.Run();
            Assert.DoesNotThrow(DispatcherUtil.DoEvents);
            Assert.AreEqual(3, homespace.Connectors.Count());
        }

        [Test]
        public void LiveChartsXYLineChartCreationTest()
        {
            var homespace = Model.CurrentWorkspace as HomeWorkspaceModel;
            Assert.NotNull(homespace, "The current workspace is not a HomeWorkspaceModel");

            var chart = new XYLineChartNodeModel();
            Model.AddNodeToCurrentWorkspace(chart, true);
            homespace.Run();
            Assert.DoesNotThrow(DispatcherUtil.DoEvents);
            Assert.AreEqual(1, homespace.Nodes.Count());

            string codeA = "[\"January\", \"February\", \"March\"];";
            string codeB = "[[5,20,50],[12,24,44],[15,25,56]];";
            string codeC = "[[30,50,140],[160,40,43],[54,25,54]];";

            //This create a new Code Block node and update the content
            var codeBlockNodeA = CreateCodeBlockNode();
            var codeBlockNodeB = CreateCodeBlockNode();
            var codeBlockNodeC = CreateCodeBlockNode();
            UpdateCodeBlockNodeContent(codeBlockNodeA, @codeA);
            UpdateCodeBlockNodeContent(codeBlockNodeB, @codeB);
            UpdateCodeBlockNodeContent(codeBlockNodeC, @codeC);

            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeA.OutPorts.FirstOrDefault(), chart.InPorts[0], Guid.NewGuid());
            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeB.OutPorts.FirstOrDefault(), chart.InPorts[1], Guid.NewGuid());
            new Dynamo.Graph.Connectors.ConnectorModel(codeBlockNodeC.OutPorts.FirstOrDefault(), chart.InPorts[2], Guid.NewGuid());

            homespace.Run();
            Assert.DoesNotThrow(DispatcherUtil.DoEvents);
            Assert.AreEqual(3, homespace.Connectors.Count());
        }
    }
}
