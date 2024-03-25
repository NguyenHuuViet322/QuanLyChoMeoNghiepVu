"use strict"
var AdminConfig = {
    LoadInit: function () {
        let someData = "";
        let dataType = "html";
        let url = "/users/Index";
        let ssCallback = function (data) {
            $("#admin_contentArea").html(data)
        }
        CustAjaxCall("", someData, url, dataType, ssCallback);
    }
    
};