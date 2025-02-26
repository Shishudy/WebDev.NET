const apiUrl = "http://localhost:5164/";
let methods;

document.addEventListener("DOMContentLoaded", function () {
	document.getElementById("loginModal").style.display = "flex";
});

loginForm.addEventListener("submit", function (event) {
	event.preventDefault();
	const username = document.getElementById("username").value;
	const password = document.getElementById("password").value;
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
		console.log(data);
		initPage();
		document.getElementById("loginModal").style.display = "none";
	})
	.catch((error) => {
		console.error("Error:", error);
	});
});

function getMethods() {
	fetch(apiUrl + "methods").then((response) => {
		if (!response.ok) {
			throw new Error('Network response was not ok');
		}
		return response.json();
	}).then((data) => {
		methods = data;
		console.log(methods);
		return (methods);
	}).catch((error) => {
		console.error("Error:", error);
	});
}

function initPage() {
	methods = getMethods();
	buildMethodSelector();
}

function buildMethodSelector() {
	const selectorDiv = document.getElementById("method-selector");
	const selector = document.createElement("select");
	let catOption;
	for (const key in methods) {
		catOption = document.createElement("option");
		catOption.setAttribute("value", key);
		// catOption.textContent = key.charAt(0).toUpperCase() + key.slice(1);
		selector.appendChild(catOption);
	}
	selectorDiv.appendChild(selector);
	// button = document.createElement("button");
	// button.setAttribute("id", "clear-filter-button");
	// button.setAttribute("onclick", "clearFilter()");
	// button.setAttribute("style", "float: right");
	// button.textContent = "Limpar filtro";
	// selectorDiv.appendChild(button);
	selectorDiv.appendChild(selector);
	// Add event listener to the category selector
	// catSelector.addEventListener("change", function () {
	// 	const selectedMethod = catSelector.value;
	// 	fetchFormStructure(selectedMethod);
	// });
}