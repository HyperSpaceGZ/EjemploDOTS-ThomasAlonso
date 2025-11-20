using UnityEngine;

public class SelectionArea : MonoBehaviour
{
    
    [SerializeField] private RectTransform selectionAreaTransform;
    [SerializeField] private Canvas canvas;

    private void Start()
    {
        EntitySelector.Instance.OnSelectionAreaStart += UnitSelectionMG_OnSelectionStart;
        EntitySelector.Instance.OnSelectionAreaEnd += UnitSelectionMG_OnSelectionEnd;
        selectionAreaTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (selectionAreaTransform.gameObject.activeSelf)
        {
            UpdateVisual();
        }
    }

    private void UnitSelectionMG_OnSelectionStart(object sender, System.EventArgs e)
    {
        selectionAreaTransform.gameObject.SetActive(true);
        
        UpdateVisual();
    }

    private void UnitSelectionMG_OnSelectionEnd(object sender, System.EventArgs e)
    {
        selectionAreaTransform.gameObject.SetActive(false);
        
    }

    private void UpdateVisual()
    {
        Rect selectionAreaRect = EntitySelector.Instance.GetSelectionAreaRect();
        float canvasScale = canvas.transform.localScale.x;
        selectionAreaTransform.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y) / canvasScale;
        selectionAreaTransform.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height) / canvasScale;
    }
}
