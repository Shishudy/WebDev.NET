function showFormModal() {
	document.getElementById("form-modal-div").style.display = "flex";
	if (!methods)
		getMethods();
	else
		buildMethodSelector();
}

function buildForm() {
	const selector = document.getElementById('method-selector');
	const form = document.getElementById('form-modal-form');
	while (form.firstElementChild)
		form.removeChild(form.firstElementChild);
	form.setAttribute('id', 'form-modal-form');
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
			form.appendChild(label);
			form.appendChild(input);
		});
		const clearInput = document.createElement('button');
		clearInput.setAttribute('id', 'modal-form-clear-button');
		clearInput.setAttribute('type', 'reset');
		clearInput.setAttribute('value', 'Reset form');
		clearInput.textContent = "Clear";
		form.appendChild(clearInput);
	}
	const submitButton = document.createElement('button');
	submitButton.setAttribute('id', 'modal-form-submit-button');
	submitButton.setAttribute('type', 'submit');
	submitButton.textContent = 'Submit';
	form.appendChild(submitButton);
}

function submitForm(data) {
	console.log(data);
	const selector = document.getElementById('method-selector');
	fetch(apiUrl + `ResolveMethod/${selector.value}`).then((response) => {
		if (!response.ok) {
			throw new Error('Network response was not ok');
		}
		return response.json();
	}).then((data) => {
		console.log(data);
	}).catch((error) => {
		console.error("Error:", error);
	});
	console.log("Submited!");
}
