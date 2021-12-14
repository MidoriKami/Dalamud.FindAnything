﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Data;
using Dalamud.Interface;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;

namespace Dalamud.FindAnything
{
    public class TextureCache : IDisposable
    {
        private readonly UiBuilder uiBuilder;
        private readonly DataManager data;

        public IReadOnlyDictionary<uint, TextureWrap> MainCommandIcons { get; init; }
        public IReadOnlyDictionary<uint, TextureWrap> GeneralActionIcons { get; init; }
        public IReadOnlyDictionary<uint, TextureWrap> ContentTypeIcons { get; init; }

        public Dictionary<int, TextureWrap> MacroIcons { get; private set; }

        public TextureWrap AetheryteIcon { get; init; }
        public TextureWrap WikiIcon { get; init; }

        public TextureWrap PluginInstallerIcon { get; init; }
        public TextureWrap LogoutIcon { get; init; }

        private TextureCache(UiBuilder uiBuilder, DataManager data)
        {
            this.uiBuilder = uiBuilder;
            this.data = data;

            var mainCommands = new Dictionary<uint, TextureWrap>();
            foreach (var mainCommand in data.GetExcelSheet<MainCommand>()!)
            {
                mainCommands.Add(mainCommand.RowId, data!.GetImGuiTextureHqIcon((uint) mainCommand.Icon)!);
            }
            MainCommandIcons = mainCommands;

            var generalActions = new Dictionary<uint, TextureWrap>();
            foreach (var action in data.GetExcelSheet<GeneralAction>()!)
            {
                generalActions.Add(action.RowId, data!.GetImGuiTextureHqIcon((uint) action.Icon)!);
            }
            GeneralActionIcons = generalActions;
            
            var contentTypes = new Dictionary<uint, TextureWrap>();
            foreach (var cType in data.GetExcelSheet<ContentType>()!)
            {
                if (cType.Icon == 0)
                    continue;
                
                contentTypes.Add(cType.RowId, data!.GetImGuiTextureHqIcon((uint) cType.Icon)!);
            }
            ContentTypeIcons = contentTypes;

            AetheryteIcon = data.GetImGuiTextureHqIcon(066417)!;
            WikiIcon = data.GetImGuiTextureHqIcon(066404)!;
            PluginInstallerIcon = data.GetImGuiTextureHqIcon(066472)!;
            LogoutIcon = data.GetImGuiTextureHqIcon(066403)!;
            
            ReloadMacroIcons();
        }

        public void ReloadMacroIcons()
        {
            MacroIcons ??= new();
            foreach (var macroLink in FindAnythingPlugin.Configuration.MacroLinks)
            {
                if (MacroIcons.ContainsKey(macroLink.IconId))
                    continue;

                var tex = data.GetImGuiTextureHqIcon((uint) macroLink.IconId);
                
                if (tex != null)
                    MacroIcons[macroLink.IconId] = tex;
            }
        }

        public static TextureCache Load(UiBuilder uiBuilder, DataManager data) => new TextureCache(uiBuilder, data);

        public void Dispose()
        {
            foreach (var mainCommandIcon in MainCommandIcons)
            {
                mainCommandIcon.Value.Dispose();
            }

            AetheryteIcon.Dispose();
        }
    }
}