using System.Reflection.Metadata;
using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace settings2config.Components
{
    public partial class Dropdown : ComponentBase
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public List<string> Items { get; set; }
        
        public event Action<string> OnSelected;

        protected override void OnInitialized()
        {
            Items = new List<string>();
        }
        
        protected void Selected(string item)
        {
            OnSelected?.Invoke(item);
        }
    }
}