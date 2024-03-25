
var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationHub").withAutomaticReconnect().build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;
 
connection.on("LoadNotifyByUserId", function (abc) {
    var msg = '123'; /*message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");*/
    msg = msg + abc;
    //var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = msg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    //connection.invoke("SendAll", message).catch(function (err) {
    //    return console.error(err.toString());
    //});
    var rqData = {
        user : user,
        message : message
    };

    CommonJs.CustAjaxCall(rqData, "POST", "/TestNotify/SendToSpecificUser", "text");

    event.preventDefault();
});