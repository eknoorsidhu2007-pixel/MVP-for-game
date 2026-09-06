using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RoleAssignment : NetworkBehaviour
{
    public static RoleAssignment Instance;

    // netId -> requested role
    private Dictionary<uint, RoleType> roleRequests = new Dictionary<uint, RoleType>();
    // Final assignments
    private Dictionary<uint, RoleType> finalAssignments = new Dictionary<uint, RoleType>();

    private void Awake()
    {
        Instance = this;
    }

    [Command(requiresAuthority = false)]
    public void CmdRequestRole(uint playerNetId, RoleType role)
    {
        roleRequests[playerNetId] = role;
    }

    [Server]
    public void ResolveRoles()
    {
        finalAssignments.Clear();
        var players = new List<uint>(roleRequests.Keys);
        
        // Group by role
        var roleGroups = roleRequests.GroupBy(kvp => kvp.Value)
                                     .ToDictionary(g => g.Key, g => g.Select(x => x.Key).ToList());

        foreach (var group in roleGroups)
        {
            RoleType role = group.Key;
            List<uint> contenders = group.Value;

            if (contenders.Count == 1)
            {
                finalAssignments[contenders[0]] = role;
            }
            else
            {
                // Coin flip for each conflict
                // Winner takes the role, losers get None (must pick again)
                int winnerIndex = Random.Range(0, contenders.Count);
                for (int i = 0; i < contenders.Count; i++)
                {
                    if (i == winnerIndex)
                        finalAssignments[contenders[i]] = role;
                    else
                        finalAssignments[contenders[i]] = RoleType.None;
                }
            }
        }

        // Sync to all players
        foreach (var kvp in finalAssignments)
        {
            RpcAssignRole(kvp.Key, kvp.Value);
        }
    }

    [ClientRpc]
    private void RpcAssignRole(uint netId, RoleType role)
    {
        // Find player and assign
        var players = FindObjectsOfType<PlayerRole>();
        foreach (var p in players)
        {
            if (p.netId == netId)
            {
                p.assignedRole = role;
                p.roleLocked = role != RoleType.None;
                break;
            }
        }
    }

    [Server]
    public void ConfirmAndStart()
    {
        // Ensure all 4 roles are filled
        var assignedRoles = finalAssignments.Values.ToList();
        if (assignedRoles.Contains(RoleType.Driver) && 
            assignedRoles.Contains(RoleType.Navigator) &&
            assignedRoles.Contains(RoleType.Gunner) &&
            assignedRoles.Contains(RoleType.Engineer))
        {
            GameManager.Instance.StartRun();
        }
    }
}