
var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationHub").withAutomaticReconnect().build();

connection.on("LoadNotifyByUserId", function (userId) {
    //Todo: hàm này được gọi từ server, thực hiện việc call ajax để load ra notification của người dùng
    $.ajax({
        async: false,
        data: {},
        method: "POST",
        url: "/Home/PushNotification?userId=" + userId
    }).done(function (data) {
        // If successful
        let total = parseInt($("#totalUnreadNotification").text());
        $("#totalUnreadNotification").text(total + 1);
        $("#headerNotification").prepend(data);
        Notification.ReadNotify();
    }).fail(function (jqXHR, textStatus, errorThrown) {
        // If fail
        alert(textStatus + ': ' + errorThrown);
        if (typeof (errCallback) === "function") {
            errCallback();
        }
    });
});


connection.start().then(function () {
    //Todo: nothing
}).catch(function (err) {
    return console.error(err.toString());
});

var Notification = {
    ReadNotify: function () {
        $("#headerNotification .dropdown-item.notRead").mouseover(function () {
            let self = $(this);
            let id = self.data("id");
            $.ajax({
                async: false,
                data: {},
                method: "POST",
                url: "/Home/ReadNotification?id=" + id
            }).done(function (data) {
                // If successful
                let total = parseInt($("#totalUnreadNotification").text());
                $("#totalUnreadNotification").text(total - 1);
                self.removeClass("notRead");
                self.attr("style", "cursor: pointer;");
                self.unbind('mouseover');
            }).fail(function (jqXHR, textStatus, errorThrown) {
                // If fail
                alert(textStatus + ': ' + errorThrown);
                if (typeof (errCallback) === "function") {
                    errCallback();
                }
            });
        });
    }
}
