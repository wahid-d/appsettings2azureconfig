using System;
using Microsoft.AspNetCore.Components;

namespace settings2config.Components
{
    public partial class Clipboard
    {
        [Parameter]
        public string Id { get; set; } 

        public event Action OnClick;
        
        protected void OnClicked()
        {
            OnClick?.Invoke();
        }
    }
}