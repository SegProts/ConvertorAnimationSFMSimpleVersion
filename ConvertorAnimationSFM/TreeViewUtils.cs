using System.Collections.Generic;
using System.Windows.Forms;

public class TreeViewUtils
{
    /// <summary>
    /// This static utiltiy method flattens all the nodes in a tree view using
    /// a queue based breath first search rather than the overhead
    /// of recursive method calls.
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    public static List<TreeNode> FlattenBreath(TreeView tree)
    {
        List<TreeNode> nodes = new List<TreeNode>();

        Queue<TreeNode> queue = new Queue<TreeNode>();

        //
        // Bang all the top nodes into the queue.
        //
        foreach (TreeNode top in tree.Nodes)
        {
            queue.Enqueue(top);
        }

        while (queue.Count > 0)
        {
            TreeNode node = queue.Dequeue();
            if (node != null)
            {
                //
                // Add the node to the list of nodes.
                //
                nodes.Add(node);

                if (node.Nodes != null && node.Nodes.Count > 0)
                {
                    //
                    // Enqueue the child nodes.
                    //
                    foreach (TreeNode child in node.Nodes)
                    {
                        queue.Enqueue(child);
                    }
                }
            }
        }
        return nodes;
    }

    /// <summary>
    /// This static utiltiy method flattens all the nodes in a tree view using
    /// a stack based depth first search rather than the overhead
    /// of recursive method calls.
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    public static List<TreeNode> FlattenDepth(TreeView tree)
    {
        List<TreeNode> nodes = new List<TreeNode>();

        Stack<TreeNode> stack = new Stack<TreeNode>();

        //
        // Bang all the top nodes into the queue.
        //
        foreach (TreeNode top in tree.Nodes)
        {
            stack.Push(top);
        }

        while (stack.Count > 0)
        {
            TreeNode node = stack.Pop();
            if (node != null)
            {

                //
                // Add the node to the list of nodes.
                //
                nodes.Add(node);

                if (node.Nodes != null && node.Nodes.Count > 0)
                {
                    //
                    // Enqueue the child nodes.
                    //
                    foreach (TreeNode child in node.Nodes)
                    {
                        stack.Push(child);
                    }
                }
            }
        }
        return nodes;
    }
}