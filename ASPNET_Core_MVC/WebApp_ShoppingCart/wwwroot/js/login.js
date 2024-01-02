let formLogin = document.getElementById("formLogin");
let inputUsername = document.getElementById("username");
let inputPassword = document.getElementById("password");
let hashPassword = document.getElementById("passhash");
let buttonLogin = document.getElementById("login");
let errorMsg = document.getElementById("errorMsg");

// Check if fields are filled before allowing login
formLogin.addEventListener("change", function (e) {
	buttonLogin.disabled = (inputUsername.value === "" || inputPassword.value === "") ? true : false;
})

// Hash password to be submitted as part of form to server
formLogin.addEventListener("submit", function (e) {
	let hash = CryptoJS.SHA1(inputPassword.value);
	hashPassword.value = hash.toString();
});

// Display error message styles if error message exists
if (errorMsg.innerHTML != "") {
	errorMsg.classList.add("alert");
	errorMsg.classList.add("alert-warning");
}