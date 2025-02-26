// document.addEventListener("DOMContentLoaded", function() {
//     const loginModal = document.getElementById("loginModal");
//     const overlay = document.getElementById("overlay");
//     const loginForm = document.getElementById("loginForm");
//     const welcomeUser = document.getElementById("welcomeUser");
//     const loginButton = document.getElementById("loginButton");

//     // Verifica se o usuário já está logado
//     if (localStorage.getItem("usuarioLogado")) {
//         liberarAcesso();
//     } else {
//         loginModal.style.display = "block";
//         overlay.style.display = "block";
//     }

//     loginForm.addEventListener("submit", function(event) {
//         event.preventDefault();
//         const username = document.getElementById("username").value;
//         const password = document.getElementById("password").value;

//         // Aqui você pode verificar o usuário e senha com um banco de dados ou API externa.

//         // fetch('https://example.com/protected-endpoint', {
//         //     method: 'POST',
//         //     headers: {
//         //         "Authorization": "Bearer your-jwt-token",   // For authentication
//         //         "Content-Type": "application/json"          // To indicate that you're sending JSON data
//         //     },
//         //     body: JSON.stringify({ key: 'value' })          // Data you're sending in the body
//         // })
//         // .then(response => response.json())
//         // .then(data => console.log(data))
//         // .catch(error => console.error("Error:", error));

//         // fetch("http://localhost:5000/login", {
//         //     method: "POST",
//         //     headers: {
//         //       "Content-Type": "application/json"
//         //     },
//         //     body: JSON.stringify({ login, password })
//         //   })
//         //     .then(response => response.json())
//         //     .then(data => liberarAcesso())
//         //     .catch(error => alert("Error:", error));
//         if (username === "admin" && password === "1234") {
//             localStorage.setItem("usuarioLogado", username);
//             liberarAcesso();
//         } else {
//             alert("Usuário ou senha incorretos!");
//         }
//     });

//     function liberarAcesso() {
//         loginModal.style.display = "none";
//         overlay.style.display = "none";
//         welcomeUser.innerText = "Bem-vindo, " + localStorage.getItem("usuarioLogado") + "!";
//         welcomeUser.style.display = "inline";
//         loginButton.style.display = "none";
//     }

//     // Para permitir logout
//     function logout() {
//         localStorage.removeItem("usuarioLogado");
//         location.reload(); // Recarrega a página para exigir login novamente
//     }

//     // Adiciona um botão de logout opcional
//     let logoutButton = document.createElement("button");
//     logoutButton.innerText = "Sair";
//     logoutButton.style.marginLeft = "10px";
//     logoutButton.onclick = logout;
//     welcomeUser.appendChild(logoutButton);
// });

const loginModal = document.getElementById("loginModal");
const overlay = document.getElementById("overlay");
const loginForm = document.getElementById("loginForm");
const welcomeUser = document.getElementById("welcomeUser");
const loginButton = document.getElementById("loginButton");
const apiUrl = "http://localhost:5164/";

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

let methods = getMethods();

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
			document.getElementById("loginModal").style.display = "none";
			buildPage();
		})
		.catch((error) => {
			console.error("Error:", error);
		});
});

document.addEventListener("DOMContentLoaded", function () {
	document.getElementById("loginModal").style.display = "flex";
	document.body.classList.add("blurred");
});

function closeLoginModal() {
	document.getElementById("loginModal").style.display = "none";
	document.body.classList.remove("blurred");
}

function buildPage() {
	buildCatSelector();
}

function buildCatSelector() {
	const page = document.getElementById("lista-livros");
	const catSelectorDiv = document.createElement("div");
	catSelectorDiv.setAttribute("id", "select-cat-div");
	catSelectorDiv.setAttribute("class", "select-cat-div");
	const catSelector = document.createElement("select");
	catSelector.setAttribute("id", "select-cat");
	let catOption;
	for (const key in methods) {
		catOption = document.createElement("option");
		catOption.setAttribute("value", key);
		catOption.textContent = key.charAt(0).toUpperCase() + key.slice(1);
		catSelector.appendChild(catOption);
	}
	catSelectorDiv.appendChild(catSelector);
	button = document.createElement("button");
	button.setAttribute("id", "clear-filter-button");
	// button.setAttribute("onclick", "clearFilter()");
	button.setAttribute("style", "float: right");
	button.textContent = "Limpar filtro";
	catSelectorDiv.appendChild(button);
	page.appendChild(catSelectorDiv);
	// Add event listener to the category selector
	catSelector.addEventListener("change", function () {
		const selectedMethod = catSelector.value;
		fetchFormStructure(selectedMethod);
	});
}

function fetchFormStructure(method) {
    fetch(apiUrl + "ResolveMethod" + method)
        .then((response) => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then((formStructure) => {
            buildForm(formStructure);
        })
        .catch((error) => {
            console.error("Error:", error);
        });
}

const form = document.getElementById("form");
const pdf = document.getElementById("file");

function BuildFormByObject(obj) {
	
}
function buildForm(formStructure) {
	const formContainer = document.getElementById('form-container');
	const form = document.createElement('form');
	form.setAttribute('id', 'dynamicForm');

	formStructure.forEach(field => {
		const formGroup = document.createElement('div');
		formGroup.setAttribute('class', 'form-group');

		const label = document.createElement('label');
		label.setAttribute('for', field.name);
		label.textContent = field.name;

		const input = document.createElement('input');
		input.setAttribute('type', field.type);
		input.setAttribute('id', field.name);
		input.setAttribute('name', field.name);
		input.setAttribute('size', field.size);
		if (field.mandatory) {
			input.setAttribute('required', 'required');
		}

		formGroup.appendChild(label);
		formGroup.appendChild(input);
		form.appendChild(formGroup);
	});

	const submitButton = document.createElement('button');
	submitButton.setAttribute('type', 'submit');
	submitButton.textContent = 'Submit';
	form.appendChild(submitButton);

	formContainer.appendChild(form);
}






// document.getElementById('imageUploadForm').addEventListener('submit', function(event) {
// 	event.preventDefault(); // Prevent form submission

// 	var formData = new FormData(event.target);
// 	console.log(formData);
// 	fetch(apiUrl + "teste", {
// 		method: 'POST',
// 		headers: {
// 			"Content-Type": "application/json",
// 		},
// 		body: JSON.stringify({ formData })
// 	})
// 	.then(response => response.json())
// 	.then(data => {
// 		console.log('Success:', data);
// 	})
// 	.catch((error) => {
// 		console.error('Error:', error);
// 	});
// });

// form.addEventListener("submit", function (event) {
// 	event.preventDefault();
// 	console.log(event);
// 	console.log(pdf.value);
// 	var options = {
// 		method: "POST",
// 		headers: {
// 			"Content-Type": "application/json",
// 		},
// 		body: JSON.stringify({ }),
// 	};
// 	fetch(apiUrl + "teste", options)
// 		.then((response) => {
// 			if (!response.ok) {
// 			  throw new Error('Network response was not ok');
// 			}
// 			return response.json();
// 		})
// 		.then((data) => {
// 			console.log(data);
// 		})
// 		.catch((error) => {
// 			console.error("Error:", error);
// 		});
// });