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
		document.getElementById("loading-modal-div").style.display = "none";
		if (!response.ok) {
			alert("Error logging in. Please try again later.");
			throw new Error('Network response was not ok');
		}
		return response.json();
		}).then((data) => {
			if (data) {
				sessionStorage.setItem("userData", JSON.stringify(data));
				// Let's implement JWT and we can save the token created in the sessionStorage
				getMethods();
				document.getElementById("login-modal-div").style.display = "none";
			}
			else {
				document.getElementById("login-message").style.display = "flex";
			}
		}).catch((error) => {
			console.error("Error:", error);
		});
	// TO SIMULATE LOADING TIME
	// setTimeout(function(){
	// 	fetch(apiUrl + "login", options).then((response) => {
	// 		document.getElementById("loading-modal-div").style.display = "none";
	// 		if (!response.ok) {
	// 			alert("Error logging in. Please try again later.");
	// 			throw new Error('Network response was not ok');
	// 		}
	// 		return response.text();
	// 	}).then((data) => {
	// 		console.log(data);
	// 		getMethods();
	// 		document.getElementById("login-modal-div").style.display = "none";
	// 	}).catch((error) => {
	// 		document.getElementById("login-message").style.display = "flex";
	// 		console.error("Error:", error);
	// 	});

	// }, 3000);
}

function togglePassword() {
	let input = document.getElementById("login-form-password-input")
	if (input.type == "password")
		input.type = "text";
	else
		input.type = "password";
}

function logout() {
	sessionStorage.removeItem('userData');
	location.reload(true);
}