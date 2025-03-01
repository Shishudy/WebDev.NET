const apiUrl = "http://localhost:5164/";

document.addEventListener("DOMContentLoaded", function () {
	document.getElementById("login-modal-div").style.display = "flex";
});

document.body.addEventListener("submit", function (event) {
	event.preventDefault();
	if (event.target.id === "login-form")
		login(event.target.elements);
	else if (event.target.id === "form-input")
		submitForm(event);
});

let methods;

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

function getMethods() {
	fetch(apiUrl + "methods").then((response) => {
		if (!response.ok) {
			throw new Error('Network response was not ok');
		}
		return response.json();
	}).then((data) => {
		methods = data;
		buildMethodSelector();
	}).catch((error) => {
		console.error("Error:", error);
	});
}

function buildMethodSelector() {
	const selectorDiv = document.getElementById("method-selector-div");
	if (selectorDiv.firstElementChild)
		return ;
	const selector = document.createElement("select");
	selector.setAttribute("id", "method-selector");
	selector.setAttribute("onchange", "buildForm()");
	let option;
	for (const key in methods) {
		option = document.createElement("option");
		option.setAttribute("value", key);
		option.textContent = key.charAt(0).toUpperCase() + key.slice(1);
		selector.appendChild(option);
	}
	selectorDiv.appendChild(selector);
}

function buildForm() {
	const selector = document.getElementById('method-selector');
	const formInputDiv = document.getElementById('form-input-div');
	if (formInputDiv.firstElementChild)
		formInputDiv.removeChild(formInputDiv.firstElementChild);
	const formInput = document.createElement('form');
	formInput.setAttribute('id', 'form-input');
	let formStructure = methods[selector.value];
	if (formStructure)
	{
		formStructure.forEach(field => {
			const label = document.createElement('label');
			label.setAttribute('for', field.name);
			label.textContent = field.name;
	
			const input = document.createElement('input');
			input.setAttribute('id', field.name);
			input.setAttribute('type', field.type);
			if (field.size)
				input.setAttribute('size', field.size);
			if (field.mandatory) {
				input.required = true;
			}
			formInput.appendChild(label);
			formInput.appendChild(input);
		});
		const clearInput = document.createElement('button');
		clearInput.setAttribute('id', 'form-clear-button');
		clearInput.setAttribute('type', 'reset');
		clearInput.setAttribute('value', 'Reset form');
		clearInput.textContent = "Clear";
		formInput.appendChild(clearInput);
	}
	const submitButton = document.createElement('button');
	submitButton.setAttribute('id', 'form-submit-button');
	submitButton.setAttribute('type', 'submit');
	submitButton.textContent = 'Submit';
	formInput.appendChild(submitButton);
	formInputDiv.appendChild(formInput);
}

function clearForm() {
	document.getElementById("form-input").reset();
	console.log("Cleared!");
}

function submitForm(data) {
	console.log(data);
	console.log("Submited!");
}

function togglePassword() {
	let input = document.getElementById("login-form-password-input")
	if (input.type == "password")
		input.type = "text";
	else
		input.type = "password";
}
