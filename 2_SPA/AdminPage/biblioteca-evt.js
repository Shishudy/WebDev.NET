// const loginButton = document.getElementById("loginButton");
// const loginModal = document.getElementById("loginModal");
// const loginForm = document.getElementById("loginForm");
// const loginUsername = document.getElementById("username");
// const loginPassword = document.getElementById("password");
// const welcomeUser = document.getElementById("welcomeUser");
// const searchIcon = document.getElementById("pesquisa");
// const searchInput = document.getElementById("buscador");

// loginForm.addEventListener("submit", login);
// searchIcon.addEventListener("submit", searchInDB);

// window.onclick = function (event) {
// 	if (event.target == loginModal) {
// 		closeLoginModal();
// 	}
// };

// function openLoginModal() {
// 	loginModal.style.display = "block";
// }

// function closeLoginModal() {
// 	loginUsername.value = "";
// 	loginPassword.value = "";
// 	loginModal.style.display = "none";
// }

// function authentication() {
// 	if (utilizador.getUsername === undefined) {
// 		openLoginModal();
// 		return false;
// 	}
// 	return true;
// }

// function afterLogin() {
// 	loginButton.style.display = "none";
// 	welcomeUser.style.display = "block";
// 	welcomeUser.textContent = "Bem-vindo, " + utilizador.getUsername + "!";
// }

// function login(event) {
// 	event.preventDefault();
// 	if (loginUsername.value == "") {
// 		showPopup("Por favor, insira um nome de usuário!");
// 		return;
// 	}
// 	if (loginPassword.value.length < 4) {
// 		showPopup("A senha deve ter pelo menos 4 caracteres!");
// 		loginPassword.value = "";
// 		return;
// 	}
// 	utilizador = new User({ username: loginUsername.value, password: loginPassword.value });
// 	showPopup("Login realizado com sucesso!");
// 	closeLoginModal();
// 	afterLogin();
// 	buildPage();
// }

// function searchInDB(event) {
// 	event.preventDefault();
// 	cardListArray = db.filter(obj => Object.values(obj).some(value => String(value).toLowerCase().includes(searchInput.value.toLowerCase())));
// 	createCardListArray(cardListArray);
// 	clearPage();
// 	buildCatSelector();
// 	buildCardList();
// 	const bookList = document.getElementById("lista-livros");
// 	bookList.firstElementChild.textContent = "Pesquisa por: " + searchInput.value;
// 	searchInput.value = "";
// }
