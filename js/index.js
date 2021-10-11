(function()
{
    "using strict";

    $(document).ready(function ()
    {
        $("#alert").hide();
        fetch("/themes/themelist.json")
        .then(response => response.json())
        .then(themes => 
        {
            // console.log(themes)
            $.each(themes, function(index, item){

                var themeFile = `/themes/${item}.json`;
                fetch(themeFile)
                .then(response => response.json())
                .then(theme => 
                {
                    var themeName = Object.keys(themes).find(key => themes[key] == item);
                    monaco.editor.defineTheme(themeName, theme);
                });
            });
        });
    });  
})();

window.appsettings = {
    toAzureConfig: function(appsettings){

        try{
    
            var object = JSON.parse(appsettings);
            var processed = parseObject(object);
    
            var data = [];
            for(var key in processed){
                data.push({
                    name: key,
                    value: processed[key],
                    slotSetting: false
                });
            }
    
            return JSON.stringify(data, null, '    ');
    
        }catch(e){
            alert(e)
        }
    
        function parseObject(object)
        {
            var inner = {};
            for(var key in object){
    
                if(typeof object[key] === 'object'){
                    var parsed = parseObject(object[key]);
                    for(var k in parsed){
                        inner[`${key}:${k}`] = parsed[k];
                    }
                }else{
                    inner[key] = object[key];
                }
            }
            return inner;
        }
    }
}

window.alerts = {
    show: function (){
        $("#alert").removeClass('d-none');
        $("#alert").hide();
        $("#alert").fadeTo(2000, 500).slideUp(250, function() {
            $("#alert").slideUp(1000);
        });
    }
}

function validateJson(jsonString){
    if(/^[\],:{}\s]*$/.test(jsonString.replace(/\\["\\\/bfnrtu]/g, '@').
    replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').
    replace(/(?:^|:|,)(?:\s*\[)+/g, ''))){
        return true;
    }else{
        return false;
    }
}