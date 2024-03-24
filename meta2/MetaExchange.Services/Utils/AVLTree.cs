using MetaExchangeAPIv2.Models;

public class AVLTreeNode
{
    public Order Order { get; set; }
    public int Height { get; set; }
    public AVLTreeNode Left { get; set; }
    public AVLTreeNode Right { get; set; }
}

public class AVLTree
{
    private AVLTreeNode root;

    // Insert an order into the AVL tree
    public void Insert(Order order)
    {
        root = Insert(root, order);
    }

    private AVLTreeNode Insert(AVLTreeNode node, Order order)
    {
        if (node == null)
        {
            return new AVLTreeNode { Order = order, Height = 1 };
        }

        // Compare orders based on price and insert accordingly
        if (order.Price < node.Order.Price)
        {
            node.Left = Insert(node.Left, order);
        }
        else if (order.Price > node.Order.Price)
        {
            node.Right = Insert(node.Right, order);
        }
        else
        {
            // Handle equal prices (possibly by comparing timestamps or other criteria)
            // For simplicity, we'll assume orders with equal prices are treated equally
            node.Left = Insert(node.Left, order);
        }

        // Update height and balance factor
        node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));
        int balance = BalanceFactor(node);

        // Perform rotation if necessary to maintain AVL balance
        if (balance > 1 && order.Price < node.Left.Order.Price)
        {
            return RightRotate(node);
        }
        if (balance < -1 && order.Price > node.Right.Order.Price)
        {
            return LeftRotate(node);
        }
        if (balance > 1 && order.Price > node.Left.Order.Price)
        {
            node.Left = LeftRotate(node.Left);
            return RightRotate(node);
        }
        if (balance < -1 && order.Price < node.Right.Order.Price)
        {
            node.Right = RightRotate(node.Right);
            return LeftRotate(node);
        }

        return node;
    }

    // Get the height of a node
    private int Height(AVLTreeNode node)
    {
        return node == null ? 0 : node.Height;
    }

    // Get the balance factor of a node
    private int BalanceFactor(AVLTreeNode node)
    {
        return node == null ? 0 : Height(node.Left) - Height(node.Right);
    }

    // Perform a right rotation
    private AVLTreeNode RightRotate(AVLTreeNode y)
    {
        AVLTreeNode x = y.Left;
        AVLTreeNode T2 = x.Right;

        x.Right = y;
        y.Left = T2;

        y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
        x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;

        return x;
    }

    // Perform a left rotation
    private AVLTreeNode LeftRotate(AVLTreeNode x)
    {
        AVLTreeNode y = x.Right;
        AVLTreeNode T2 = y.Left;

        y.Left = x;
        x.Right = T2;

        x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
        y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;

        return y;
    }

    // Traverse the AVL tree in-order
    public List<Order> InOrderTraversal()
    {
        List<Order> result = new List<Order>();
        InOrderTraversal(root, result);
        return result;
    }

    private void InOrderTraversal(AVLTreeNode node, List<Order> result)
    {
        if (node != null)
        {
            InOrderTraversal(node.Left, result);
            result.Add(node.Order);
            InOrderTraversal(node.Right, result);
        }
    }
}