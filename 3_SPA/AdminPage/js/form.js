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
