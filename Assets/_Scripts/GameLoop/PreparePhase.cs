using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PreparePhase : MonoBehaviour
{
    public static PreparePhase Instance;

    [Header("UI")]
    public GameObject preparePanel;
    public Button driverBtn;
    public Button navigatorBtn;
    public Button gunnerBtn;
    public Button engineerBtn;
    public Button confirmBtn;
    public Text statusText;

    private RoleType selectedRole = RoleType.None;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        driverBtn.onClick.AddListener(() => SelectRole(RoleType.Driver));
        navigatorBtn.onClick.AddListener(() => SelectRole(RoleType.Navigator));
        gunnerBtn.onClick.AddListener(() => SelectRole(RoleType.Gunner));
        engineerBtn.onClick.AddListener(() => SelectRole(RoleType.Engineer));
        confirmBtn.onClick.AddListener(ConfirmRole);

        GameManager.OnGameStateChanged += OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
        preparePanel.SetActive(state == GameState.Prepare);
    }

    private void SelectRole(RoleType role)
    {
        selectedRole = role;
        statusText.text = $"Selected: {role}";
        
        var localPlayer = NetworkClient.localPlayer?.GetComponent<PlayerRole>();
        if (localPlayer != null)
        {
            RoleAssignment.Instance?.CmdRequestRole(localPlayer.netId, role);
        }
    }

    private void ConfirmRole()
    {
        if (NetworkServer.active)
        {
            RoleAssignment.Instance?.ResolveRoles();
            RoleAssignment.Instance?.ConfirmAndStart();
        }
    }
}