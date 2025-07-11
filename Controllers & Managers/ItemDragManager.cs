using UnityEngine;

public class ItemDragManager : MonoBehaviour
{
    public static ItemDragManager Instance;

    [Header("Components")] [SerializeField]
    private InventoryInputManager inputManager;

    [SerializeField] private DragVisualizer dragVisualizer;

    private IItemDragService dragService;
    private IItemOperationService operationService;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeServices();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeServices()
    {
        dragService = new ItemDragService();
        operationService = new ItemOperationService();

        inputManager.Initialize(dragService, operationService, dragVisualizer);
    }

    // Public API for external access
    public void HandleLeftClick(ItemContainerSlotUI slot)
    {
        inputManager.HandleLeftClick(slot);
    }

    public void HandleRightClick(ItemContainerSlotUI slot)
    {
        inputManager.HandleRightClick(slot);
    }

    public void HandleHoverEnter(ItemContainerSlotUI slot)
    {
        inputManager.HandleHoverEnter(slot);
    }

    public void HandleHoverExit(ItemContainerSlotUI slot)
    {
        inputManager.HandleHoverExit(slot);
    }
}