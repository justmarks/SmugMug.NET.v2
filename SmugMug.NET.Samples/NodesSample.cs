using SmugMug;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmugMug.NET.Samples
{
    class NodesSample
    {
        public static async Task WorkingWithNodes(SmugMugAPI api)
        {
            //Get access to the user you want to enumerate albums for
            User user = await api.GetUser("cmac");
            Console.WriteLine(user.Name);

            Node rootNode = await api.GetRootNode(user);
            Console.WriteLine(rootNode);

            //Get the children of the root node
            List<Node> childNodes = await api.GetChildNodes(rootNode);
            Console.WriteLine("The first node '{0}' is a {1}", childNodes[0].Name, childNodes[0].Type);

            //Get a specific node, "XWx8t"
            Node node = await api.GetNode("XWx8t");
            Console.WriteLine("Node '{0}' is a {1}", node.Name, node.Type);
        }

        public static async Task CreatingNodes(SmugMugAPI api)
        {
            //Get access to the logged in user
            User user = await api.GetAuthenticatedUser();
            Console.WriteLine("{0} is currently authenticated", user.Name);

            //Get the root node ID
            string defaultNodeID = api.GetDefaultNodeID(user);

            //Create a new folder node at the root
            Node folderNode = await api.CreateNode(NodeType.Folder, "TestFolderNode", defaultNodeID);

            //Create a new album node in that folder
            Dictionary<string, string> arguments = new Dictionary<string, string>() { { "Description", "test description" } };
            Node albumNode = await api.CreateNode(NodeType.Album, "TestAlbumNode", folderNode.NodeID, arguments);

            //Delete the newly created nodes
            await api.DeleteNode(folderNode);
        }
    }
}