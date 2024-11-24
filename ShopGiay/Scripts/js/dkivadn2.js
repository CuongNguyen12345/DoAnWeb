var firstname = document.forms["signupForm"]["firstname"];
var lastname = document.forms["signupForm"]["lastname"];
var email = document.forms["signupForm"]["email"];
var password = document.forms["signupForm"]["password"];

var signup_error = document.querySelector(".signup_error");

firstname.addEventListener("textInput", fstnameVerify);
lastname.addEventListener("textInput", lstnameVerify);
email.addEventListener("textInput", emailVerify);
password.addEventListener("textInput", passwordVerify);

function signupValid(){
    if (firstname.value.length <= 2) {
        signup_error.style.display = "block";
        firstname.style.border = "1px solid red";
        signup_error.innerText = "vui lòng điền tên của bạn";
        firstname.focus();
        return false;
    }
    if (lastname.value.length <= 2) {
        signup_error.style.display = "block";
        lastname.style.border = "1px solid red";
        signup_error.innerText = "vui lòng điền họ của bạn";
        lastname.focus();
        return false;
    }
    if (email.value.length <= 8) {
        signup_error.style.display = "block";
        email.style.border = "1px solid red";
        signup_error.innerText = "vui lòng điền email hoặc sđt của bạn";
        email.focus();
        return false;
    }
    if (password.value.length <= 3) {
        signup_error.style.display = "block";
        password.style.border = "1px solid red";
        signup_error.innerText = "vui lòng điền mật khẩu của bạn";
        password.focus();
        return false;
    }
}
function fstnameVerify(){
    if (firstname.value.length > 1) {
        signup_error.style.display = "none";
        firstname.style.border = "1px solid #3498db";
        signup_error.innerText = "";
        return true;
    }
}
function lstnameVerify(){
    if (lastname.value.length > 1) {
        signup_error.style.display = "none";
        lastname.style.border = "1px solid #3498db";
        signup_error.innerText = "";
        return true;
    }
}
function emailVerify(){
    if (email.value.length > 7) {
        signup_error.style.display = "none";
        email.style.border = "1px solid #3498db";
        signup_error.innerText = "";
        return true;
    }
}
function passwordVerify(){
    if (password.value.length > 2) {
        signup_error.style.display = "none";
        password.style.border = "1px solid #3498db";
        signup_error.innerText = "";
        return true;
    }
}


var email = document.forms["loginForm"]["email"];
var password = document.forms["loginForm"]["password"];

var login_error = document.querySelector(".login_error");

email.addEventListener("textInput", emailVerify);
password.addEventListener("textInput", passwordVerify);

function loginValid(){
    
    if (email.value.length <= 8) {
        login_error.style.display = "block";
        email.style.border = "1px solid red";
        login_error.innerText = "vui lòng điền email hoặc sđt của bạn";
        email.focus();
        return false;
    }
    if (password.value.length <= 3) {
        login_error.style.display = "block";
        password.style.border = "1px solid red";
        login_error.innerText = "vui lòng điền mật khẩu của bạn";
        password.focus();
        return false;
    }
}
function emailVerify(){
    if (email.value.length > 7) {
        login_error.style.display = "none";
        email.style.border = "1px solid #3498db";
        login_error.innerText = "";
        return true;
    }
}
function passwordVerify(){
    if (password.value.length > 2) {
        login_error.style.display = "none";
        password.style.border = "1px solid #3498db";
        login_error.innerText = "";
        return true;
    }
}