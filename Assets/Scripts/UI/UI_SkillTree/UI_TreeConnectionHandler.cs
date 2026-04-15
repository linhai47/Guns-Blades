using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
[Serializable]
public class UI_TreeConnectDetails
{
    public UI_TreeConnectionHandler childNode;
    public NodeDirectionType direction;
    [Range(100f, 350f)] public float length;
    [Range(-50f, 50f)] public float rotation;
}

public class UI_TreeConnectionHandler : MonoBehaviour
{
    private RectTransform rect => GetComponent<RectTransform>();
    [SerializeField] private UI_TreeConnectDetails[] connectionsDetails;
    [SerializeField] private UI_TreeConnection[] connections;

    private Image connectionImage;
    private Color originalColor;
    private void Awake()
    {
        if (connectionImage != null)
            originalColor = connectionImage.color;
    }

    public UI_TreeNode[] GetChildNodes()
    {
        List<UI_TreeNode> childrenToReturn = new List<UI_TreeNode>();

        foreach (var node in connectionsDetails)
        {
            if (node.childNode != null)
            {
                childrenToReturn.Add(node.childNode.GetComponent<UI_TreeNode>());
            }
        }
        return childrenToReturn.ToArray();

    }
    private void UpdateConnections()
    {
   
        for (int i = 0; i < connectionsDetails.Length; i++)
        {
            var detail = connectionsDetails[i];
            var connection = connections[i];
        
            Vector2 targetPosition = connection.GetConnectionPoint(rect);
            Image connectionImage = connection.GetConnectionImage();

            connection.DirectConnection(detail.direction, detail.length, detail.rotation);
            if (detail.childNode == null) continue;

            detail.childNode?.SetPosition(targetPosition);
            detail.childNode?.SetConnectionImage(connectionImage);
            detail.childNode.transform.SetAsLastSibling();

        }
    }

    public void UpdateAllConnections()
    {
        UpdateConnections();
        foreach (var node in connectionsDetails)
        {
         
            if (node.childNode == null) continue;
            node.childNode.UpdateAllConnections();
        }
    }

    public void UnlockConnectionImage(bool unlocked)
    {
        if (connectionImage == null)
        {
            return;
        }

        connectionImage.color = unlocked ? Color.white : originalColor;
    }

    public void SetConnectionImage(Image image) => connectionImage = image;
    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;
    private void OnValidate()
    {
        if (connectionsDetails.Length <= 0)
        {
            return;
        }


        if (connectionsDetails.Length != connections.Length)
        {
            Debug.Log("Amount of details should ve same as amount of connections. -" + gameObject.name);
            return;
        }

        UpdateConnections();
    }

}
