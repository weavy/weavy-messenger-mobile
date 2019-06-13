namespace Messenger.Helpers {


    public static class ScriptHelper {

        public static string ScriptChecker = @"
/********************************************/
/* Check if script already injected (Droid) */
/********************************************/

if(typeof weavyAppScripts === 'undefined') {
    Native('injectScriptCallback', true);
}

";

        public static string ReconnectScript = @"
/********************************************/
/* Reconnect to weavy rtm or reload page      */
/********************************************/
try{ 
    if(!weavy.connection.status || weavy.connection.status() === 4) {
        weavy.connection.connect(); 
        weavy.messenger.refresh(); 
    }
} catch(e){}
";

        public static string UpdateBadgeScript = @"
/********************************************/
/* update badge                             */
/********************************************/
try{ 
    weavyAppScripts.badge.check();
} catch(e){}
";

        public static string Scripts = @"
var weavyAppScripts = weavyAppScripts || {};

/***************************************/
/* Debug script for test purposes only */
/***************************************/
weavyAppScripts.debug = (function(){    
    function test(){
        alert('debug!');  
    }
    /*test();*/
})();






/********************************************/
/* Register user for azure notification hub */
/********************************************/
weavyAppScripts.push = (function(){    
    function register(){
        var userId = $('body').data('user');        
        var userGuid = $('body').data('guid');
        
        if(userId != -1){ 
            Native('registerForNotificationsCallback', 'uid:' + userGuid);
        }
        
    }

    document.addEventListener('turbolinks:load', function (e) { 
        register();
    });

    register();
})();






/********************************************/
/* Handle badge changes                     */
/********************************************/
weavyAppScripts.badge = (function(){    
    weavy.realtime.on('badge.weavy', function(e, data){
        Native('badgeCallback', data.conversations);
    });

    var check = function(){
        
        $.ajax('/a/conversations/unread?followed=true').then(function(response){
            var count = response.data != null ? response.data.length : 0;
                   
            Native('badgeCallback', count);
        });        
    };
    
    check();

    return {
        check: check
    };
})();






/********************************************/
/* Handle theme                             */
/********************************************/
weavyAppScripts.theme = (function(){    
    function set(){
        var color = '#156B93';
                
        $.ajax('/a/theme').then(function(response){
            var colors = response.colors;
            var color = response.theme_color;
            if(!color && colors.length && colors.length > 7) {
                color = colors[7];
            }
                   
            Native('themeCallback', color);
        });        
    }

    document.addEventListener('turbolinks:load', function (e) { 
        set();
    });

    set();
})();






/********************************************/
/* Sign-in and sign-out                     */
/********************************************/
weavyAppScripts.authentication = (function(){  

    var signIn = function(){
        var signInForm = $('.controller-account.action-signin');

        if(signInForm[0] && $('#select-site').length == 0){
            var el = $('<a id=""select-site"" style=""display:block;text-align:center"">Sign in to another Messenger site</a>');
            el.addClass('changesite');
            $(signInForm).find('main form').last().after(el);
            $('a.changesite').on('click', function(){
                Native('changeSiteCallback', '');
                return false;
            });   
        }
    };

    var signOut = function(){
        $(document).on('click', 'a[href^=""/sign-out""]', function(){       
    	    Native('signOutCallback', '');
            return true;
        });
    };

    signIn();
    signOut();

})();



/********************************************/
/* Handle external links                    */
/********************************************/
weavyAppScripts.links = (function(){    
    function handle(){
       $(document).on('click', 'a[href^=http]', function (e) {
	        var url = $(this).attr('href');
            var target = $(this).attr('target');

	        if(url.indexOf(window.location.origin) == -1 || target == '_blank'){
			    e.preventDefault();	
                Native('handleLinksCallback', url);
	        }
        });

        $(document).on('click', 'a[href^=ms-]', function (e) {
	        e.preventDefault();		
            var url = $(this).attr('href');
            Native('handleLinksCallback', url);	
        });
    }

    handle();
})();







";
    }
}
