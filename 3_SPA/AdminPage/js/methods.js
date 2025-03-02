let methods;

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