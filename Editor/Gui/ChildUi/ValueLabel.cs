﻿using System;
using System.Numerics;
using ImGuiNET;
using T3.Core.Operator.Slots;
using T3.Core.Utils;
using T3.Editor.Gui.Interaction;
using T3.Editor.Gui.Styling;
using T3.Editor.Gui.UiHelpers;

namespace T3.Editor.Gui.ChildUi
{
    public static class ValueLabel
    {
        public static bool Draw(ImDrawListPtr drawList, ImRect screenRect, Vector2 alignment, InputSlot<float> inputSlot)
        {
            var modified = false;
            var value = (double)inputSlot.TypedDefaultValue.Value;
            var valueText = $"{value:G5}";
            var hashCode = inputSlot.GetHashCode();
            ImGui.PushID(hashCode);
            
            var editingUnlocked = ImGui.GetIO().KeyCtrl || _jogDialValue != null;
            var highlight = editingUnlocked;
            // InputGizmo
            {
                var labelSize = screenRect.GetSize() / 4 + Vector2.One * 4;
                var space = screenRect.GetSize() - labelSize;
                var position = screenRect.Min + space * alignment - Vector2.One * 2;
                ImGui.SetCursorScreenPos(position);
                if (editingUnlocked)
                {
                    ImGui.InvisibleButton("button", labelSize);
                    
                    double value2 = inputSlot.TypedInputValue.Value;
                    if (ImGui.IsItemActivated() && ImGui.GetIO().KeyCtrl)
                    {
                        _jogDailCenter = ImGui.GetIO().MousePos;
                        _jogDialValue = inputSlot;
                        drawList.AddRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), Color.White);
                    }
                    
                    if (ImGui.IsItemActive() || !ImGui.IsAnyItemActive())
                    {
                        drawList.AddRectFilled(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), _hoverRegionColor);
                    }
                    else
                    {
                        highlight = false;
                    }

                    if (_jogDialValue == inputSlot)
                    {
                        if (ImGui.IsItemActive())
                        {
                            modified = JogDialOverlay.Draw(ref value, ImGui.IsItemActivated(), _jogDailCenter, Double.NegativeInfinity, Double.PositiveInfinity,
                                                           0.01f);
                            if (modified)
                            {
                                inputSlot.TypedInputValue.Value = (float)value;
                                inputSlot.Input.IsDefault = false;
                                inputSlot.DirtyFlag.Invalidate();
                            }
                        }
                        else
                        {
                            _jogDialValue = null;
                        }
                    }
                }
            }
            
            // Draw aligned label
            {
                ImGui.PushFont(Fonts.FontSmall);
                var labelSize = ImGui.CalcTextSize(valueText);
                var space = screenRect.GetSize() - labelSize;
                var position = screenRect.Min + space * alignment;
                drawList.AddText(MathUtils.Floor(position), highlight ? T3Style.Colors.ValueLabelHover : T3Style.Colors.ValueLabel, valueText);
                ImGui.PopFont();
            }            
            ImGui.PopID();
            return modified;
        }

        private static readonly Color _hoverRegionColor = new Color(0, 0, 0, 0.2f);
        private static Vector2 _jogDailCenter;
        private static InputSlot<float> _jogDialValue;        
    }
}