var SystemConfig = {
    Init: function () {
        $('#ValueType').change(function () {
            SystemConfig.ChangeDataType($(this).val());
            CommonJs.UpdateFormState(jQuery(document));
        });
    },
    ChangeDataType: function (selectedInput) {
        var listinput = ['IntVal', 'FloatVal', 'DateTimeVal', 'StringVal'];
        listinput.forEach(function (x) {
            if (x == selectedInput) {
                document.getElementById(x).readOnly = false;
            } else {
                document.getElementById(x).readOnly = false;
                document.getElementById(x).value = "";
                document.getElementById(x).readOnly = true;
                
            }            
        });
    }
};
