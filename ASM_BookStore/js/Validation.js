function check_pass() {
    var p = document.getElementById("password").value;
    var c = document.getElementById("confirmPassword").value;
    if (p === c) {
        document.getElementById("password").style.borderBottomColor = "lightgrey";
        document.getElementById("confirmPassword").style.borderBottomColor = "lightgrey";
    }
    else {
        document.getElementById("password").style.borderBottomColor = "red";
        document.getElementById("confirmPassword").style.borderBottomColor = "red";
    }
}