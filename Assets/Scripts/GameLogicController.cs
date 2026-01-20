using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLogicController
{
    private List<string> _busyPeopleNames = new List<string>();

    public struct ScenePlan
    {
        public List<SpawnInstruction> Instructions;
        public List<InventoryItem> StartingInventory;
    }

    public struct SpawnInstruction
    {
        public string PersonName;
        public string RoleName;
        public CharacterRole TargetRole;
        public string Gender;
        public bool IsDoctor;
        public bool ForceLying;
    }
    public string PrepareAndProcessAction(string target, string rawInput, string mode, List<InventoryItem> inventory)
    {
        string processedText = rawInput;

        if (rawInput.StartsWith("Apply "))
        {
            string itemName = rawInput.Replace("Apply ", "").Split(new string[] { " to" }, StringSplitOptions.None)[0];
            var item = inventory?.Find(i => i.name == itemName);
            if (item != null && item.count > 0)
            {
                item.count--; 
            }
        }

        if (mode == "freeform" && !processedText.StartsWith("["))
        {
            processedText = $"[{target}] {processedText}";
        }

        return processedText;
    }

    public ScenePlan PrepareFullScene(InitialSceneData data)
    {
        _busyPeopleNames.Clear();
        var plan = new ScenePlan
        {
            Instructions = new List<SpawnInstruction>(),
            StartingInventory = data.inventory 
        };

        if (data.hasDoctor)
        {
            plan.Instructions.Add(new SpawnInstruction
            {
                IsDoctor = true,
                RoleName = "Doctor",
                TargetRole = CharacterRole.Doctor,
                ForceLying = false
            });
        }

        plan.Instructions.Add(new SpawnInstruction
        {
            Gender = data.patientGender,
            RoleName = "Patient",
            TargetRole = CharacterRole.Patient,
            ForceLying = true
        });

        if (data.visitors != null)
        {
            foreach (var visitor in data.visitors)
            {
                plan.Instructions.Add(new SpawnInstruction
                {
                    Gender = visitor.gender,
                    RoleName = visitor.role,
                    TargetRole = CharacterRole.Any,
                    ForceLying = false
                });
            }
        }
        return plan;
    }
}

