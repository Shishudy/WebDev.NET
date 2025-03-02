function login(inputs)
{
	if (document.getElementById("login-modal-div").style.display == "none")
		return ;
	document.getElementById("loading-modal-div").style.display = "flex";
	var options = {
		method: "POST",
		headers: {
			"Content-Type": "application/json",
		},
		body: JSON.stringify({ username : inputs[0].value, password : inputs[1].value }),
	};
	fetch(apiUrl + "login", options).then((response) => {
		if (!response.ok) {
			throw new Error('Network response was not ok');
		}
		document.getElementById("loading-modal-div").style.display = "none";
		return response.json();
	}).then((data) => {
		getMethods();
		document.getElementById("login-modal-div").style.display = "none";
	}).catch((error) => {
		document.getElementById("login-message").style.display = "flex";
		console.error("Error:", error);
	});
}

function togglePassword() {
	let input = document.getElementById("login-form-password-input")
	if (input.type == "password")
		input.type = "text";
	else
		input.type = "password";
}