const apiUrl = "http://localhost:5164/";

document.addEventListener("DOMContentLoaded", function () {
	document.getElementById("login-modal-div").style.display = "flex";
});

document.body.addEventListener("submit", function (event) {
	event.preventDefault();
	if (event.target.id === "login-form")
		login();
	else if (event.target.id === "form-submit-button")
		submitForm();
	else if (event.target.id === "form-clear-button")
		buildForm();
});

let methods;

function login()
{
	const username = document.getElementById("login-form-username-input").value;
	const password = document.getElementById("login-form-password-input").value;
	var options = {
		method: "POST",
		headers: {
			"Content-Type": "application/json",
		},
		body: JSON.stringify({ username, password }),
	};
	fetch(apiUrl + "login", options)
	.then((response) => {
		if (!response.ok) {
			throw new Error('Network response was not ok');
		}
		return response.json();
	})
	.then((data) => {
		document.getElementById("login-modal-div").style.display = "none";
		getMethods();
	})
	.catch((error) => {
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
	// button = document.createElement("button");
	// button.setAttribute("id", "method-selector-button");
	// button.setAttribute("onclick", "clearFilter()");
	// button.setAttribute("style", "float: right");
	// button.textContent = "Limpar filtro";
	// selectorDiv.appendChild(button);
	selectorDiv.appendChild(selector);
}

function buildForm() {
	console.log("called");
	const selector = document.getElementById('method-selector');
	const formInputDiv = document.getElementById('form-input-div');
	if (formInputDiv.firstElementChild)
		formInputDiv.removeChild(formInputDiv.firstElementChild);
	const formInput = document.createElement('form-input');
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
				input.setAttribute('required', 'required');
			}
			formInput.appendChild(label);
			formInput.appendChild(input);
		});
	}
	const clearButton = document.createElement('button');
	clearButton.setAttribute('id', 'form-clear-button');
	clearButton.setAttribute('type', 'submit');
	clearButton.textContent = 'Clear';
	formInput.appendChild(clearButton);
	const submitButton = document.createElement('button');
	submitButton.setAttribute('id', 'form-submit-button');
	submitButton.setAttribute('type', 'submit');
	submitButton.textContent = 'Submit';
	formInput.appendChild(submitButton);
	formInputDiv.appendChild(formInput);
}

function submitForm() {

}

