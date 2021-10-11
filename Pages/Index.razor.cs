using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorMonaco;
using settings2config.Components;
using settings2config.Services;

namespace settings2config.Pages
{
    public partial class Index
    {
        public MonacoEditor InputEditor { get; set; }
        public MonacoEditor OutputEditor { get; set; }
        public Dropdown FontsizeDropdown { get; set; }
        public Dropdown ThemesDropdown { get; set; }
        public Clipboard OutputClipboard { get; set; }
        public List<string> Fontsizes { get; set; }
        public List<string> Themes { get; set; }
        [Inject]
        protected HttpClient Client { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public ClipboardService ClipboardService { get; set; }
        
        
        static DateTime firstCall = DateTime.Now.AddSeconds(-1);

        protected override async Task OnInitializedAsync()
        {
            var themes = await Client.GetFromJsonAsync<Dictionary<string,string>>("themes/themelist.json");
            if(themes is not null)
            {
                Themes = themes.Keys.ToList();
            }

            Fontsizes = new List<string> { "12", "14", "18", "22", "24", "28" };

            FontsizeDropdown.OnSelected += FontsizeSelected;
            ThemesDropdown.OnSelected += ThemeSelected;
            OutputClipboard.OnClick += OnClipboardClicked;

            var jsonObj = await Client.GetFromJsonAsync<object>("sample.json");
            string sampleJson = System.Text.Json.JsonSerializer.Serialize(jsonObj, new JsonSerializerOptions() { WriteIndented = true });
            
            await InputEditor.SetValue(sampleJson);
            await OnPasted(new PasteEvent());
            StateHasChanged();
        }

        private async void OnClipboardClicked()
        {
            await ClipboardService.WriteTextAsync(await OutputEditor.GetValue());
            await JS.InvokeVoidAsync("alerts.show");
        }

        private async Task OnPasted(PasteEvent e)
        {
            var elapsingSeconds = (DateTime.Now - firstCall).TotalSeconds;
            firstCall = DateTime.Now;
            if(elapsingSeconds > 1)
            {
                var value = "";
                value = await JS.InvokeAsync<string>("appsettings.toAzureConfig", await InputEditor.GetValue());
                await OutputEditor.SetValue(value);   
            }
        }

        private void ThemeSelected(string theme)
        {
            InputEditor.UpdateOptions(new GlobalEditorOptions() { Theme = theme });
            OutputEditor.UpdateOptions(new GlobalEditorOptions() { Theme = theme });
        }

        private void FontsizeSelected(string fontsize)
        {
            int size = 12;
            int.TryParse(fontsize, out size);
            InputEditor.UpdateOptions(new GlobalEditorOptions() { FontSize = size });
            OutputEditor.UpdateOptions(new GlobalEditorOptions() { FontSize = size });
        }

        public StandaloneEditorConstructionOptions OutputOptions(MonacoEditor editor)
        {
            var outputOptions = DefaultOptions();
            outputOptions.Value = "";
            outputOptions.ReadOnly = true;
            return outputOptions;
        }

        public StandaloneEditorConstructionOptions InputOptions(MonacoEditor editor)
        {

            var inputOptions = DefaultOptions();
            inputOptions.Value = "{}";
            return inputOptions;
        }

        private StandaloneEditorConstructionOptions DefaultOptions() =>
        new  StandaloneEditorConstructionOptions
        {
            Language = "json",
            Theme = "cobalt",
            FontSize = 12,
            AutomaticLayout = true,
            FormatOnPaste = true,
            FormatOnType = true,
            AutoIndent = true,
            CodeLens = true,
            ScrollBeyondLastLine = false,
            ScrollBeyondLastColumn = 0,
            Padding = new EditorPaddingOptions() { Top = 15, Bottom = 15 },
            Minimap = new EditorMinimapOptions() { Enabled = false },
            LineNumbersMinChars = 3,
            QuickSuggestions = new QuickSuggestionsOptions() { Other = false, Comments = false, Strings = false }
        };
    }
}